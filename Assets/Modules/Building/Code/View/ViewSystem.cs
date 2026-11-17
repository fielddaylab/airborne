using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay;
using FieldDay.Systems;
using UnityEngine;

namespace Airborne.Building {
    public class ViewSystem : SystemComponent {
        public override unsafe void RegisterSystems(ref SystemRegistrationTable ecs)
        {
            ecs.Register(&UpdateView,
                new SysUpdate(GameLoopPhase.LateFixedUpdate, 12),
                new SysPermissions()
                    .Read<CollisionView>()
                    .ReadWrite<IsoView>()
            );
        }
        
        static private unsafe void UpdateView(float dt) {
            var view = Find.FirstComponent<IsoView>();
            var room = Find.FirstComponent<CollisionView>();
            
            while (view.QueueReady)
            {
                // remap all points and clear queue
                var remapper = view.GetFirstInQueue();
                remapper.TransformToMap.position = ViewUtility.WorldToViewPoint(remapper.WorldPosition, 
                    view.FloorCollider.bounds, 
                    room.Collider.bounds); 
            }
        }
    }
}
