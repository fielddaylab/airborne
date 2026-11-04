using BeauUtil;
using FieldDay.Rendering;
using NUnit.Framework;
using System;
using UnityEngine;

namespace FieldDay.Editor.UnitTests {
    static internal class MathUnitTests {
        [Test]
        static public void CanFindPerspectiveParametersForBox() {
            System.Random r = new System.Random(640);

            const float precision = 0.001f;
            int iter = 10000;
            int iterCount = iter;
            float errAccum = 0;
            while (iter-- > 0) {
                FrustumBoxFramingParams box;
                box.MinHeight = MathUtils.Quantize(r.NextFloat(4, 100), 0.52f);
                box.MaxHeight = MathUtils.Quantize(box.MinHeight + r.NextFloat(0.5f, 100), 0.25f);
                box.Range = MathUtils.Quantize(r.NextFloat(1, 600), 0.05f);

                FrustumBoxView camera = PerspectiveMath.CalculateFrustumForBox(box);

                float computedMinHeight = CameraHelper.HeightForDistanceAndFOV(camera.DistanceFromMin, camera.FOVDeg);
                float computedMaxHeight = CameraHelper.HeightForDistanceAndFOV(camera.DistanceFromMin + box.Range, camera.FOVDeg);

                float minHeightError = Math.Abs(computedMinHeight - box.MinHeight);
                float maxHeightError = Math.Abs(computedMaxHeight - box.MaxHeight);

                errAccum += (minHeightError + maxHeightError) / 2;

                float minRelError = minHeightError / box.MinHeight;
                float maxRelError = maxHeightError / box.MaxHeight;

                //Debug.LogFormat("Min Height: {0} err {1} ({2}%); Max Height: {3} err {4} ({5}%)", computedMinHeight, minHeightError, minRelError * 100, computedMaxHeight, maxHeightError, maxRelError * 100);

                Assert.IsTrue(minHeightError < precision);
                Assert.IsTrue(maxHeightError < precision);
            }

            Debug.LogFormat("Average Error: {0}", errAccum / iterCount);
        }
    }
}