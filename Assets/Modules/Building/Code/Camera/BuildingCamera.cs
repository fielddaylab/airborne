using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using UnityEngine;

namespace Airborne {
    public class BuildingCamera : MonoBehaviour {
        public Transform RootPosition;
        public Transform EffectsPosition;
        public Camera Camera;

        private void Awake() {
            //CameraHelper.FOVForHeightAndDistance(10, )
        }
    }
}
