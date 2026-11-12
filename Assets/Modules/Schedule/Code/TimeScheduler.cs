using System;
using Airborne.Building;
using BeauUtil.Debugger;
using FieldDay.SharedState;

namespace Airborne.Schedule {
    public sealed class TimeScheduler : SharedStateComponent {
        [NonSerialized] public int MinHour;
        [NonSerialized] public int MaxHour;
        [NonSerialized] public int HourCount;

        [NonSerialized] public TimePhase Phase;
        [NonSerialized] public int CurrentHourIndex;
        [NonSerialized] public int LoopCount;
    }

    public enum TimePhase {
        Uninitialized,
        Ready,
        Observation,
        Mitigation,
        Ended
    }

    static public partial class ScheduleUtility {
        #region Hours

        /// <summary>
        /// Defines the range of hours represented on the timeline.
        /// NOTE: This is an inclusive range (e.g. 8-9 will allocate 2 hours)
        /// </summary>
        static public void DefineHourRange(TimeScheduler scheduler, int startHourClock, int endHourClock) {
            Assert.NotNullOrDestroyed(scheduler);

            Assert.True(scheduler.Phase <= TimePhase.Ready || (startHourClock == scheduler.MinHour && scheduler.MaxHour == endHourClock), "Cannot change time range during cycle");

            Assert.True(endHourClock >= startHourClock, "End hour {0} must not be less than start hour {1}", endHourClock, startHourClock);
            Assert.True(startHourClock > 0 && startHourClock < 24, "Hour {0} outside accepted range 0-23", startHourClock);
            Assert.True(endHourClock > 0 && endHourClock < 24, "Hour {0} outside accepted range 0-23", endHourClock);

            scheduler.MinHour = startHourClock;
            scheduler.MaxHour = endHourClock;
            scheduler.HourCount = endHourClock - startHourClock + 1;

            Assert.True(scheduler.HourCount <= BuildingConsts.MaxHours, "Range {0}-{1} exceeds maximum number of hours {2}", startHourClock, endHourClock, BuildingConsts.MaxHours);
        }

        /// <summary>
        /// Returns the indexed hour as its 24-hour clock time.
        /// </summary>
        static public int GetClockHour(TimeScheduler scheduler, int hourIndex) {
            Assert.NotNullOrDestroyed(scheduler);
            Assert.True(hourIndex >= 0 && hourIndex < scheduler.HourCount, "Hour index {0} not in range (0-{1})", hourIndex, scheduler.HourCount);
            return scheduler.MinHour + hourIndex;
        }

        /// <summary>
        /// Returns the index for the given 24-hour clock time.
        /// </summary>
        static public int GetHourIndex(TimeScheduler scheduler, int hourClock) {
            Assert.NotNullOrDestroyed(scheduler);
            int hourIndex = hourClock - scheduler.MinHour;
            Assert.True(hourIndex >= 0 && hourIndex < scheduler.HourCount, "Hour {0} not in range ({1}-{2})", hourClock, scheduler.MinHour, scheduler.MinHour + scheduler.HourCount - 1);
            return hourIndex;
        }

        #endregion // Hours
    }
}