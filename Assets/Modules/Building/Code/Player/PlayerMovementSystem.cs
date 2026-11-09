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
    public sealed class PlayerMovementSystem : SystemComponent {
        public override unsafe void RegisterSystems(ref SystemRegistrationTable ecs) {
            ecs.Register(&UpdatePlayerMotion,
                new SysUpdate(GameLoopPhase.FixedUpdate, 10),
                new SysPermissions()
                    .ReadWrite<PlayerController>()
                    .Read<CharacterBody>()
                    .ReadShared<InputState>()
                );

            ecs.Register(&ResolvePlayerOverlaps,
                new SysUpdate(GameLoopPhase.LateFixedUpdate, 10),
                new SysPermissions()
                    .ReadWrite<PlayerController>()
                    .Read<CharacterBody>()
                );
        }

        static private void UpdatePlayerMotion(float dt) {
            Find.State(out InputState input);

            var player = Find.FirstComponent<PlayerController>();
            if (!player) {
                return;
            }

            if (player.Input.IsInputEnabled()) {
                Vector2 move = input.NavAnalogNormalized;

                if (move.sqrMagnitude > 0) {
                    Collider2D collider = player.Body.Collider;

                    float moveSpeed = player.MoveSpeed * dt;
                    Vector2 offset = move * moveSpeed;

                    player.Body.Transform.localPosition += (Vector3) offset;
                    player.LookVector = move.normalized;
                }
            }
        }

        static private void ResolvePlayerOverlaps(float dt) {
            Find.State(out InputState input);

            var player = Find.FirstComponent<PlayerController>();
            if (!player) {
                return;
            }

            ContactFilter2D contactFilter = default;
            contactFilter.SetLayerMask(player.SolidMask);

            WorkList<ContactPoint2D> contacts = PhysicsBuffers.GetContact2DBuffer();
            contacts.UpdateCount(Physics2D.GetContacts(player.Body.Collider, contactFilter, contacts.InternalArray));
            if (contacts.Count > 0) {
                Vector2 shift = default;
                for (int i = 0; i < contacts.Count; i++) {
                    ContactPoint2D contact = contacts[i];
                    float separation = contact.separation + (1f / 128);
                    float adjustedSeparation = separation + Vector2.Dot(contact.normal, shift);
                    if (adjustedSeparation < 0) {
                        shift -= adjustedSeparation * contact.normal;
                    }
                }

                if (shift.sqrMagnitude > 0) {
                    player.Body.transform.localPosition += (Vector3) shift;
                }
            }
        }
    }
}