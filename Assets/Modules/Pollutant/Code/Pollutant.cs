using System;
using System.Runtime.InteropServices;
using Airborne.Building;
using FieldDay.Assets;
using FieldDay.Data;
using UnityEngine;

namespace Airborne.Pollutants {
    [CreateAssetMenu(menuName = "Airborne/Pollutant Definition")]
    public sealed class PollutantDefinition : NamedAsset {

    }

    public unsafe struct PollutantLevels {
        public fixed byte Levels[BuildingConsts.MaxPollutants];
    }
}