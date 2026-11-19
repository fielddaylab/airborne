using Airborne.Building;
using Airborne.Pollutants;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Scripting;
using Leaf.Runtime;

namespace Airborne.Schedule {
    static public class ScheduleScript {
        #region Time

        /// <summary>
        /// Defines the range of hours for the timeline.
        /// </summary>
        [LeafMember("DefineHourRange")]
        static public void DefineHourRange(int startClockHour, int endClockHour) {
            Find.State(out TimeScheduler time);
            ScheduleUtility.DefineHourRange(time, startClockHour, endClockHour);
        }

        #endregion // TIme

        #region Residents

        /// <summary>
        /// Schedules a resident's location for the given hour.
        /// </summary>
        [LeafMember("ScheduleResidentLocation")]
        static public void ScheduleResidentLocation(int clockHour, StringHash32 residentName, StringHash32 roomName, StringHash32 postName) {
            Find.State(out TimeScheduler time, out ActorScheduler scheduler, out BuildingManifest manifest);
            Assert.True(time.IsInitializing, "Default schedules can only be adjusted within a level initialization");
            int residentIndex = BuildingLookup.GetResidentIndex(manifest, residentName);
            int hourIndex = ScheduleUtility.GetHourIndex(time, clockHour);

            ref ResidentTimeline timeline = ref scheduler.DefaultResidentTimelines[residentIndex];
            ref ResidentTimeBlock block = ref timeline[hourIndex];

            ScheduleUtility.DefineResidentBlockLocation(ref block, roomName, postName, manifest);
        }

        /// <summary>
        /// Schedules a resident's symptom for the given hour.
        /// </summary>
        [LeafMember("ScheduleResidentSymptom")]
        static public void ScheduleResidentSymptom(int clockHour, StringHash32 residentName, StringSlice symptomName) {
            Find.State(out TimeScheduler time, out ActorScheduler scheduler, out BuildingManifest manifest);
            Assert.True(time.IsInitializing, "Default schedules can only be adjusted within a level initialization");
            int residentIndex = BuildingLookup.GetResidentIndex(manifest, residentName);
            int hourIndex = ScheduleUtility.GetHourIndex(time, clockHour);

            ref ResidentTimeline timeline = ref scheduler.DefaultResidentTimelines[residentIndex];
            ref ResidentTimeBlock block = ref timeline[hourIndex];

            ScheduleUtility.DefineResidentBlockSymptom(ref block, Symptom.Parse(symptomName));
        }

        /// <summary>
        /// Schedules a resident's script node for the given hour.
        /// </summary>
        [LeafMember("ScheduleResidentScript")]
        static public void ScheduleResidentScript(int clockHour, StringHash32 residentName, StringHash32 nodeId) {
            Find.State(out TimeScheduler time, out ActorScheduler scheduler, out BuildingManifest manifest);
            Assert.True(time.IsInitializing, "Default schedules can only be adjusted within a level initialization");
            int residentIndex = BuildingLookup.GetResidentIndex(manifest, residentName);
            int hourIndex = ScheduleUtility.GetHourIndex(time, clockHour);

            ref ResidentTimeline timeline = ref scheduler.DefaultResidentTimelines[residentIndex];
            ref ResidentTimeBlock block = ref timeline[hourIndex];

            ScheduleUtility.DefineResidentBlockScriptNode(ref block, nodeId);
        }

        /// <summary>
        /// Copies a resident's schedule from a different time.
        /// </summary>
        [LeafMember("ScheduleResidentCopy")]
        static public void ScheduleResidentCopy(int sourceClockHour, int destClockHour, StringHash32 residentName) {
            Find.State(out TimeScheduler time, out ActorScheduler scheduler, out BuildingManifest manifest);
            Assert.True(time.IsInitializing, "Default schedules can only be adjusted within a level initialization");
            int residentIndex = BuildingLookup.GetResidentIndex(manifest, residentName);
            int sourceHourIndex = ScheduleUtility.GetHourIndex(time, sourceClockHour);
            int destHourIndex = ScheduleUtility.GetHourIndex(time, destClockHour);

            ref ResidentTimeline timeline = ref scheduler.DefaultResidentTimelines[residentIndex];

            ScheduleUtility.CopyResidentBlock(ref timeline, sourceHourIndex, destHourIndex);
        }

