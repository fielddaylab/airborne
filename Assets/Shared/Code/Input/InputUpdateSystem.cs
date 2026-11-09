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

            NavigationKeys navKeys = 0;
            if (Game.Input.IsKeyDown(KeyCode.LeftArrow) || Game.Input.IsKeyDown(KeyCode.A)) {
                navKeys |= NavigationKeys.Left;
            }
            if (Game.Input.IsKeyDown(KeyCode.RightArrow) || Game.Input.IsKeyDown(KeyCode.D)) {
                navKeys |= NavigationKeys.Right;
            }
            if (Game.Input.IsKeyDown(KeyCode.UpArrow) || Game.Input.IsKeyDown(KeyCode.W)) {
                navKeys |= NavigationKeys.Up;
            }
            if (Game.Input.IsKeyDown(KeyCode.DownArrow) || Game.Input.IsKeyDown(KeyCode.S)) {
                navKeys |= NavigationKeys.Down;
            }

            state.NavKeys.Update(navKeys);

            Vector2 moveRaw = default;
            if ((navKeys & NavigationKeys.Left) != 0) {
                moveRaw.x -= 1;
            }
            if ((navKeys & NavigationKeys.Right) != 0) {
                moveRaw.x += 1;
            }
            if ((navKeys & NavigationKeys.Up) != 0) {
                moveRaw.y += 1;
            }
            if ((navKeys & NavigationKeys.Down) != 0) {
                moveRaw.y -= 1;
            }

            state.NavAnalogRaw = moveRaw;

            float maxComponent = Math.Max(Math.Abs(moveRaw.x), Math.Abs(moveRaw.y));
            state.NavAnalogNormalized = moveRaw.normalized * maxComponent;

            ActionKeys actionKeys = 0;

            if (Game.Input.IsKeyDown(KeyCode.Space) || Game.Input.IsKeyDown(KeyCode.E) || Game.Input.IsKeyDown(KeyCode.Return)) {
                actionKeys |= ActionKeys.Interact;
            }
            if (Game.Input.IsKeyDown(KeyCode.Backspace) || Game.Input.IsKeyDown(KeyCode.Escape)) {
                actionKeys |= ActionKeys.Back;
            }
            if (Game.Input.IsKeyDown(KeyCode.Tab) || Game.Input.IsKeyDown(KeyCode.Q)) {
                actionKeys |= ActionKeys.Menu;
            }

            state.ActionKeys.Update(actionKeys);
        }
    }
}
