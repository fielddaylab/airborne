using System;
using System.Collections;
using System.Collections.Generic;
using FieldDay.HID;
using FieldDay.SharedState;
using UnityEngine;

namespace Airborne {
    public class InputState : SharedStateComponent {
        [NonSerialized] public Vector2 NavAnalogRaw;
        [NonSerialized] public Vector2 NavAnalogNormalized;
        [NonSerialized] public DigitalControlStates<NavigationKeys> NavKeys;
        [NonSerialized] public DigitalControlStates<ActionKeys> ActionKeys;
    }

    [Flags]
    public enum NavigationKeys : byte {
        None = 0x0,

        Right = 0x1,
        Up = 0x2,
        Left = 0x04,
        Down = 0x80
    }

    [Flags]
    public enum ActionKeys : byte {
        None = 0x0,

        Menu = 0x1,
        Interact = 0x2,
        Back = 0x4
    }
}
