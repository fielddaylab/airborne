using BeauUtil;
using FieldDay;
using FieldDay.Analytics;
using FieldDay.Collections;
using FieldDay.Scripting;
using FieldDay.SharedState;
using FieldDay.Systems;
using Leaf.Runtime;

namespace Airborne.Building {
    /// <summary>
    /// Requested interaction.
    /// </summary>
    public sealed class InteractionArbiterSystem : SystemComponent {
        public override unsafe void RegisterSystems(ref SystemRegistrationTable ecs) {
            ecs.Register(&ProcessRequests,
                new SysUpdate(GameLoopPhase.Update, 1000),
                new SysPermissions()
                    .ReadWriteShared<InteractionArbiter>()
                    .ReadWrite<ScriptActor>()
            );
        }

        static private void ProcessRequests(float dt) {
            Find.State(out InteractionArbiter arbiter);

            while(arbiter.Requests.TryRead(out InteractRequest request)) {
                InteractionUtility.ProcessInteraction(request);
            }
        }
    }
}