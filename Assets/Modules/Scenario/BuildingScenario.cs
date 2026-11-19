using System;
using Airborne.Building;
using Airborne.Inventory;
using Airborne.Pollutants;
using Airborne.Schedule;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.Assets;
using FieldDay.Debugging;
using FieldDay.Scripting;
using FieldDay.SharedState;
using Leaf;
using ScriptableBake;
using UnityEngine;

namespace Airborne.Schedule {
    /// <summary>
    /// Description of scenario-specific data (i.e. pollutants, meter count)
    /// </summary>
    public sealed class BuildingScenario : SharedStateComponent, IBaked {
        [AssetName(typeof(PollutantDefinition))] public StringHash32[] Pollutants;
        [Range(0, BuildingConsts.MaxMeters)] public int MaxMeters = 4;

#if UNITY_EDITOR

        int IBaked.Order { get { return -10000; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            if (Pollutants.Length > 4) {
                Array.Resize(ref Pollutants, 4);
                return true;
            }

            return false;
        }

#endif // UNITY_EDITOR
    }
}