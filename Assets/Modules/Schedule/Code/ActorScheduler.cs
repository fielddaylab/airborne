using System.Runtime.InteropServices;
using Airborne.Building;
using Airborne.Pollutants;
using BeauUtil;
using FieldDay.SharedState;

namespace Airborne.Schedule {
    public sealed class ActorScheduler : SharedStateComponent {
        // Default timelines. Set once at level boot.
        public ResidentTimeline[] DefaultResidentTimelines;
        public FeatureTimeline[] DefaultFeatureTimelines;
        public PassageTimeline[] DefaultPassageTimelines;

        // Active timelines. Copied from default on loop start.
        public ResidentTimeline[] ActiveResidentTimelines;
        public FeatureTimeline[] ActiveFeatureTimelines;
        public PassageTimeline[] ActivePassageTimelines;
    }

    /// <summary>
    /// Resident schedule.
    /// </summary>
    public unsafe struct ResidentTimeline {
        public fixed ulong PackedTimeBlocks[BuildingConsts.MaxHours];

        public ref ResidentTimeBlock this[int index] {
            get { return ref Unsafe.FastCast<ulong, ResidentTimeBlock>(ref PackedTimeBlocks[index]); }
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

    public enum ResidentTimeBlockFlags : byte {
        IsDefined = 0x01,
        Hidden = 0x02,
    }

    public struct FeatureTimeline {
        private const byte StaticCheck_BuildingConstsMaxHoursIs8 = (BuildingConsts.MaxHours <= 8 ? 0 : -1);

        // note: this can only be a byte as long as BuildingConsts.MaxHours == 8
        // if expanded, please update the type accordingly
        public byte EnabledMask;
    }

    public struct PassageTimeline {
        private const byte StaticCheck_BuildingConstsMaxHoursIs8 = (BuildingConsts.MaxHours <= 8 ? 0 : -1);

        // note: this can only be a byte as long as BuildingConsts.MaxHours == 8
        // if expanded, please update the type accordingly
        public byte EnabledMask;
    }

    static public partial class ScheduleScripting {

    }
}