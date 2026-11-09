using BeauUtil;
using FieldDay.Collections;
using System;
using System.Runtime.CompilerServices;
using TinyIL;
using UnityEngine;

namespace FieldDay.Physics {
    static public class PhysicsBuffers {
        static private WorkList<ContactPoint2D> s_ContactPoint2DBuffer;
        static private WorkList<RaycastHit2D> s_RaycastHit2DBuffer;

        /// <summary>
        /// Returns a contact buffer.
        /// </summary>
        static public WorkList<ContactPoint2D> GetContact2DBuffer() {
            if (s_ContactPoint2DBuffer == null) {
                s_ContactPoint2DBuffer = new WorkList<ContactPoint2D>(32);
            } else {
                s_ContactPoint2DBuffer.Clear();
            }
            return s_ContactPoint2DBuffer;
        }

        /// <summary>
        /// Returns a raycast buffer.
        /// </summary>
        static public WorkList<RaycastHit2D> GetRaycast2DBuffer() {
            if (s_RaycastHit2DBuffer == null) {
                s_RaycastHit2DBuffer = new WorkList<RaycastHit2D>(32);
            } else {
                s_RaycastHit2DBuffer.Clear();
            }
            return s_RaycastHit2DBuffer;
        }
    }
}