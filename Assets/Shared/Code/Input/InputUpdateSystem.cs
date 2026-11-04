using System;
using System.Collections;
using System.Collections.Generic;
using FieldDay;
using FieldDay.HID;
using FieldDay.SharedState;
using FieldDay.Systems;
using UnityEngine;

namespace Airborne {
    public sealed class InputUpdateSystem : SystemComponent {
        public unsafe override void RegisterSystems(ref SystemRegistrationTable ecs) {
            ecs.Register(&Update,
                new SysUpdate(GameLoopPhase.PreUpdate, -100),
                new SysPermissions()
                    .ReadWriteShared<InputState>()
                );
        }

        static private void Update(float dt) {
            Find.State(out InputState state);
        }
    }
}
