using System;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using UnityEngine;

namespace Airborne.Building {
    public sealed class Movable : BatchedComponent, IRegistrationCallbacks {
        public Collider PrimaryCollider;
        public LayerMask SolidMask;
        public LayerMask PushMask;

        [NonSerialized] public Transform Transform;
        [NonSerialized] public Vector3 DesiredMovement;

        #region IRegistrationCallbacks

        void IRegistrationCallbacks.OnDeregister() {

        }

        void IRegistrationCallbacks.OnRegister() {
            this.CacheComponent(ref Transform);
        }

        #endregion // IRegistrationCallbacks
    }
}