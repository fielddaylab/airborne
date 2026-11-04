using BeauUtil;
using FieldDay.Assets;
using FieldDay.Collections;
using FieldDay.Components;
using ScriptableBake;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FieldDay.UI {
    [DisallowMultipleComponent]
    public sealed class WorldInputLayer : BatchedComponent, IInputLayer, IRegistrationCallbacks {
        #region Inspector

        [SerializeField, InputGroupName] private StringHash32 m_GroupId;

        [Header("Sorting")]
        [SerializeField, SortingLayer] private int m_SortingLayerId;
        [SerializeField] private short m_SortingOrder;

        #endregion // Inspector

        public readonly CastableEvent<WorldInputLayer, bool> OnStateChanged = new CastableEvent<WorldInputLayer, bool>();

        [NonSerialized] private InputLayerMask m_Mask;
        [NonSerialized] private bool m_InputEnabled;
        [NonSerialized] private bool m_IsPushed;
        [NonSerialized] private bool m_PushQueued;
        [NonSerialized] private bool m_IsRegistered;

        private void Awake() {
            m_Mask.GroupId = m_GroupId;
            m_Mask.LayerMask = (1 << gameObject.layer);

            m_Mask.SortKey = CanvasSortKey.CreateForWorld(m_SortingLayerId, m_SortingOrder);
        }

        void IRegistrationCallbacks.OnRegister() {
            m_IsRegistered = true;
            Game.Gui.RegisterInputLayer(this);
            if (m_PushQueued && !m_IsPushed) {
                m_IsPushed = true;
                Game.Gui.PushPriority(this);
            }
        }

        void IRegistrationCallbacks.OnDeregister() {
            if (m_IsPushed) {
                Game.Gui.PopPriority(this);
                m_IsPushed = false;
            }
            Game.Gui.DeregisterInputLayer(this);
            m_IsRegistered = false;
        }

        public InputLayerMask InputMask {
            get { return m_Mask; }
            set { m_Mask = value; }
        }

        public bool IsInputEnabled() {
            return m_InputEnabled;
        }

        public bool TryPushPriority() {
            if (!m_IsPushed) {
                m_IsPushed = true;
                m_PushQueued = true;
                if (m_IsRegistered) {
                    Game.Gui.PushPriority(this);
                }
                return true;
            }

            return false;
        }

        public bool TryPopPriority() {
            if (m_IsPushed) {
                m_IsPushed = false;
                m_PushQueued = false;
                if (m_IsRegistered) {
                    Game.Gui.PopPriority(this);
                }
                return true;
            }

            return false;
        }

        void IInputLayer.UpdateInputEnabled(bool enabled) {
            if (m_InputEnabled != enabled) {
                m_InputEnabled = enabled;
                OnStateChanged.Invoke(this, enabled);
            }
        }
    }
}