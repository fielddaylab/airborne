using System;
using System.Runtime.InteropServices;
using Airborne.Building;
using BeauUtil.Debugger;
using FieldDay.Assets;
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

    public unsafe struct PollutantLevels {
        public fixed byte Levels[BuildingConsts.MaxPollutants];

        public ref byte this[int index] {
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
}