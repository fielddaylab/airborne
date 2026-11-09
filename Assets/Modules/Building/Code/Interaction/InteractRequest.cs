using BeauUtil;
using FieldDay;
using FieldDay.Analytics;
using FieldDay.Scripting;
using Leaf.Runtime;

namespace Airborne.Building {
    /// <summary>
    /// Interaction data packet.
    /// Verb, involved actors, and context.
    /// </summary>
    public struct InteractRequest {
        public StringHash32 SourceId;
        public StringHash32 TargetId;
        public StringHash32 ContextId;
    }

    static public partial class InteractionUtility {
        #region Requests

        static public void RequestInteraction(InteractRequest request) {
            Find.State(out InteractionArbiter arbiter);
            arbiter.Requests.Write(request);
        }

        static public void RequestInteractionByActor(ScriptActor source, ScriptActor target, StringHash32 contextId) {
            InteractRequest request;
            request.SourceId = source?.Id ?? default;
            request.TargetId = target?.Id ?? default;
            request.ContextId = contextId;
            RequestInteraction(request);
        }

        static public void RequestInteractionById(StringHash32 sourceId, StringHash32 targetId, StringHash32 contextId) {
            InteractRequest request;
            request.SourceId = sourceId;
            request.TargetId = targetId;
            request.ContextId = contextId;
            RequestInteraction(request);
        }

        #endregion // Requests

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

                if (targetActor) {
                    t.Set("targetClass", targetActor.ClassName);
                }

                if (sourceActor) {
                    t.Set("sourceClass", sourceActor.ClassName);
                }

                return ScriptUtility.Trigger("OnInteract", request.TargetId, targetActor, t);
            }
        }
    }
}