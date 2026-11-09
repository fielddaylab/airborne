using System;
using BeauUtil;
using FieldDay.Components;
using FieldDay.Scripting;
using FieldDay.SharedState;
using FieldDay.UI;
using UnityEngine;

namespace Airborne.Building {
    public sealed class PlayerController : ScriptActorComponent {
        public WorldInputLayer Input;

        [Header("Movement")]
        public CharacterBody Body;
        public float MoveSpeed = 2;
        public LayerMask SolidMask;

        [Header("Interactions")]
        public Collider2D InteractionVolume;
        public LayerMask InteractionMask;

        [NonSerialized] public Interactable ClosestInteractable;
        [NonSerialized] public Vector2 LookVector = Vector2.right;
    }
}