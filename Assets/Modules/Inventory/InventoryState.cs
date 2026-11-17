using System;
using UnityEngine;
using Airborne.Building;
using FieldDay.SharedState;
using BeauUtil;
using Airborne.Pollutants;
using BeauUtil.Debugger;

namespace Airborne.Inventory {
    public sealed class InventoryState : SharedStateComponent {
        // asserts for items and clues counts
        private const byte StaticAssert_RoomsIs16 = (BuildingConsts.MaxRooms <= 16 ? 0 : -1);
        private const byte StaticAssert_ItemsIs32 = (BuildingConsts.MaxItems <= 32 ? 0 : -1);
        private const byte StaticAssert_CluesIs32 = (BuildingConsts.MaxClues <= 32 ? 0 : -1);

        [NonSerialized] public RoomInventory[] Rooms;
        [NonSerialized] public ResidentInventory[] Residents;
        [NonSerialized] public PassageInstance[] Passages;
        [NonSerialized] public FeatureInventory[] Features;

        [NonSerialized] public ushort RoomsObservedBits;
        [NonSerialized] public BitSet32 ItemsCollected;
        [NonSerialized] public BitSet32 CluesCollected;
    }

    public struct ResidentInventory {
        public byte ObservedScheduleMask;
    }

    public struct PassageInventory {
        public byte ObservedScheduleMask;
    }

    public struct FeatureInventory {
        public byte RecordedActivationStates;
    }

    public unsafe struct RoomInventory {
        public RecordedRoomLevels RoomLevels;
    }

    static public partial class InventoryUtility {
        /// <summary>
        /// Allocates buffers for the player's inventory state.
        /// </summary>
        static public void AllocateBuffers(InventoryState inv, BuildingManifestCounts counts) {
            Assert.NotNullOrDestroyed(inv);
            Assert.True(inv.Rooms == null, "Inventory state already allocated");

            inv.Rooms = new RoomInventory[counts.RoomCount];
            inv.Residents = new ResidentInventory[counts.ResidentCount];
            inv.Passages = new PassageInstance[counts.PassageCount];
            inv.Features = new FeatureInventory[counts.FeatureCount];

            inv.RoomsObservedBits = 0;
            inv.ItemsCollected = default;
            inv.CluesCollected = default;
        }

        /// <summary>
        /// Clears the player's inventory of items.
        /// </summary>
        static public void ClearItems(InventoryState inv) {
            Assert.NotNullOrDestroyed(inv);
            
            inv.ItemsCollected.Clear();
        }
    }
}