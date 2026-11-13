using System.Collections.Generic;
using Airborne.Pollutants;
using Airborne.Schedule;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Scenes;
using FieldDay.Scripting;
using UnityEngine;

namespace Airborne.Building {
    [PreloadOrder(1000)]
    public sealed class BuildingLoader : SceneController {
        protected override IEnumerator<WorkSlicer.Result?> OnScenePreload() {
            Find.State(out BuildingScenario scenario);
            Find.State(out TimeScheduler time, out ActorScheduler actors, out BuildingManifest manifest, out Simulator sim);

            BuildingManifestCounts counts = BuildingLookup.CountObjects(manifest);

            // initialize buffers
            Log.Msg("[BuildingLoader] Allocating buffers...");
            ScenarioUtility.AllocateBuffers(scenario, time, actors, sim, counts);
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

        protected override void OnSceneReady() {
            Find.State(out BuildingScenario scenario);
            Find.State(out TimeScheduler time, out ActorScheduler actors, out BuildingManifest manifest, out Simulator sim);

            ScenarioUtility.WriteInitialDataForLoop(actors, sim);

            Log.Msg("Hour 0: {0}", ScheduleUtility.GetClockHour(time, 0));
            SimulatorUtility.DumpPollutantLevels(sim.RoomConditions.Current, manifest.RoomNames, scenario.Pollutants);
            sim.EnabledConnections = new BitSet64(Bits.AllU64);
            sim.SourceQueue.Add(new SimulatedSource() {
                RoomIndex = 0,
                PollutantIndex = 0,
                Adjustment = 1
            });
            for (int i = 1; i < time.HourCount; i++) {
                Log.Msg("Hour {0}: {1}", i, ScheduleUtility.GetClockHour(time, i));
                SimulatorUtility.Simulate(sim);
                SimulatorUtility.DumpPollutantLevels(sim.RoomConditions.Current, manifest.RoomNames, scenario.Pollutants);
            }
        }
    }
}