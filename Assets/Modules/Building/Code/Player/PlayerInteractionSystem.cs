using System;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Collections;
using FieldDay.Components;
using FieldDay.Physics;
using FieldDay.SharedState;
using FieldDay.Systems;
using FieldDay.UI;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Airborne.Building {
    public sealed class PlayerInteractionSystem : SystemComponent {
        private const int MaxEvaluatedInteractables = 8;

        public override unsafe void RegisterSystems(ref SystemRegistrationTable ecs) {
            ecs.Register(&UpdateClosestInteractable,
                new SysUpdate(GameLoopPhase.Update, 10),
                new SysPermissions()
                    .ReadWrite<PlayerController>()
                    .ReadShared<InputState>()
                );

            ecs.Register(&HandleInteractions,
                new SysUpdate(GameLoopPhase.Update, 15),
                new SysPermissions()
                    .ReadWrite<PlayerController>()
                    .WriteShared<InteractionArbiter>()
                    .ReadShared<InputState>()
                );
        }

        static private unsafe void UpdateClosestInteractable(float dt) {
            Find.State(out InputState input);

            var player = Find.FirstComponent<PlayerController>();
            if (!player) {
                return;
            }

            Interactable closestInteractable = null;

            if (player.Input.IsInputEnabled()) {

                Vector2 look = player.LookVector;
                look.y *= 0.404f;
                player.InteractionVolume.offset = look * 0.2f;

                using(var colliders = TempReferenceBuffer<Collider2D>.Create(MaxEvaluatedInteractables))
                using (var interactables = TempReferenceBuffer<Interactable>.Create(MaxEvaluatedInteractables)) {
                    ContactFilter2D interactionFilter = default;
                    interactionFilter.SetLayerMask(player.InteractionMask);
                    interactionFilter.useTriggers = true;

                    colliders.UpdateCount(player.InteractionVolume.OverlapCollider(interactionFilter, colliders.InternalArray));
                    InteractionSelectionRecords records = default;
                    for (int i = 0; i < colliders.Count; i++) {
                        Interactable interactable = colliders[i].ResolveComponent<Interactable>();
                        if (interactable) {
                            ColliderDistance2D distStats = player.InteractionVolume.Distance(colliders[i]);
                            records.Separations[interactables.Count] = distStats.distance;
                            records.Alignment[interactables.Count] = Vector2.Dot(player.LookVector, -distStats.normal);
                            interactables.Add(interactable);
                        }
                    }

                    if (interactables.Count > 0) {
                        // TODO: select based on distance, alignment
                        closestInteractable = interactables[0];
                    }
                }
            }

            player.ClosestInteractable = closestInteractable;
        }

        static private unsafe void HandleInteractions(float dt) {
            Find.State(out InputState input);

            var player = Find.FirstComponent<PlayerController>();
            if (!player) {
                return;
            }

            if (player.Input.IsInputEnabled()) {
                if (player.ClosestInteractable != null) {
                    if (input.ActionKeys.ConsumePress(ActionKeys.Interact)) {
                        InteractionUtility.RequestInteractionByActor(player.Actor, InteractVerb.Inspect, player.ClosestInteractable.Actor, null);
                    }
                }
            }
        }

        private unsafe struct InteractionSelectionRecords {
            public fixed float Separations[MaxEvaluatedInteractables];
            public fixed float Alignment[MaxEvaluatedInteractables];
        }
    }
}