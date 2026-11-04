using System;
using System.Collections;
using System.Collections.Generic;
using FieldDay;
using FieldDay.Rendering;
using UnityEngine;

namespace Airborne.Building {
    public class PerspectiveCameraTester : ComponentKit {
        public Camera Camera;
        public Transform CameraRoot;

        public RoomInstance Aim;
        public float Pitch;
        public float MinHeight;
        public float MaxHeight;

        protected override void ApplyChanges() {
            if (MaxHeight <= MinHeight || !Aim || Aim.Range.size.z <= 0 || !Camera || !CameraRoot) {
                return;
            }

            var instance = Camera;
            var transform = CameraRoot;

            float range = Aim.Range.size.z;
            float pitchRad = Pitch * Mathf.Deg2Rad;

            FrustumBoxFramingParams frustumBox;
            frustumBox.Range = PerspectiveMath.RotatedDistanceToPlane(range, pitchRad);
            frustumBox.MinHeight = MinHeight;
            frustumBox.MaxHeight = MaxHeight;

            FrustumBoxView view = PerspectiveMath.CalculateFrustumForBox(frustumBox);

            PrepareChange(instance);
            instance.fieldOfView = view.FOVDeg;
            instance.nearClipPlane = Math.Max(0.3f, view.DistanceFromMin - 15);
            instance.farClipPlane = view.DistanceFromMax + 128;

            PrepareChange(transform);

            float avgDistance = (view.DistanceFromMax + view.DistanceFromMin) / 2;

            Vector3 pos = Aim.transform.position;
            pos.z -= avgDistance * (float) Math.Cos(pitchRad);
            pos.y += avgDistance * (float) Math.Sin(pitchRad);

            Vector3 rot = default;
            rot.x = Pitch;

            transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
        }
    }
}
