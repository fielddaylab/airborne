using System;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using UnityEngine;

namespace Airborne.Building {
    public sealed class Actor : BatchedComponent, IRegistrationCallbacks {
        [NonSerialized] public RoomInstance CurrentRoom;
        [NonSerialized] public Transform Transform;

        void IRegistrationCallbacks.OnDeregister() {
            
        }

        void IRegistrationCallbacks.OnRegister() {
            this.CacheComponent(ref Transform);
        }
    }
}