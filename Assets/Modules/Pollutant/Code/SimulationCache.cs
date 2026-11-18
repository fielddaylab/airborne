#if UNITY_EDITOR || DEVELOPMENT || DEVELOPMENT_BUILD
#define SIM_VALIDATE_DETERMINISM
#endif // UNITY_EDITOR || DEVELOPMENT || DEVELOPMENT_BUILD

using System;
using System.Text;
using Airborne.Building;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.SharedState;

namespace Airborne.Pollutants {
    /// <summary>
    /// Cache of pollutant levels per-room per-hour.
    /// Used to validate against player meter recordings
    /// and to validate against simulation results for determinism.
    /// </summary>
    public sealed class SimulationCache : SharedStateComponent {
        [NonSerialized] public RecordedRoomLevels[] RoomConditions;
    }

    /// <summary>
    /// Compressed pollutant levels over a day for a single room.
    /// </summary>
    public unsafe struct RecordedRoomLevels {
        public unsafe fixed ushort PackedValues[BuildingConsts.MaxHours];

        public ref CompressedPollutantLevels this[int index] {
            get {
                Assert.True(index >= 0 && index < BuildingConsts.MaxHours, "hour index out of range");
                return ref Unsafe.FastCast<ushort, CompressedPollutantLevels>(ref PackedValues[index]);
            }
        }
    }

    static public partial class SimulatorUtility {
        static public void AllocateCache(SimulationCache cache, int roomCount) {
            Assert.NotNullOrDestroyed(cache);
            Assert.True(cache.RoomConditions == null, "Cache already initialized");

            cache.RoomConditions = new RecordedRoomLevels[roomCount];
        }

        static public void RecordToCache(SimulationCache cache, Simulator simulator, int hourIndex) {
            Assert.NotNullOrDestroyed(cache);
            Assert.NotNullOrDestroyed(simulator);

            int roomCount = simulator.RoomConditions.Current.Length;
            for(int roomIndex = 0; roomIndex < roomCount; roomIndex++) {
                ref CompressedPollutantLevels compressed = ref cache.RoomConditions[roomIndex][hourIndex];
                compressed = SimulatorUtility.CompressLevels(simulator.RoomConditions.Current[roomIndex]);
            }
        }

#if !SIM_VALIDATE_DETERMINISM
        [System.Diagnostics.Conditional("__NEVER")]
#endif // !SIM_VALIDATE_DETERMINISM
        static public void ValidateWithCache(SimulationCache cache, Simulator simulator, int hourIndex) {
#if SIM_VALIDATE_DETERMINISM
            int roomCount = simulator.RoomConditions.Current.Length;
            BitSet32 erroredRooms = default;
            for (int roomIndex = 0; roomIndex < roomCount; roomIndex++) {
                PollutantLevels decompressedCached = SimulatorUtility.DecompressLevels(cache.RoomConditions[roomIndex][hourIndex]);
                PollutantLevels current = simulator.RoomConditions.Current[roomIndex];
                erroredRooms.Set(roomIndex, !SimulatorUtility.AreLevelsEqual(current, decompressedCached));
            }

            if (!erroredRooms.IsEmpty) {
                StringBuilder sb = new StringBuilder(2048);
                sb.Append("Simulation has different results at hour index ").AppendNoAlloc(hourIndex);
                foreach(var roomIndex in erroredRooms) {
                    PollutantLevels decompressedCached = SimulatorUtility.DecompressLevels(cache.RoomConditions[roomIndex][hourIndex]);
                    PollutantLevels current = simulator.RoomConditions.Current[roomIndex];
                    sb.Append("\nRoom ").AppendNoAlloc(roomIndex);
                    for(int p = 0; p < simulator.PollutantTypeCount; p++) {
                        sb.Append(" | ").AppendNoAlloc(current[p]).Append(" / expected ").AppendNoAlloc(decompressedCached[p]);
                    }
                }

                Log.Error(sb.ToString());
                Assert.Fail("Simulation determinism failure - {0} rooms did not line up with cached initial run. See log for details.", erroredRooms.Count);
            }
#endif // SIM_VALIDATE_DETERMINISM
        }
    }
}