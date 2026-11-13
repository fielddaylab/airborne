using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay;
using FieldDay.Systems;
using UnityEngine;

namespace Airborne.Building {
    public class ViewSystem : SystemComponent {
        //todo: WIP to be of State/System structure
        
        public override unsafe void RegisterSystems(ref SystemRegistrationTable ecs)
        {
            ecs.Register(&UpdateView,
                new SysUpdate(GameLoopPhase.LateFixedUpdate, 12),
                new SysPermissions()
                    .Read<PlayerController>()
                    .Read<RemappableBounds>()
                    .ReadWrite<RoomView>()
                    // todo: probably need to write to all the visualizers (doing it all in one state for now)
            );
        }
        
        static private unsafe void UpdateView(float dt) {
            var player = Find.FirstComponent<PlayerController>();
            var view = Find.FirstComponent<RoomView>();
            var room = Find.FirstComponent<RemappableBounds>();
            
            view.PlayerVisualizer.Transform.position = 
                ViewUtility.WorldToViewPoint(player.Body.Transform.localPosition, 
                    view.FloorCollider.bounds, 
                    room.Collider.bounds); 
        }
    }
}
