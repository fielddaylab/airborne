using System;
using BeauUtil;
using FieldDay.Components;
using ScriptableBake;
using UnityEngine;

namespace Airborne.Building {
    /// <summary>
    /// Named location within a room.
    /// Indicates what action should be taken by residents when posted there.
    /// </summary>
    public sealed class ActionPost : BatchedComponent, IBaked {
        [HideInInspector] public StringHash32 Id;
        [HideInInspector] public int Index;

        // TODO: facing, action

#if UNITY_EDITOR

        int IBaked.Order { get { return -1001; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            Id = name;
            return true;
        }

#endif // UNITY_EDITOR
    }
}