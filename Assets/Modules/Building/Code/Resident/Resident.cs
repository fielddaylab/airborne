using BeauUtil;
using FieldDay.Components;
using FieldDay.Scripting;
using ScriptableBake;
using UnityEngine;

namespace Airborne.Building {
    public sealed class Resident : ScriptActorComponent, IBaked {
        [HideInInspector] public StringHash32 Id;
        [HideInInspector] public int Index;

#if UNITY_EDITOR

        int IBaked.Order { get { return -1003; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            Id = StringHash32.First(ScriptUtility.ActorId(this), name);
            return true;
        }

#endif // UNITY_EDITOR
    }
}