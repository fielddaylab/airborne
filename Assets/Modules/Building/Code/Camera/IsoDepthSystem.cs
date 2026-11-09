using System;
using System.Collections.Generic;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using FieldDay.Scenes;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.Rendering;

namespace Airborne.Building {
    public sealed class IsoDepthSystem : SystemComponent {
        public override unsafe void RegisterSystems(ref SystemRegistrationTable ecs) {
            ecs.Register(&ApplySorting,
                new SysUpdate(GameLoopPhase.UnscaledLateUpdate, 1000),
                new SysPermissions()
                    .ReadWrite<IsoDepth>());
        }

        static private void ApplySorting(float dt) {
            var isos = Find.Components<IsoDepth>();

            foreach(var iso in isos) {
                iso.CachedSorting.sortingOrder = IsoDepth.CalculateDepth(iso.Root.transform.position.y);
            }
        }
    }
}