using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using UnityEngine;

namespace Airborne.Building
{
    public struct ViewTransformProps //todo rename
    {
        public Transform TransformToMap;
        public Vector2 WorldPosition;
    }
    
    public class ViewTransformPair : BatchedComponent {
        public Transform CollisionTransform;
        public Transform IsoTransform;
        
        public void QueueTransformUpdate()
        {
            // todo: this definitely is not state/system friendly
            var view = Find.FirstComponent<IsoView>();
            view?.AddToQueue(new ViewTransformProps
            {
                TransformToMap = IsoTransform,
                WorldPosition = CollisionTransform.position
            });
        }
    }
}
