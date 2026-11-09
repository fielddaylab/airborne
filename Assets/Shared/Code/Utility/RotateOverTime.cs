using FieldDay;
using UnityEngine;

namespace Airborne {
    public sealed class RotateOverTime : MonoBehaviour {
        public Vector3 Rotate;
        
        private void LateUpdate() {
            transform.Rotate(Rotate * Frame.DeltaTime, Space.Self);
        }
    }
}