        /// <summary>
        /// Copies a resident's schedule over a specific range.
        /// </summary>
        [LeafMember("ScheduleResidentCopyRange")]
        static public void ScheduleResidentCopyRange(int startClockHour, int endClockHour, StringHash32 residentName) {
            Find.State(out TimeScheduler time, out ActorScheduler scheduler, out BuildingManifest manifest);
            Assert.True(time.IsInitializing, "Default schedules can only be adjusted within a level initialization");
            int residentIndex = BuildingLookup.GetResidentIndex(manifest, residentName);
            int startHourIndex = ScheduleUtility.GetHourIndex(time, startClockHour);
            int endHourIndex = ScheduleUtility.GetHourIndex(time, endClockHour);

            ref ResidentTimeline timeline = ref scheduler.DefaultResidentTimelines[residentIndex];

            ScheduleUtility.CopyResidentBlockRange(ref timeline, startHourIndex, endHourIndex - startHourIndex + 1);
        }

        /// <summary>
        /// Overrides a resident's location for the given hour.
        /// </summary>
        [LeafMember("OverrideResidentLocation")]
        static public void OverrideResidentLocation(int clockHour, StringHash32 residentName, StringHash32 roomName, StringHash32 postName) {
            Find.State(out TimeScheduler time, out ActorScheduler scheduler, out BuildingManifest manifest, out ScheduleState state);
            Assert.True(!time.IsInitializing && state.Phase == TimePhase.Mitigation, "Schedule overrides can only happen during mitigation");
            int residentIndex = BuildingLookup.GetResidentIndex(manifest, residentName);
            int hourIndex = ScheduleUtility.GetHourIndex(time, clockHour);

            ref ResidentTimeline timeline = ref scheduler.LiveResidentTimelines[residentIndex];
            ref ResidentTimeBlock block = ref timeline[hourIndex];

            ScheduleUtility.DefineResidentBlockLocation(ref block, roomName, postName, manifest);
            block.Flags |= ResidentTimeBlockFlags.IsOverride;
        }

        /// <summary>
        /// Overrides a resident's symptom for the given hour.
        /// </summary>
        [LeafMember("OverrideResidentSymptom")]
        static public void OverrideResidentSymptom(int clockHour, StringHash32 residentName, StringSlice symptomName) {
            Find.State(out TimeScheduler time, out ActorScheduler scheduler, out BuildingManifest manifest, out ScheduleState state);
            Assert.True(!time.IsInitializing && state.Phase == TimePhase.Mitigation, "Schedule overrides can only happen during mitigation");
            int residentIndex = BuildingLookup.GetResidentIndex(manifest, residentName);
            int hourIndex = ScheduleUtility.GetHourIndex(time, clockHour);

            ref ResidentTimeline timeline = ref scheduler.LiveResidentTimelines[residentIndex];
            ref ResidentTimeBlock block = ref timeline[hourIndex];

            ScheduleUtility.DefineResidentBlockSymptom(ref block, Symptom.Parse(symptomName));
            block.Flags |= ResidentTimeBlockFlags.IsOverride;
        }

        /// <summary>
        /// Overrides a resident's script node for the given hour.
        /// </summary>
        [LeafMember("OverrideResidentScript")]
        static public void OverrideResidentScript(int clockHour, StringHash32 residentName, StringHash32 nodeId) {
            Find.State(out TimeScheduler time, out ActorScheduler scheduler, out BuildingManifest manifest, out ScheduleState state);
            Assert.True(!time.IsInitializing && state.Phase == TimePhase.Mitigation, "Schedule overrides can only happen during mitigation");
            int residentIndex = BuildingLookup.GetResidentIndex(manifest, residentName);
            int hourIndex = ScheduleUtility.GetHourIndex(time, clockHour);

            ref ResidentTimeline timeline = ref scheduler.LiveResidentTimelines[residentIndex];
            ref ResidentTimeBlock block = ref timeline[hourIndex];

            ScheduleUtility.DefineResidentBlockScriptNode(ref block, nodeId);
            block.Flags |= ResidentTimeBlockFlags.IsOverride;
        }

        /// <summary>
        /// Overrides a resident's schedule by copying from a different time.
        /// </summary>
        [LeafMember("OverrideResidentCopy")]
        static public void OverrideResidentCopy(int sourceClockHour, int destClockHour, StringHash32 residentName) {
            Find.State(out TimeScheduler time, out ActorScheduler scheduler, out BuildingManifest manifest, out ScheduleState state);
            Assert.True(!time.IsInitializing && state.Phase == TimePhase.Mitigation, "Schedule overrides can only happen during mitigation");
            int residentIndex = BuildingLookup.GetResidentIndex(manifest, residentName);
            int sourceHourIndex = ScheduleUtility.GetHourIndex(time, sourceClockHour);
            int destHourIndex = ScheduleUtility.GetHourIndex(time, destClockHour);

            ref ResidentTimeline timeline = ref scheduler.LiveResidentTimelines[residentIndex];

            ScheduleUtility.CopyResidentBlock(ref timeline, sourceHourIndex, destHourIndex);
            timeline[destHourIndex].Flags |= ResidentTimeBlockFlags.IsOverride;
        }

        #endregion // Residents
    }
}