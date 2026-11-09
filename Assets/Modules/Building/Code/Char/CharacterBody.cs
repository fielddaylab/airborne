using System;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using UnityEngine;

namespace Airborne.Building {
    public sealed class CharacterBody : BatchedComponent, IRegistrationCallbacks {
        public Collider2D Collider;
        [NonSerialized] public Transform Transform;

        void IRegistrationCallbacks.OnDeregister() {
            
        }

        void IRegistrationCallbacks.OnRegister() {
            this.CacheComponent(ref Transform);
        }
    }
}