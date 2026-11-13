using System;
using Airborne.Building;
using BeauUtil.Debugger;
using FieldDay.SharedState;

namespace Airborne.Schedule {
    public sealed class ScheduleState : SharedStateComponent {
        [NonSerialized] public TimePhase Phase;
        [NonSerialized] public int CurrentHourIndex;
        [NonSerialized] public int LoopCount;
    }

    public enum TimePhase {
        Ready,
        Observation,
        Mitigation,
        Ended
    }

    static public partial class ScheduleUtility {
    }
}