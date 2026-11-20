using BeauUtil;
using FieldDay.Components;
using ScriptableBake;
using UnityEngine;

namespace Airborne.Building {
    public sealed class FeatureInstance : BatchedComponent, IBaked {
        [HideInInspector] public StringHash32 Id;
        [HideInInspector] public int Index;

        public bool Enabled = true;

#if UNITY_EDITOR

        int IBaked.Order { get { return -1001; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            Id = name;
            return true;
        }

#endif // UNITY_EDITOR

        public void OnRoomEnabled() {   // these could be pulled into an interface instead?
            if (Enabled) {
                gameObject.SetActive(true);
            }
        }

        public void OnRoomDisabled() {
            gameObject.SetActive(false);
        }
    }
}