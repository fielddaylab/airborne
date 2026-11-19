using System.Collections.Generic;
using Airborne.Inventory;
using Airborne.Pollutants;
using Airborne.Schedule;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Scenes;
using FieldDay.Scripting;
using UnityEngine;

namespace Airborne.Building {
    [PreloadOrder(1000), LateInitializeOrder(10000)]
    public sealed class BuildingLoader : SceneController {
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
            Find.State(out BuildingScenario scenario, out InventoryState inv, out SimulationCache cache);
            Find.State(out TimeScheduler time, out ActorScheduler actors, out BuildingManifest manifest, out Simulator sim);
        }

        protected override void OnSceneReady() {
            Find.State(out BuildingScenario scenario, out InventoryState inv, out SimulationCache cache, out BuildingState buildingState);
            Find.State(out TimeScheduler time, out ActorScheduler actors, out BuildingManifest manifest, out Simulator sim);

            ScenarioUtility.WriteInitialDataForLoop(actors, sim, inv);

            Log.Msg("Hour 0: {0}", ScheduleUtility.GetClockHour(time, 0));
            SimulatorUtility.DumpPollutantLevels(sim.RoomConditions.Current, manifest.RoomNames, scenario.Pollutants);
            ScenarioUtility.ApplyActorSchedulesToState(0, actors, buildingState);

            BuildingUtility.SyncResidentVisualPositions(buildingState, manifest);

            SimulatorUtility.RecordToCache(cache, sim, 0);

            for (int i = 1; i < time.HourCount; i++) {
                Log.Msg("Hour {0}: {1}", i, ScheduleUtility.GetClockHour(time, i));
                SimulatorUtility.ApplyPassageStates(sim, buildingState.EnabledPassages);
                SimulatorUtility.AccumulateFeatureContributions(sim, buildingState.EnabledFeatures);
                SimulatorUtility.Simulate(sim);
                SimulatorUtility.DumpPollutantLevels(sim.RoomConditions.Current, manifest.RoomNames, scenario.Pollutants);
                SimulatorUtility.RecordToCache(cache, sim, i);

                ScenarioUtility.ApplyActorSchedulesToState(i, actors, buildingState);
            }

            ScenarioUtility.ApplyActorSchedulesToState(4, actors, buildingState);
            BuildingUtility.SyncResidentVisualPositions(buildingState, manifest);
        }
    }
}