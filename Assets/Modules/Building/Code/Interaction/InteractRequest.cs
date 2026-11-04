using BeauUtil;
using FieldDay.Analytics;
using FieldDay.Scripting;
using Leaf.Runtime;

namespace Airborne.Building {
    /// <summary>
    /// Requested interaction.
    /// </summary>
    public struct InteractRequest {
        public InteractVerb Verb;
        public StringHash32 SourceId;
        public StringHash32 TargetId;
        public StringHash32 ContextId;
    }

    public enum InteractVerb : byte {
        None = 0,

        Look,
        Listen,
        Smell,
        Talk,
        Switch,
        Use,
        Give,
    }

    static public partial class InteractionUtility {
        static public LeafThreadHandle ProcessInteraction(InteractRequest request) {
            ScriptActor targetActor = request.TargetId.IsEmpty ? null : ScriptUtility.FindActor(request.TargetId);
            ScriptActor sourceActor = request.SourceId.IsEmpty ? null : ScriptUtility.FindActor(request.SourceId);

            if (targetActor && targetActor.TryGetComponent(out Interactable interactable)) {
                interactable.CustomCallback?.Invoke(interactable, request);
            }

            using (var t = TempVarTable.Alloc()) {
                t.Set("source", request.SourceId);
                t.Set("context", request.ContextId);
                t.Set("target", request.TargetId);
                t.Set("verb", s_VerbNames[(int) request.Verb]);

                if (targetActor) {
                    t.Set("targetClass", targetActor.ClassName);
                }

                if (sourceActor) {
                    t.Set("sourceClass", sourceActor.ClassName);
                }

                return ScriptUtility.Trigger(s_VerbTriggers[(int) request.Verb], request.TargetId, targetActor, t);
            }
        }

        static private readonly StringHash32[] s_VerbTriggers = new StringHash32[] {
            "",

            "OnLook",
            "OnListen",
            "OnSmell",
            "OnTalk",
            "OnSwitch",
            "OnUse",
            "OnGive"
        };

        static private readonly StringHash32[] s_VerbNames = new StringHash32[] {
            "",

            "Look",
            "Listen",
            "Smell",
            "Talk",
            "Switch",
            "Use",
            "Give"
        };
    }
}