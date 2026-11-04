using BeauUtil;
using FieldDay.Scripting;

namespace Airborne.Building {
    public sealed class Interactable : ScriptActorComponent {
        public InteractableCallback CustomCallback;
    }

    public delegate void InteractableCallback(Interactable interactable, InteractRequest request);
}