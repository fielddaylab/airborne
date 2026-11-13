using System;
using Airborne.Building;
using Airborne.Pollutants;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.Debugging;
using FieldDay.Scripting;
using FieldDay.SharedState;

namespace Airborne.Schedule {
    public sealed class TimeScheduler : SharedStateComponent {
        [NonSerialized] public int MinHour = -1;
        [NonSerialized] public int MaxHour = -1;
        [NonSerialized] public int HourCount;

        [NonSerialized] public bool IsInitializing;
    }

    static public partial class ScheduleUtility {
        #region Hours

        /// <summary>
        /// Defines the range of hours represented on the timeline.
        /// </summary>
        static public void DefineHourRange(TimeScheduler scheduler, int startHourClock, int endHourClock) {
            Assert.NotNullOrDestroyed(scheduler);

            Assert.True(scheduler.MinHour < 0 || (startHourClock == scheduler.MinHour && scheduler.MaxHour == endHourClock), "Cannot change time range without reloading level");

            Assert.True(endHourClock > startHourClock, "End hour {0} must be greater than start hour {1}", endHourClock, startHourClock);
            Assert.True(startHourClock > 0 && startHourClock < 24, "Start hour {0} outside accepted range 0-23", startHourClock);
            Assert.True(endHourClock > 0 && endHourClock <= 24, "End hour {0} outside accepted range 0-24", endHourClock);

            scheduler.MinHour = startHourClock;
            scheduler.MaxHour = endHourClock;
            scheduler.HourCount = endHourClock - startHourClock;

            Assert.True(scheduler.HourCount <= BuildingConsts.MaxHours, "Range {0}-{1} exceeds maximum number of hours {2}", startHourClock, endHourClock, BuildingConsts.MaxHours);
        }

        /// <summary>
        /// Returns the indexed hour as its 24-hour clock time.
        /// </summary>
        static public int GetClockHour(TimeScheduler scheduler, int hourIndex) {
            Assert.NotNullOrDestroyed(scheduler);
            Assert.True(hourIndex >= 0 && hourIndex <= scheduler.HourCount, "Hour index {0} not in range (0-{1})", hourIndex, scheduler.HourCount);
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