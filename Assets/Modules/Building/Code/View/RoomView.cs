using System.Collections;
using System.Collections.Generic;
using FieldDay.Components;
using UnityEngine;

namespace Airborne.Building
{
    public class RoomView : BatchedComponent
    {
        public BoxCollider2D FullRoomCollider;
        public BoxCollider2D FloorCollider;
        
        [Space]
        public PlayerView PlayerVisualizer; 
        public ResidentView[] Residents;
    }
}
