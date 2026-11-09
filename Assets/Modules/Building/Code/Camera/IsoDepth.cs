using System;
using System.Collections.Generic;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using FieldDay.Scenes;
using UnityEngine;
using UnityEngine.Rendering;

namespace Airborne.Building {
    [RequireComponent(typeof(SortingGroup))]
    [ExecuteAlways]
    public sealed class IsoDepth : MonoBehaviour, IComponentData, IScenePreload {
        public Transform Root;
        public bool IsStatic;

        [NonSerialized] public SortingGroup CachedSorting;

#if UNITY_EDITOR
        [NonSerialized] private bool m_IsEditor;
#endif // UNITY_EDITOR

        private void OnEnable() {
            Game.Components?.Register(this);
        }

        private void OnDisable() {
            Game.Components?.Deregister(this);
        }

        public IEnumerator<WorkSlicer.Result?> Preload() {
            this.CacheComponent(ref CachedSorting);
            if (!Root) {
                Root = transform;
            }

            CachedSorting.sortingOrder = CalculateDepth(Root.position.y);
            if (IsStatic) {
                enabled = false;
            }
            return null;
        }

        static public int CalculateDepth(float y) {
            return (int) (-y * 32);
        }

#if UNITY_EDITOR
        private void LateUpdate() {
            if (!Game.IsEditMode) {
                return;
            }

            this.CacheComponent(ref CachedSorting);

            if (!Root) {
                Root = transform;
            }

            CachedSorting.sortingOrder = CalculateDepth(Root.position.y);
        }
#endif // UNITY_EDITOR
    }
}