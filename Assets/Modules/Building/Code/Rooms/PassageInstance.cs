using System;
using Airborne.Pollutants;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.Components;
using FieldDay.Scripting;
using ScriptableBake;
using UnityEngine;

namespace Airborne.Building {
    public sealed class PassageInstance : ScriptActorComponent, IBaked {
        [HideInInspector] public StringHash32 Id;
        [HideInInspector] public int Index;

        public RoomInstance Source;
        public RoomInstance Target;
        [Range(1, 24)] public int Rank = 1;
        public bool IsBidirectional = true;

#if UNITY_EDITOR

        int IBaked.Order { get { return -999; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            Id = StringHash32.First(ScriptUtility.ActorId(this), name);
            return true;
        }

#endif // UNITY_EDITOR
    }

    static public partial class BuildingLookup {

    }
}