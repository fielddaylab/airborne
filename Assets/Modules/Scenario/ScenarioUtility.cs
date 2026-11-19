using System;
using System.Collections;
using Airborne.Building;
using Airborne.Inventory;
using Airborne.Pollutants;
using Airborne.Schedule;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Debugging;
using FieldDay.Scripting;

namespace Airborne.Scenario {
    static public partial class ScenarioUtility {
        #region Initialization

        static public void AllocateBuffers(BuildingScenario scenario, TimeScheduler timeScheduler, ActorScheduler actorScheduler, Simulator simulator, SimulationCache cache, InventoryState inv, BuildingState buildingState, BuildingManifestCounts objectCounts) {
            BuildingUtility.AllocateStateBuffers(buildingState, objectCounts, scenario.MaxMeters);
            ScheduleUtility.AllocateTimelineBuffers(actorScheduler, objectCounts.ResidentCount, objectCounts.FeatureCount, objectCounts.PassageCount);
            SimulatorUtility.AllocatePollutantBuffers(simulator, objectCounts.RoomCount, objectCounts.FeatureCount, scenario.Pollutants.Length);
            SimulatorUtility.AllocateCache(cache, objectCounts.RoomCount);
            InventoryUtility.AllocateBuffers(inv, objectCounts);
        }

        static public void LoadDefaultSchedules(TimeScheduler timeScheduler, ActorScheduler actorScheduler, Simulator simulator, ScheduleStartupType startupType) {
            Assert.False(timeScheduler.IsInitializing, "Cannot nest RunScheduleStartup calls");
            timeScheduler.IsInitializing = true;

            ScheduleUtility.ClearTimelines(actorScheduler);
            SimulatorUtility.ResetInitialStates(simulator);

            int ran = ScriptUtility.Invoke("LoadSchedules");
            if (ran <= 0) {
                DebugDraw.AddLogText("No \"LoadSchedules\" functions invoked - schedules will be empty!", ColorBank.DarkRed, 5);
            }

            ScheduleUtility.PatchTimelineHoles(actorScheduler, timeScheduler.HourCount);
            bool scheduleIsDifferent = ScheduleUtility.ApplyTimelineHash(actorScheduler);

            if (ran > 0 && scheduleIsDifferent && startupType == ScheduleStartupType.HotReload) {
                DebugDraw.AddLogText("Actor schedules have changed - restart the loop to see them applied", ColorBank.LightYellow, 3);
            }

            timeScheduler.IsInitializing = false;
        }

        #endregion // Initialization

        #region Loop

        static public void WriteInitialDataForLoop(ActorScheduler actorScheduler, Simulator simulator, InventoryState inv) {
            ScheduleUtility.MakeTimelineLive(actorScheduler);
            SimulatorUtility.ApplyInitialStates(simulator);
            InventoryUtility.ClearItems(inv);
        }

        static public void ApplyActorSchedulesToState(int hourIndex, ActorScheduler actorSchedule, BuildingState state) {
            int hourBitCheck = (1 << hourIndex);

            // passages

            for (int i = 0, len = actorSchedule.LivePassageTimelines.Length; i < len; i++) {
                bool isEnabled = (actorSchedule.LivePassageTimelines[i].EnabledMask & hourBitCheck) != 0;
                state.EnabledPassages.Set(i, isEnabled);
            }

            // features

            for (int i = 0, len = actorSchedule.LiveFeatureTimelines.Length; i < len; i++) {
                bool isEnabled = (actorSchedule.LiveFeatureTimelines[i].EnabledMask & hourBitCheck) != 0;
                state.EnabledFeatures.Set(i, isEnabled);
            }

            // residents

            for (int i = 0, len = actorSchedule.LiveResidentTimelines.Length; i < len; i++) {
                ResidentTimeBlock block = actorSchedule.LiveResidentTimelines[i][hourIndex];
                ref RuntimeResidentState residentState = ref state.Residents[i];
                residentState.RoomIndex = block.RoomIndex;
                residentState.PostIndex = block.PostIndex;
                residentState.Flags = block.Flags;
                residentState.Symptom = block.Symptom;
                residentState.ScriptNode = block.ScriptNode;
            }
        }

        #endregion // Loop

        #region Pollutants

        /// <summary>
        /// Returns the index of the given pollutant.
        /// </summary>
        static public int GetPollutantIndex(BuildingScenario scenario, StringHash32 pollutantId) {
            Assert.NotNullOrDestroyed(scenario);
            int index = Array.IndexOf(scenario.Pollutants, pollutantId);
            Assert.True(index >= 0, "Pollutant '{0}' not present in scenario", pollutantId);
            return index;
        }

        /// <summary>
        /// Attempts to return the index of the given pollutant.
        /// </summary>
        static public bool TryGetPollutantIndex(BuildingScenario scenario, StringHash32 pollutantId, out int index) {
            Assert.NotNullOrDestroyed(scenario);
            index = Array.IndexOf(scenario.Pollutants, pollutantId);
            return index >= 0;
        }

        #endregion // Pollutants

        #region Cache Generation

        static public IEnumerator GenerateSimulationCacheTask() {
            using (Profiling.Time("generating simulation cache")) {
                Find.State(out SimulationCache cache, out Simulator sim, out BuildingState building, out BuildingManifest manifest);
                Find.State(out ActorScheduler scheduler, out TimeScheduler time, out BuildingScenario scenario, out InventoryState inv);

                Log.Msg("[ScenarioUtility] Generating simulation cache...");
                ScenarioUtility.WriteInitialDataForLoop(scheduler, sim, inv);
                ScenarioUtility.ApplyActorSchedulesToState(0, scheduler, building);
                SimulatorUtility.RecordToCache(cache, sim, 0);
                yield return null;

                for (int i = 1; i < time.HourCount; i++) {
                    Log.Msg("[ScenarioUtility] Simulating for hour {0}...", i);
                    SimulatorUtility.ApplyPassageStates(sim, building.EnabledPassages);
                    SimulatorUtility.AccumulateFeatureContributions(sim, building.EnabledFeatures);
                    SimulatorUtility.Simulate(sim);
                    SimulatorUtility.RecordToCache(cache, sim, i);
                    ScenarioUtility.ApplyActorSchedulesToState(i, scheduler, building);
                    yield return null;
                }

                Log.Msg("[ScenarioUtility] Simulation cache generated!");
            }
        }

        #endregion // Cache Generation
    }

    public enum ScheduleStartupType {
        Initialization,
        HotReload
    }
}