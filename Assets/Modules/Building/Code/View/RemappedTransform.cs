using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using UnityEngine;

namespace Airborne.Building
{
    public class RemappedTransform : BatchedComponent {
        public Transform RemappableTransform;
        public Transform VisualisedTransform;
        
        public void UpdateTransforms()
        {
            //VisualisedTransform.position = 
            //    ViewUtility.WorldToViewPoint(RemappableTransform.localPosition, 
            //        view.FloorCollider.bounds, 
            //        room.Collider.bounds); 
        }
    }
}
