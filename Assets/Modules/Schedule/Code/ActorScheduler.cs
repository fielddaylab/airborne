using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Airborne.Building;
using Airborne.Pollutants;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.SharedState;

namespace Airborne.Schedule {
    public sealed class ActorScheduler : SharedStateComponent {
        // Default timelines. Set once at level boot.
        [NonSerialized] public ResidentTimeline[] DefaultResidentTimelines;
        [NonSerialized] public FeatureTimeline[] DefaultFeatureTimelines;
        [NonSerialized] public PassageTimeline[] DefaultPassageTimelines;
        [NonSerialized] public ulong TimelineHash;

        // Active timelines. Copied from default on loop start, modified from script.
        [NonSerialized] public ResidentTimeline[] LiveResidentTimelines;
        [NonSerialized] public FeatureTimeline[] LiveFeatureTimelines;
        [NonSerialized] public PassageTimeline[] LivePassageTimelines;
    }

    /// <summary>
    /// Resident schedule.
    /// </summary>
    public unsafe struct ResidentTimeline {
        public fixed ulong PackedTimeBlocks[BuildingConsts.MaxHours];

        public ref ResidentTimeBlock this[int index] {
            get {
                Assert.True(index >= 0 && index < BuildingConsts.MaxHours, "Index out of range");
                return ref Unsafe.FastCast<ulong, ResidentTimeBlock>(ref PackedTimeBlocks[index]);
            }
        }
    }

    /// <summary>
    /// Resident information per time block.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct ResidentTimeBlock {
        // 4 bytes
        public byte RoomIndex;
        public byte PostIndex;
        public Symptom Symptom;
        public ResidentTimeBlockFlags Flags;

