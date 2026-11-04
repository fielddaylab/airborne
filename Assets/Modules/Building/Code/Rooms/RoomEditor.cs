using System.Collections;
using System.Collections.Generic;
using FieldDay;
using FieldDay.Components;
using UnityEngine;

namespace Airborne {
    [RequireComponent(typeof(RoomInstance))]
    public sealed class RoomEditor : ComponentKit {
        public Vector3 RoomSize;

        protected override void ApplyChanges() {
            var instance = GetComponent<RoomInstance>();

            if (instance.Range) {
                PrepareChange(instance.Range);
                instance.Range.size = RoomSize;
                instance.Range.center = new Vector3(0, RoomSize.y / 2, 0);
            }

            if (instance.FloorCollider) {
                PrepareChange(instance.FloorCollider);
                instance.FloorCollider.size = new Vector3(RoomSize.x, 0.5f, RoomSize.z);
                instance.FloorCollider.center = new Vector3(0, -0.25f, 0);
            }

            if (instance.DebugFloor) {
                PrepareChange(instance.DebugFloor);
                instance.DebugFloor.localScale = new Vector3(RoomSize.x, 1, RoomSize.z);
            }
        }
    }
}
