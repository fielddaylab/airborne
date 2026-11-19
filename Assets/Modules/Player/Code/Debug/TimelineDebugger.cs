using System.Collections;
using System.Collections.Generic;
using Airborne.Building;
using Airborne.Pollutants;
using Airborne.Scenario;
using Airborne.Schedule;
using BeauPools;
using BeauUtil;
using FieldDay;
using FieldDay.Debugging;
using FieldDay.Systems;
using UnityEngine;

namespace Airborne
{
    public class TimelineDebugger : SystemComponent
    {
        public override unsafe void RegisterSystems(ref SystemRegistrationTable ecs) {
            ecs.Register(&Update, new SysUpdate(GameLoopPhase.DebugUpdate, 0), default);
        }

        static private void Update(float dt) {
            Find.State(out TimeScheduler time);
            for(int i = 0; i < time.HourCount; i++) {
                KeyCode key = KeyCode.Alpha1 + i;
                if (DebugInput.IsPressed(key)) {
                    DebugJumpToTimeIndex(i);
                    break;
                }
            }

            Find.State(out Simulator simulator, out BuildingState building, out BuildingManifest manifest, out BuildingScenario scenario);
            using(PooledStringBuilder psb = PooledStringBuilder.Create()) {
                int roomCount = building.Rooms.Length;
                int pollutantCount = simulator.PollutantTypeCount;
                for(int i = 0; i < roomCount; i++) {
                    psb.Builder.Append("Room ").Append(manifest.RoomNames[i].ToDebugString());
                    PollutantLevels levels = simulator.RoomConditions.Current[i];
                    for (int p = 0; p < pollutantCount; p++) {
                        byte amount = levels[p];
                        if (amount > 0) {
                            psb.Builder.Append("\n\t").AppendNoAlloc(amount).Append(" ").Append(scenario.Pollutants[p].ToDebugString());
                        }
                    }
                    psb.Builder.Append("\n");
                }

                psb.Builder.TrimEnd(StringUtils.DefaultNewLineChars);

                DebugDraw.AddViewportText(new Vector2(1, 1), new Vector2(-16, -16), psb, Color.yellow, 0, TextAnchor.UpperRight, DebugTextStyle.BackgroundDark);
            }
        }

        static private void DebugJumpToTimeIndex(int index) {
            Find.State(out BuildingManifest manifest, out BuildingState state, out ActorScheduler scheduler);
            ScenarioUtility.ApplyActorSchedulesToState(index, scheduler, state);
            Find.State(out Simulator simulator, out SimulationCache cache);
            SimulatorUtility.UnpackFromCache(cache, simulator, index);

            BuildingUtility.SyncResidentVisualPositions(state, manifest);

            Find.State(out ScheduleState scheduleState);
            scheduleState.CurrentHourIndex = index;
        }
    }
}
