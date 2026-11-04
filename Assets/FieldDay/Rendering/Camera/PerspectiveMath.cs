using BeauUtil;
using BeauUtil.Debugger;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FieldDay.Rendering {
    [Serializable]
    public struct FrustumBoxFramingParams {
        public float MinHeight;
        public float MaxHeight;
        public float Range;
    }

    [Serializable]
    public struct FrustumBoxView {
        public float FOVDeg;
        public float DistanceFromMin;
        public float DistanceFromMax;
    }

    static public class PerspectiveMath {
        /// <summary>
        /// Computes the view parameters required to fit the given frustum box.
        /// </summary>
        static public FrustumBoxView CalculateFrustumForBox(FrustumBoxFramingParams box) {
            Assert.True(box.MinHeight < box.MaxHeight && box.Range > 0, "Frustum must not be orthographic");

            // Method 1: find distance, then fov
            // 1 trig function
            // slightly less accurate (avg. err. 3.348136E-06)
            float d = (box.MinHeight * box.Range) / (box.MaxHeight - box.MinHeight);
            float fov = 2f * (float)Math.Atan(box.MinHeight * 0.5f / d) * Mathf.Rad2Deg;

            // Method 2: find fov, then distance
            // 2 trig functions
            // slightly more accurate (avg. err. 2.293038E-06)
            //float fov = 2f * (float)Math.Atan((box.MaxHeight - box.MinHeight) / (2 * box.Range)) * Mathf.Rad2Deg;
            //float d = (float)(box.MinHeight * 0.5f / Math.Tan(fov / 2 * Mathf.Deg2Rad));

            FrustumBoxView result;
            result.DistanceFromMin = d;
            result.DistanceFromMax = d + box.Range;
            result.FOVDeg = fov;
            return result;
        }

        /// <summary>
        /// Computes the distance from a point to a plane with a rotated normal towards the plane.
        /// Equivalent to distance / cos(angle).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public float RotatedDistanceToPlane(float perpendicularDistance, float pitch) {
            return perpendicularDistance / (float) Math.Cos(pitch);
        }
    }
}