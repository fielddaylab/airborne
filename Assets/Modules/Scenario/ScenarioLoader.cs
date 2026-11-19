using System.Collections.Generic;
using Airborne.Building;
using Airborne.Inventory;
using Airborne.Pollutants;
using Airborne.Scenario;
using Airborne.Schedule;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Job;
using FieldDay.Scenes;
using FieldDay.Scripting;
using UnityEngine;

namespace Airborne.Scenario {
    [PreloadOrder(1000), LateInitializeOrder(10000)]
    public sealed class ScenarioLoader : SceneController {
        protected override IEnumerator<WorkSlicer.Result?> OnScenePreload() {
            Find.State(out BuildingScenario scenario, out InventoryState inv, out SimulationCache cache, out BuildingState buildingState);
            Find.State(out TimeScheduler time, out ActorScheduler actors, out BuildingManifest manifest, out Simulator sim);

            BuildingManifestCounts counts = BuildingLookup.CountObjects(manifest);

            // initialize buffers
            Log.Msg("[BuildingLoader] Allocating buffers...");
            ScenarioUtility.AllocateBuffers(scenario, time, actors, sim, cache, inv, buildingState, counts);
            yield return null;

            // build static graph
            Log.Msg("[BuildingLoader] Allocating simulation graph...");
            SimulatorUtility.AllocateGraph(sim, manifest);
            yield return null;

            // wait for scripts to finish loading
            while(ScriptDBUtility.IsLoading()) {
                yield return null;
            }

            // run initialization script
            ScenarioUtility.LoadDefaultSchedules(time, actors, sim, ScheduleStartupType.Initialization);
        }

        protected override void OnSceneEnable() {
            // run simulation result caching
            Jobs.PushLoadDependency(
                ScenarioUtility.GenerateSimulationCacheTask(),
                AsyncFlags.MainThreadOnly | AsyncFlags.HighPriority
                );
        }
    }
}