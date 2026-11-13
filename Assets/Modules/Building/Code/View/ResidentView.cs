using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using UnityEngine;

namespace Airborne.Building
{
    public class ResidentView : BatchedComponent, IRegistrationCallbacks {
        [NonSerialized] public Transform Transform;

        void IRegistrationCallbacks.OnDeregister() {
            
        }

        void IRegistrationCallbacks.OnRegister() {
            this.CacheComponent(ref Transform);
        }
    }
}
