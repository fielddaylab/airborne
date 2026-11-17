using System.Collections;
using System.Collections.Generic;
using FieldDay.Components;
using UnityEngine;

namespace Airborne.Building
{
    public class IsoView : BatchedComponent
    {
        public BoxCollider2D FullRoomCollider;
        public BoxCollider2D FloorCollider;
        
        [Space, HideInInspector]
        public List<ViewTransformProps> RemappingQueue;
        
        public bool QueueReady => RemappingQueue != null && RemappingQueue.Count > 0;
        
        public void AddToQueue(ViewTransformProps remappingParams)
        {
            if (RemappingQueue == null) 
                RemappingQueue = new List<ViewTransformProps>();
            
            RemappingQueue.Add(remappingParams);
        }
        
        public ViewTransformProps GetFirstInQueue()
        {
            if (QueueReady)
            {
                var first = RemappingQueue[0];
                RemappingQueue.RemoveAt(0);
                return first;
            }
            else
            {
                Debug.LogWarning("Queue is empty");
                return default;
            }
        }
    }
}
