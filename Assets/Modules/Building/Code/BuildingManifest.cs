using System;
using System.Collections.Generic;
using Airborne.Pollutants;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.SharedState;
using ScriptableBake;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Airborne.Building {
    /// <summary>
    /// Manifest of all rooms, residents, features, and passages.
    /// </summary>
    public sealed class BuildingManifest : SharedStateComponent, IBaked {
        [HideInInspector] public StringHash32[] RoomNames;
        [HideInInspector] public StringHash32[] ResidentNames;
        [HideInInspector] public StringHash32[] FeatureNames;
        [HideInInspector] public StringHash32[] PassageNames;

        [HideInInspector] public RoomInstance[] Rooms;
        [HideInInspector] public Resident[] Residents;
        [HideInInspector] public FeatureInstance[] Features;
        [HideInInspector] public PassageInstance[] Passages;

#if UNITY_EDITOR

        int IBaked.Order { get { return 1001; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            BuildNameTable(context.Scene, out RoomNames, out Rooms, (a) => a.Id, (a, i) => a.Index = i);
            BuildNameTable(context.Scene, out ResidentNames, out Residents, (a) => a.Id, (a, i) => a.Index = i);
            BuildNameTable(context.Scene, out FeatureNames, out Features, (a) => a.Id, (a, i) => a.Index = i);
            BuildNameTable(context.Scene, out PassageNames, out Passages, (a) => a.Id, (a, i) => a.Index = i);

            return true;
        }

        static private void BuildNameTable<T>(Scene scene, out StringHash32[] names, out T[] references, Func<T, StringHash32> idFunc, Action<T, int> indexHandler) where T : Component {
            List<T> components = new List<T>();
            scene.GetAllComponents<T>(true, components);
            components.Sort((a, b) => idFunc(a).CompareTo(idFunc(b)));

            references = components.ToArray();

            if (indexHandler != null) {
                for(int i = 0; i < references.Length; i++) {
                    indexHandler(references[i], i);
                }
            } 

            names = new StringHash32[references.Length];
            for(int i = 0; i < names.Length; i++) {
                names[i] = idFunc(references[i]);
            }
        }

#endif // UNITY_EDITOR
    }

    static public partial class BuildingLookup {
        #region Room Name Resolution

        static public int GetRoomIndex(BuildingManifest map, StringHash32 roomName) {
            Assert.NotNullOrDestroyed(map);
            int index = ArrayUtils.IndexOf(map.RoomNames, roomName);
            Assert.True(index >= 0, "Room '{0}' not present", roomName);
            return index;
        }

        static public StringHash32 GetRoomId(BuildingManifest map, int roomIndex) {
            Assert.NotNullOrDestroyed(map);
            Assert.True(roomIndex >= 0 && roomIndex < map.RoomNames.Length, "Room index {0} is out of range");
            return map.RoomNames[roomIndex];
        }

        static public RoomInstance GetRoomInstance(BuildingManifest map, int roomIndex) {
            Assert.NotNullOrDestroyed(map);
            Assert.True(roomIndex >= 0 && roomIndex < map.RoomNames.Length, "Room index {0} is out of range");
            return map.Rooms[roomIndex];
        }

        #endregion // Room Name Resolution

        #region Resident Name Resolution

        static public int GetResidentIndex(BuildingManifest map, StringHash32 roomName) {
            Assert.NotNullOrDestroyed(map);
            int index = ArrayUtils.IndexOf(map.ResidentNames, roomName);
            Assert.True(index >= 0, "Resident '{0}' not present", roomName);
            return index;
        }

        static public StringHash32 GetResidentId(BuildingManifest map, int roomIndex) {
            Assert.NotNullOrDestroyed(map);
            Assert.True(roomIndex >= 0 && roomIndex < map.ResidentNames.Length, "Resident index {0} is out of range");
            return map.ResidentNames[roomIndex];
        }

        static public Resident GetResidentInstance(BuildingManifest map, int roomIndex) {
            Assert.NotNullOrDestroyed(map);
            Assert.True(roomIndex >= 0 && roomIndex < map.ResidentNames.Length, "Resident index {0} is out of range");
            return map.Residents[roomIndex];
        }

        #endregion // Resident Name Resolution

        #region Feature Name Resolution

        static public int GetFeatureIndex(BuildingManifest map, StringHash32 roomName) {
            Assert.NotNullOrDestroyed(map);
            int index = ArrayUtils.IndexOf(map.FeatureNames, roomName);
            Assert.True(index >= 0, "Feature '{0}' not present", roomName);
            return index;
        }

        static public StringHash32 GetFeatureId(BuildingManifest map, int roomIndex) {
            Assert.NotNullOrDestroyed(map);
            Assert.True(roomIndex >= 0 && roomIndex < map.FeatureNames.Length, "Feature index {0} is out of range");
            return map.FeatureNames[roomIndex];
        }

        static public FeatureInstance GetFeatureInstance(BuildingManifest map, int roomIndex) {
            Assert.NotNullOrDestroyed(map);
            Assert.True(roomIndex >= 0 && roomIndex < map.FeatureNames.Length, "Feature index {0} is out of range");
            return map.Features[roomIndex];
        }

        #endregion // Feature Name Resolution

        #region Passage Name Resolution

        static public int GetPassageIndex(BuildingManifest map, StringHash32 roomName) {
            Assert.NotNullOrDestroyed(map);
            int index = ArrayUtils.IndexOf(map.PassageNames, roomName);
            Assert.True(index >= 0, "Passage '{0}' not present", roomName);
            return index;
        }

        static public StringHash32 GetPassageId(BuildingManifest map, int roomIndex) {
            Assert.NotNullOrDestroyed(map);
            Assert.True(roomIndex >= 0 && roomIndex < map.PassageNames.Length, "Passage index {0} is out of range");
            return map.PassageNames[roomIndex];
        }

        static public PassageInstance GetPassageInstance(BuildingManifest map, int roomIndex) {
            Assert.NotNullOrDestroyed(map);
            Assert.True(roomIndex >= 0 && roomIndex < map.PassageNames.Length, "Passage index {0} is out of range");
            return map.Passages[roomIndex];
        }

        #endregion // Passage Name Resolution
    }
}