using System;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using ScriptableBake;
using UnityEngine;

namespace Airborne.Building {
    public sealed class Resident : BatchedComponent, IBaked, IRegistrationCallbacks {
        [NonSerialized] public Transform Transform;
        [HideInInspector] public StringHash32 Id;
        [HideInInspector] public int Index;
        
        public ViewTransformPair ViewTransform;

#if UNITY_EDITOR

        int IBaked.Order { get { return -1003; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            Id = name;
            return true;
        }

#endif // UNITY_EDITOR
        public void OnRegister()
        {
            this.CacheComponent(ref Transform);
            if (ViewTransform) ViewTransform.QueueTransformUpdate();
        }

        public void OnDeregister()
        {
            
        }
    }
}