using BeauUtil;
using FieldDay.Components;
using ScriptableBake;
using UnityEngine;

namespace Airborne.Building {
    public class RoomRenderElement : BatchedComponent, IBaked {
        [HideInInspector] public StringHash32 Id;
        [HideInInspector] public int Index;
        
        [HideInInspector] public RenderElementState State;
        
        // todo: hook this up
        public readonly CastableEvent<RenderElementState> OnStateChange = new CastableEvent<RenderElementState>();
        
#if UNITY_EDITOR

        int IBaked.Order { get { return -1001; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            Id = name;
            return true;
        }

#endif // UNITY_EDITOR
        
        public void OnRoomEnabled() {
            if (State != RenderElementState.Disabled) {
                gameObject.SetActive(true);
            }
        }

        public void OnRoomDisabled() {
            gameObject.SetActive(false);
        }
    }

    public enum RenderElementState {
        Enabled,
        NotAnimating,
        Disabled
    }
}