using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using UnityEngine;

namespace Airborne.Building
{
    public static class ViewUtility 
    {
        static public Vector2 WorldToViewPoint(Vector2 worldSpacePoint, Bounds viewBounds, Bounds worldBounds)
        {  
            var viewRect = Geom.BoundsToRect(viewBounds);
            var roomRect = Geom.BoundsToRect(worldBounds);

            var worldFromCenter = worldSpacePoint - roomRect.center;
            var pointPercentages = worldFromCenter / roomRect.size;
            return pointPercentages * viewRect.size +  viewRect.center;
        }
    }
}
