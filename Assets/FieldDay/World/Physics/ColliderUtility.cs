using BeauUtil;
using FieldDay.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyIL;
using UnityEngine;

namespace FieldDay.Physics {
    static public class ColliderUtility {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [IntrinsicIL("ldarg.0; ldfld [arg contact]::m_Collider; ret")]
        static private int GetColliderId(ContactPoint2D contact) {
            throw new NotImplementedException();
        }

        #region Sorting

        static public void GroupContactPointsByCollider(WorkList<ContactPoint2D> contactPoints) {
            contactPoints.Sort(ContactPoint2DSortByCollider.Instance);
        }

        static public void SortContactPointsBySeparation(WorkList<ContactPoint2D> contactPoints) {
            contactPoints.Sort(ContactPoint2DSortBySeparation.Instance);
        }

        static public void SortContactPointsByReverseSeparation(WorkList<ContactPoint2D> contactPoints) {
            contactPoints.Sort(ContactPoint2DSortBySeparationReverse.Instance);
        }

        private class ContactPoint2DSortByCollider : IComparer<ContactPoint2D> {
            static internal readonly ContactPoint2DSortByCollider Instance = new ContactPoint2DSortByCollider();

            public int Compare(ContactPoint2D x, ContactPoint2D y) {
                return GetColliderId(x).CompareTo(GetColliderId(y));
            }
        }

        private class ContactPoint2DSortBySeparation : IComparer<ContactPoint2D> {
            static internal readonly ContactPoint2DSortBySeparation Instance = new ContactPoint2DSortBySeparation();

            public int Compare(ContactPoint2D x, ContactPoint2D y) {
                return x.separation.CompareTo(y.separation);
            }
        }

        private class ContactPoint2DSortBySeparationReverse : IComparer<ContactPoint2D> {
            static internal readonly ContactPoint2DSortBySeparationReverse Instance = new ContactPoint2DSortBySeparationReverse();

            public int Compare(ContactPoint2D x, ContactPoint2D y) {
                return y.separation.CompareTo(x.separation);
            }
        }

        #endregion // Sorting
    }
}