using System;
using BeauUtil;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.UI;
using UnityEngine;

namespace Airborne.Building {
    public sealed class PlayerController : BatchedComponent {
        public WorldInputLayer Input;
        public Movable Mover;
        public Collider InteractionVolume;

        [NonSerialized] public Interactable ClosestInteractable;
    }
}