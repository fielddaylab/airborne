using System.Collections;
using System.Collections.Generic;
using FieldDay.Components;
using UnityEngine;

namespace Airborne {
    public sealed class RoomInstance : BatchedComponent {
        [Header("Colliders")]
        public BoxCollider Range;
        public BoxCollider FloorCollider;

        [Header("Renderers")]
        public Transform DebugFloor;
    }
}
