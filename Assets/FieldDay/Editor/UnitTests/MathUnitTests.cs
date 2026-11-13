using BeauUtil;
using FieldDay.Collections;
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

        [Test]
        static public void CanBitPack64() {
            ulong bits = 0;
            bits = BitPack.Pack64(3, 5, 0);
            bits |= BitPack.Pack64(16, 5, 5);

            Assert.AreEqual(3, BitPack.Unpack64(bits, 5, 0));
            Assert.AreEqual(16, BitPack.Unpack64(bits, 5, 5));
        }

        [Test]
        static public void CanBitPack32() {
            uint bits = 0;
            bits |= BitPack.Pack32(3, 5, 0);
            bits |= BitPack.Pack32(16, 5, 5);

            Assert.AreEqual(3, BitPack.Unpack32(bits, 5, 0));
            Assert.AreEqual(16, BitPack.Unpack32(bits, 5, 5));
        }

        [Test]
        static public void CanBitPack16() {
            ushort bits = 0;
            bits = BitPack.Pack16(3, 5, 0);
            bits |= BitPack.Pack16(16, 5, 5);

            Assert.AreEqual(3, BitPack.Unpack16(bits, 5, 0));
            Assert.AreEqual(16, BitPack.Unpack16(bits, 5, 5));
        }

        [Test]
        static public void CanBitPack8() {
            byte bits = 0;
            bits = BitPack.Pack8(3, 3, 0);
            bits |= BitPack.Pack8(16, 5, 3);

            Assert.AreEqual(3, BitPack.Unpack8(bits, 3, 0));
            Assert.AreEqual(16, BitPack.Unpack8(bits, 5, 3));
        }
    }
}