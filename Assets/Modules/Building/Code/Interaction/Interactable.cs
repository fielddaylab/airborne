using System;
using BeauUtil;
using FieldDay.Scripting;

namespace Airborne.Building {
    public sealed class Interactable : ScriptActorComponent {
        public InteractableCallback CustomCallback;
        public InteractablePredicate CustomPredicate;

        public bool HasInteractions = true;
    }

    public delegate void InteractableCallback(Interactable interactable, InteractRequest request);
    public delegate bool InteractablePredicate(Interactable interactable, ScriptActor source);
}