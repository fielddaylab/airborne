using System;
using System.Collections.Generic;
using Airborne.Pollutants;
using Airborne.Schedule;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.SharedState;
using ScriptableBake;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Airborne.Building {
    /// <summary>
    /// Tracks all state for building objects.
    /// </summary>
    public sealed class BuildingState : SharedStateComponent {
        private const byte StaticAssert_PassagesIs32 = (BuildingConsts.MaxPassages <= 32 ? 0 : -1);
        private const byte StaticAssert_MaxFeaturesIs64 = (BuildingConsts.MaxFeatures <= 64 ? 0 : -1);

        [NonSerialized] public RuntimeRoomState[] Rooms;
        [NonSerialized] public RuntimeResidentState[] Residents;
        [NonSerialized] public RuntimeItemState[] Items;
        [NonSerialized] public RuntimeMeterState[] Meters;

        [NonSerialized] public BitSet32 EnabledPassages;
        [NonSerialized] public BitSet32 EnabledClues;
        [NonSerialized] public BitSet64 EnabledFeatures;
    }

    public struct RuntimeRoomState {
        public BitSet32 Residents;
        public BitSet32 Items;
    }

    public struct RuntimeResidentState {
        public byte RoomIndex;
        public byte PostIndex;
        public Symptom Symptom;
        public ResidentTimeBlockFlags Flags;
        public StringHash32 ScriptNode;
    }

    public struct RuntimeItemState {
        public byte RoomIndex;
        public byte PostIndex;
    }
    
    public struct RuntimeMeterState {
        public byte RoomIndex;
        public byte PostIndex;
        public byte PollutantIndex;
    }

    static public partial class BuildingUtility {
        static public void AllocateStateBuffers(BuildingState buildingState, BuildingManifestCounts objectCounts, int meterCount) {
            Assert.NotNullOrDestroyed(buildingState);
            Assert.True(buildingState.Rooms == null, "State buffers already initialized");

            buildingState.Rooms = new RuntimeRoomState[objectCounts.RoomCount];
            buildingState.Residents = new RuntimeResidentState[objectCounts.ResidentCount];
            buildingState.Items = new RuntimeItemState[objectCounts.ItemCount];
            buildingState.Meters = new RuntimeMeterState[meterCount];
        }

        static public void SyncResidentVisualPositions(BuildingState buildingState, BuildingManifest manifest) {
            Assert.NotNullOrDestroyed(buildingState);
            Assert.NotNullOrDestroyed(manifest);

            int residentCount = manifest.Residents.Length;
            for(int residentIndex = 0; residentIndex < residentCount; residentIndex++) {
                RuntimeResidentState state = buildingState.Residents[residentIndex];
                Resident instance = manifest.Residents[residentIndex];

                if ((state.Flags & ResidentTimeBlockFlags.Hidden) != 0) {
                    instance.gameObject.SetActive(false);
                } else {
                    instance.gameObject.SetActive(true);

                    RoomInstance room = BuildingLookup.GetRoomInstance(manifest, state.RoomIndex);
                    ActionPost post = BuildingLookup.GetPostInstance(room, state.PostIndex);

                    instance.transform.position = post.transform.position;
                }
            }
        }
    }
}