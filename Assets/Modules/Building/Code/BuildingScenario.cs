using System;
using Airborne.Pollutants;
using Airborne.Schedule;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.Assets;
using FieldDay.Debugging;
using FieldDay.Scripting;
using FieldDay.SharedState;
using Leaf;
using ScriptableBake;

namespace Airborne.Building {
    public sealed class BuildingScenario : SharedStateComponent, IBaked {
        [AssetName(typeof(PollutantDefinition))] public StringHash32[] Pollutants;

#if UNITY_EDITOR

        int IBaked.Order { get { return -10000; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            if (Pollutants.Length > 4) {
                Array.Resize(ref Pollutants, 4);
                return true;
            }

            return false;
        }

#endif // UNITY_EDITOR
    }

    static public partial class ScenarioUtility {
        #region Initialization

        static public void AllocateBuffers(BuildingScenario scenario, TimeScheduler timeScheduler, ActorScheduler actorScheduler, Simulator simulator, BuildingManifestCounts objectCounts) {
            ScheduleUtility.AllocateTimelineBuffers(actorScheduler, objectCounts.ResidentCount, objectCounts.FeatureCount, objectCounts.PassageCount);
            SimulatorUtility.AllocatePollutantBuffers(simulator, objectCounts.RoomCount, objectCounts.FeatureCount, scenario.Pollutants.Length);
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

        static public void WriteInitialDataForLoop(ActorScheduler actorScheduler, Simulator simulator) {
            ScheduleUtility.MakeTimelineLive(actorScheduler);
            SimulatorUtility.ApplyInitialStates(simulator);
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
    }

    public enum ScheduleStartupType {
        Initialization,
        HotReload
    }
}