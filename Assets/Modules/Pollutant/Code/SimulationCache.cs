// #define SIM_DIFFUSION_DEBUG

using System;
using System.Runtime.InteropServices;
using System.Text;
using Airborne.Building;
using BeauPools;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.Collections;
using FieldDay.SharedState;

namespace Airborne.Pollutants {
    public sealed class SimulationCache : SharedStateComponent {
        [NonSerialized] public RecordedRoomLevels[] RoomConditions;
    }

    public unsafe struct RecordedRoomLevels {
        public unsafe fixed ushort PackedValues[BuildingConsts.MaxHours];

        public ref CompressedPollutantLevels this[int index] {
            get {
                Assert.True(index >= 0 && index < BuildingConsts.MaxHours, "hour index out of range");
                return ref Unsafe.FastCast<ushort, CompressedPollutantLevels>(ref PackedValues[index]);
            }
        }
    }
}