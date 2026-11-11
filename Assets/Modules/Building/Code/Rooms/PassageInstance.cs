using System;
using Airborne.Pollutants;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.Components;
using ScriptableBake;
using UnityEngine;

namespace Airborne.Building {
    public sealed class PassageInstance : BatchedComponent, IBaked {
        [HideInInspector] public StringHash32 Id;
        [HideInInspector] public int Index;

        public RoomInstance Source;
        public RoomInstance Target;
        public bool IsBidirectional = true;

#if UNITY_EDITOR

        int IBaked.Order { get { return -1001; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            Id = name;
            return true;
        }

#endif // UNITY_EDITOR
    }

    static public partial class BuildingLookup {

    }
}