        // 4 bytes
        public StringHash32 ScriptNode;
    }

    [Flags]
    public enum ResidentTimeBlockFlags : byte {
        IsDefined = 0x01,
        Hidden = 0x02,
        IsOverride = 0x04
    }

    public struct FeatureTimeline {
        // In the absence of compile-time asserts, this will suffice to alert us.
        private const byte StaticCheck_BuildingConstsMaxHoursIs8 = (BuildingConsts.MaxHours <= 8 ? 0 : -1);

        // note: this can only be a byte as long as BuildingConsts.MaxHours <= 8
        // if expanded, please update the type accordingly
        public byte EnabledMask;
    }

    public struct PassageTimeline {
        // In the absence of compile-time asserts, this will suffice to alert us.
        private const byte StaticCheck_BuildingConstsMaxHoursIs8 = (BuildingConsts.MaxHours <= 8 ? 0 : -1);

        // note: this can only be a byte as long as BuildingConsts.MaxHours <= 8
        // if expanded, please update the type accordingly
        public byte EnabledMask;
    }

    static public partial class ScheduleUtility {
        #region Timeline

        /// <summary>
        /// Allocates data for timelines for the given object counts.
        /// </summary>
        static public void AllocateTimelineBuffers(ActorScheduler scheduler, int residentCount, int featureCount, int passageCount) {
            Assert.NotNullOrDestroyed(scheduler);
            Assert.True(scheduler.DefaultResidentTimelines == null, "Scheduler timeline arrays already created");

            scheduler.DefaultResidentTimelines = new ResidentTimeline[residentCount];
            scheduler.LiveResidentTimelines = new ResidentTimeline[residentCount];

            scheduler.DefaultFeatureTimelines = new FeatureTimeline[featureCount];
            scheduler.LiveFeatureTimelines = new FeatureTimeline[featureCount];

            scheduler.DefaultPassageTimelines = new PassageTimeline[passageCount];
            scheduler.LivePassageTimelines = new PassageTimeline[passageCount];
        }

        /// <summary>
        /// Patches holes in the timeline.
        /// </summary>
        static public void PatchTimelineHoles(ActorScheduler scheduler, int hourCount) {
            Assert.NotNullOrDestroyed(scheduler);
            Assert.True(hourCount <= BuildingConsts.MaxHours);

            for(int residentIdx = 0; residentIdx < scheduler.DefaultResidentTimelines.Length; residentIdx++) {
                ref ResidentTimeline timeline = ref scheduler.DefaultResidentTimelines[residentIdx];
                for(int hour = 0; hour < hourCount; hour++) {
                    if (DefineResidentBlock(ref timeline[hour])) {
                        Log.Warn("[ActorScheduler] Actor index {0} was not defined for hour index {1}", residentIdx, hour);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the 
        /// </summary>
        static public bool ApplyTimelineHash(ActorScheduler scheduler) {
            Assert.NotNullOrDestroyed(scheduler);

            ulong prevHash = scheduler.TimelineHash;
            ulong nextHash = ComputeTimelineHash(scheduler);
            if (nextHash != prevHash) {
                scheduler.TimelineHash = nextHash;
                return true;
            }
            return false;
        }

        static private unsafe ulong ComputeTimelineHash(ActorScheduler scheduler) {
            int residentByteCount = sizeof(ResidentTimeline) * scheduler.DefaultResidentTimelines.Length;
            int featureByteCount = sizeof(FeatureTimeline) * scheduler.DefaultFeatureTimelines.Length;
            int passageByteCount = sizeof(PassageTimeline) * scheduler.DefaultPassageTimelines.Length;

            int maxByteCount = Math.Max(passageByteCount, Math.Max(residentByteCount, featureByteCount));

            ulong* alignedHashBuffer = stackalloc ulong[(maxByteCount + 7) / 8];

            ulong hash;
            fixed(ResidentTimeline* resPtr = scheduler.DefaultResidentTimelines) {
                Unsafe.FastCopy(resPtr, residentByteCount, alignedHashBuffer);
                hash = Unsafe.Hash64(alignedHashBuffer, residentByteCount);
            }
            fixed (FeatureTimeline* featPtr = scheduler.DefaultFeatureTimelines) {
                Unsafe.FastCopy(featPtr, featureByteCount, alignedHashBuffer);
                hash = Unsafe.CombineHash64(hash, alignedHashBuffer, featureByteCount);
            }
            fixed (PassageTimeline* passPtr = scheduler.DefaultPassageTimelines) {
                Unsafe.FastCopy(passPtr, passageByteCount, alignedHashBuffer);
                hash = Unsafe.CombineHash64(hash, alignedHashBuffer, passageByteCount);
            }

            return hash;
        }

        /// <summary>
        /// Clears all timeline data.
        /// Call before defining timelines.
        /// </summary>
        static public unsafe void ClearTimelines(ActorScheduler scheduler) {
            Assert.NotNullOrDestroyed(scheduler);
            Assert.NotNull(scheduler.DefaultResidentTimelines, "Scheduler timeline not initialized");

            Unsafe.Clear(scheduler.DefaultResidentTimelines);
            Unsafe.Clear(scheduler.DefaultFeatureTimelines);
            Unsafe.Clear(scheduler.DefaultPassageTimelines);
        }

        /// <summary>
        /// Resets live timelines to their default schedule.
        /// </summary>
        static public unsafe void MakeTimelineLive(ActorScheduler scheduler) {
            Assert.NotNullOrDestroyed(scheduler);
            Assert.NotNull(scheduler.DefaultResidentTimelines, "Scheduler timeline not initialized");

            fixed(ResidentTimeline* defaultSchedule = scheduler.DefaultResidentTimelines)
            fixed(ResidentTimeline* liveSchedule = scheduler.LiveResidentTimelines) {
                Unsafe.FastCopyArray(defaultSchedule, scheduler.DefaultResidentTimelines.Length, liveSchedule);
            }

            fixed (FeatureTimeline* defaultSchedule = scheduler.DefaultFeatureTimelines)
            fixed (FeatureTimeline* liveSchedule = scheduler.LiveFeatureTimelines) {
                Unsafe.FastCopyArray(defaultSchedule, scheduler.DefaultFeatureTimelines.Length, liveSchedule);
            }

            fixed (PassageTimeline* defaultSchedule = scheduler.DefaultPassageTimelines)
            fixed (PassageTimeline* liveSchedule = scheduler.DefaultPassageTimelines) {
                Unsafe.FastCopyArray(defaultSchedule, scheduler.DefaultPassageTimelines.Length, liveSchedule);
            }
        }

        #endregion // Timeline

        #region Resident Info

        /// <summary>
        /// Ensures a resident block is defined.
        /// </summary>
        static public bool DefineResidentBlock(ref ResidentTimeBlock block) {
            if ((block.Flags & ResidentTimeBlockFlags.IsDefined) == 0) {
                block.RoomIndex = Simulator.NullIndex;
                block.PostIndex = Simulator.NullIndex;
                block.Symptom = default;
                block.Flags = ResidentTimeBlockFlags.IsDefined | ResidentTimeBlockFlags.Hidden;
                block.ScriptNode = default;
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Defines a resident's room and post for a resident block.
        /// </summary>
        static public void DefineResidentBlockLocation(ref ResidentTimeBlock timeBlock, StringHash32 roomId, StringHash32 postId, BuildingManifest manifest) {
            DefineResidentBlock(ref timeBlock);

            if (roomId.IsEmpty) {
                timeBlock.RoomIndex = Simulator.NullIndex;
                timeBlock.PostIndex = Simulator.NullIndex;
                timeBlock.Flags |= ~ResidentTimeBlockFlags.Hidden;
            } else {
                int roomIndex = BuildingLookup.GetRoomIndex(manifest, roomId);
                int postIndex = BuildingLookup.GetPostIndex(BuildingLookup.GetRoomInstance(manifest, roomIndex), postId);

                timeBlock.RoomIndex = (byte) roomIndex;
                timeBlock.PostIndex = (byte) postIndex;
                timeBlock.Flags &= ~ResidentTimeBlockFlags.Hidden;
            }
        }

        /// <summary>
        /// Defines a resident's symptoms for a resident block.
        /// </summary>
        static public void DefineResidentBlockSymptom(ref ResidentTimeBlock timeBlock, Symptom symptom) {
            DefineResidentBlock(ref timeBlock);
            timeBlock.Symptom = symptom;
        }

        /// <summary>
        /// Defines a resident's script node to run for a resident block.
        /// </summary>
        static public void DefineResidentBlockScriptNode(ref ResidentTimeBlock timeBlock, StringHash32 nodeId) {
            DefineResidentBlock(ref timeBlock);
            timeBlock.ScriptNode = nodeId;
        }

        /// <summary>
        /// Copies a resident's time blocks over the given range.
        /// </summary>
        static public void CopyResidentBlock(ref ResidentTimeline timeline, int srcIndex, int dstIndex) {
            Assert.True(srcIndex >= 0 && srcIndex < BuildingConsts.MaxHours, "Source index {0} out of range", srcIndex);
            Assert.True(dstIndex >= 0 && srcIndex < BuildingConsts.MaxHours, "Destination index {0} out of range", srcIndex);

            ref ResidentTimeBlock timeBlock = ref timeline[srcIndex];
            if (DefineResidentBlock(ref timeBlock)) {
                Log.Warn("[ActorScheduler] Resident block at hour index {0} was not initially configured", srcIndex);
            }

            if (srcIndex != dstIndex) {
                timeline[dstIndex] = timeBlock;
            }
        }

        /// <summary>
        /// Copies a resident's time blocks over the given range.
        /// </summary>
        static public void CopyResidentBlockRange(ref ResidentTimeline timeline, int startIndex, int endIndex) {
            Assert.True(startIndex >= 0 && startIndex < BuildingConsts.MaxHours, "Start index {0} out of range", startIndex);
            Assert.True(endIndex >= startIndex && endIndex < BuildingConsts.MaxHours, "End index {0} out of range", startIndex);

            ref ResidentTimeBlock timeBlock = ref timeline[startIndex];
            if (DefineResidentBlock(ref timeBlock)) {
                Log.Warn("[ActorScheduler] Resident block at hour index {0} was not initially configured", startIndex);
            }

            for(int i = startIndex + 1; i <= endIndex; i++) {
                timeline[i] = timeBlock;
            }
        }

        #endregion // Resident Info
    }
}