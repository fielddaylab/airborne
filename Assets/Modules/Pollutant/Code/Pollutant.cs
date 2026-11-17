using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Airborne.Building;
using BeauUtil.Debugger;
using FieldDay.Assets;
using FieldDay.Collections;
using FieldDay.Data;
using UnityEngine;

namespace Airborne.Pollutants {
    [CreateAssetMenu(menuName = "Airborne/Pollutant Definition")]
    public sealed class PollutantDefinition : NamedAsset {
        public PollutantCategory Category;
        public SymptomMask Symptoms;

        [Header("Display")]
        public string DisplayName;
        public string DisplayAbbreviation;
    }

    /// <summary>
    /// Pollutant levels, 8-bit unsigned.
    /// </summary>
    public unsafe struct PollutantLevels {
        public fixed byte Levels[BuildingConsts.MaxPollutants];

        public ref byte this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                Assert.True(index >= 0 && index < BuildingConsts.MaxPollutants, "Index out of range");
                return ref Levels[index];
            }
        }
    }

    /// <summary>
    /// Pollutant levels, 4-bit unsigned.
    /// </summary>
    public unsafe struct CompressedPollutantLevels {
        private const byte StaticCheck_MaxPollutantFitsIn4Bits = (SimulationConsts.MaxPollutantLevel <= 15 ? 0 : -1);
        private const byte StaticCheck_MaxPollutantCountIs4 = (BuildingConsts.MaxPollutants <= 4 ? 0 : -1);

        public ushort PackedValues;

        public byte this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get {
                Assert.True(index >= 0 && index < BuildingConsts.MaxPollutants, "pollutant index out of range");
                return (byte) BitPack.Unpack16(PackedValues, 4, index * 4);
            }
            set {
                Assert.True(index >= 0 && index < BuildingConsts.MaxPollutants, "pollutant index out of range");
                Assert.True(value <= SimulationConsts.MaxPollutantLevel, "Value {0} cannot fit into 4 bits", value);
                PackedValues = BitPack.Combine(BitPack.Clear16(PackedValues, 4, index * 4), BitPack.Pack16(value, 4, index * 4));
            }
        }
    }

    /// <summary>
    /// Pollutant changes, 8-bit signed.
    /// </summary>
    public unsafe struct PollutantAdjustments {
        public fixed sbyte Levels[BuildingConsts.MaxPollutants];

        public ref sbyte this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                Assert.True(index >= 0 && index < BuildingConsts.MaxPollutants, "Index out of range");
                return ref Levels[index];
            }
        }
    }

    public enum PollutantCategory : byte {
        Combustion,
        Particulate,
        VolatileOrganics
    }

    static public partial class SimulatorUtility {
        /// <summary>
        /// Packs the given pollutant levels into a half-size container.
        /// </summary>
        static public unsafe CompressedPollutantLevels CompressLevels(PollutantLevels levels) {
            ushort packed = 0;
            for(int i = 0; i < BuildingConsts.MaxPollutants; i++) {
                byte level = levels.Levels[i];
                packed = BitPack.Combine(packed, BitPack.Pack16(level, 4, i * 4));
            }

            CompressedPollutantLevels compressed;
            compressed.PackedValues = packed;
            return compressed;
        }
        
        /// <summary>
        /// Unpacks the given compressed pollutant levels into full-size containers.
        /// </summary>
        static public unsafe PollutantLevels DecompressLevels(CompressedPollutantLevels levels) {
            PollutantLevels uncompressed;
            for(int i = 0; i < BuildingConsts.MaxPollutants; i++) {
                uncompressed.Levels[i] = levels[i];
            }
            return uncompressed;
        }

        private const byte StaticAssert_MaxPollutantsEquals4 = BuildingConsts.MaxPollutants == 4 ? 0 : -1;

        /// <summary>
        /// Checks if the two sets of pollutant levels are equal.
        /// </summary>
        static public unsafe bool AreLevelsEqual(PollutantLevels a, PollutantLevels b) {
            
            return *(uint*) (&a) == *(uint*) (&b);

            //bool areEqual = true;
            //for(int i = 0; i < BuildingConsts.MaxPollutants; i++) {
            //    areEqual &= a.Levels[i] == b.Levels[i];
            //}
            //return areEqual;
        }
    }
}