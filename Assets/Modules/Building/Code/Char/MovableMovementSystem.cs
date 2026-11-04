using System;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using FieldDay.Systems;
using UnityEngine;

namespace Airborne.Building {
    public sealed class MovableMovementSystem : SystemComponent {
        public override unsafe void RegisterSystems(ref SystemRegistrationTable ecs) {
            ecs.Register(&Update,
                new SysUpdate(GameLoopPhase.FixedUpdate, 10),
                new SysPermissions()
                    .ReadWrite<Movable>()
                );
        }

        static private void Update(float dt) {
            Physics.SyncTransforms();

            var movables = Find.Components<Movable>();

            foreach (var movable in movables) {
                
            }
        }
    }
}