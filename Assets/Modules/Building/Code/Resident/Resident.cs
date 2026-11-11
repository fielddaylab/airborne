using BeauUtil;
using FieldDay.Components;
using ScriptableBake;
using UnityEngine;

namespace Airborne.Building {
    public sealed class Resident : BatchedComponent, IBaked {
        [HideInInspector] public StringHash32 Id;
        [HideInInspector] public int Index;

#if UNITY_EDITOR

        int IBaked.Order { get { return -1003; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            Id = name;
            return true;
        }

#endif // UNITY_EDITOR
    }
}