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
            BuildNameTable(context.Scene, BuildingConsts.MaxRooms, out RoomNames, out Rooms, (a) => a.Id, (a, i) => a.Index = i);
            BuildNameTable(context.Scene, BuildingConsts.MaxResidents, out ResidentNames, out Residents, (a) => a.Id, (a, i) => a.Index = i);
            BuildNameTable(context.Scene, BuildingConsts.MaxFeatures, out FeatureNames, out Features, (a) => a.Id, (a, i) => a.Index = i);
            BuildNameTable(context.Scene, BuildingConsts.MaxPassages, out PassageNames, out Passages, (a) => a.Id, (a, i) => a.Index = i);

            foreach(var passage in Passages) {
                if (passage.Source) {
                    passage.Source.PassageMask.Set(passage.Index);
                }
                if (passage.Target) {
                    passage.Target.PassageMask.Set(passage.Index);
                }
            }

            return true;
        }

        static private void BuildNameTable<T>(Scene scene, int maxCount, out StringHash32[] names, out T[] references, Func<T, StringHash32> idFunc, Action<T, int> indexHandler) where T : Component {
            List<T> components = new List<T>();
            scene.GetAllComponents<T>(true, components);
            components.Sort((a, b) => idFunc(a).CompareTo(idFunc(b)));

            if (components.Count > maxCount) {
                Assert.Fail("[BuildingManifest] Too many elements of type '{0}' present - {1} vs max {2}", typeof(T).FullName, components.Count, maxCount);
            }

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

    public struct BuildingManifestCounts {
        public int RoomCount;
        public int ResidentCount;
        public int FeatureCount;
        public int PassageCount;
    }

    static public partial class BuildingLookup {
        #region Counts

        static public BuildingManifestCounts CountObjects(BuildingManifest manifest) {
            BuildingManifestCounts counts;
            counts.RoomCount = manifest.Rooms.Length;
            counts.ResidentCount = manifest.Residents.Length;
            counts.FeatureCount = manifest.Features.Length;
            counts.PassageCount = manifest.Passages.Length;
            return counts;
        }

        #endregion // Counts

        #region Room Name Resolution

        static public int GetRoomIndex(BuildingManifest manifest, StringHash32 roomName) {
            Assert.NotNullOrDestroyed(manifest);
            int index = ArrayUtils.IndexOf(manifest.RoomNames, roomName);
            Assert.True(index >= 0, "Room '{0}' not present", roomName);
            return index;
        }

        static public StringHash32 GetRoomId(BuildingManifest manifest, int roomIndex) {
            Assert.NotNullOrDestroyed(manifest);
            Assert.True(roomIndex >= 0 && roomIndex < manifest.RoomNames.Length, "Room index {0} is out of range");
            return manifest.RoomNames[roomIndex];
        }

        static public RoomInstance GetRoomInstance(BuildingManifest manifest, int roomIndex) {
            Assert.NotNullOrDestroyed(manifest);
            Assert.True(roomIndex >= 0 && roomIndex < manifest.RoomNames.Length, "Room index {0} is out of range");
            return manifest.Rooms[roomIndex];
        }

        #endregion // Room Name Resolution

        #region Resident Name Resolution

        static public int GetResidentIndex(BuildingManifest manifest, StringHash32 roomName) {
            Assert.NotNullOrDestroyed(manifest);
            int index = ArrayUtils.IndexOf(manifest.ResidentNames, roomName);
            Assert.True(index >= 0, "Resident '{0}' not present", roomName);
            return index;
        }

        static public StringHash32 GetResidentId(BuildingManifest manifest, int roomIndex) {
            Assert.NotNullOrDestroyed(manifest);
            Assert.True(roomIndex >= 0 && roomIndex < manifest.ResidentNames.Length, "Resident index {0} is out of range");
            return manifest.ResidentNames[roomIndex];
        }

        static public Resident GetResidentInstance(BuildingManifest manifest, int roomIndex) {
            Assert.NotNullOrDestroyed(manifest);
            Assert.True(roomIndex >= 0 && roomIndex < manifest.ResidentNames.Length, "Resident index {0} is out of range");
            return manifest.Residents[roomIndex];
        }

        #endregion // Resident Name Resolution

        #region Feature Name Resolution

        static public int GetFeatureIndex(BuildingManifest manifest, StringHash32 roomName) {
            Assert.NotNullOrDestroyed(manifest);
            int index = ArrayUtils.IndexOf(manifest.FeatureNames, roomName);
            Assert.True(index >= 0, "Feature '{0}' not present", roomName);
            return index;
        }

        static public StringHash32 GetFeatureId(BuildingManifest manifest, int roomIndex) {
            Assert.NotNullOrDestroyed(manifest);
            Assert.True(roomIndex >= 0 && roomIndex < manifest.FeatureNames.Length, "Feature index {0} is out of range");
            return manifest.FeatureNames[roomIndex];
        }

        static public FeatureInstance GetFeatureInstance(BuildingManifest manifest, int roomIndex) {
            Assert.NotNullOrDestroyed(manifest);
            Assert.True(roomIndex >= 0 && roomIndex < manifest.FeatureNames.Length, "Feature index {0} is out of range");
            return manifest.Features[roomIndex];
        }

        #endregion // Feature Name Resolution

        #region Passage Name Resolution

        static public int GetPassageIndex(BuildingManifest manifest, StringHash32 roomName) {
            Assert.NotNullOrDestroyed(manifest);
            int index = ArrayUtils.IndexOf(manifest.PassageNames, roomName);
            Assert.True(index >= 0, "Passage '{0}' not present", roomName);
            return index;
        }

        static public StringHash32 GetPassageId(BuildingManifest manifest, int roomIndex) {
            Assert.NotNullOrDestroyed(manifest);
            Assert.True(roomIndex >= 0 && roomIndex < manifest.PassageNames.Length, "Passage index {0} is out of range");
            return manifest.PassageNames[roomIndex];
        }

        static public PassageInstance GetPassageInstance(BuildingManifest manifest, int roomIndex) {
            Assert.NotNullOrDestroyed(manifest);
            Assert.True(roomIndex >= 0 && roomIndex < manifest.PassageNames.Length, "Passage index {0} is out of range");
            return manifest.Passages[roomIndex];
        }

        #endregion // Passage Name Resolution
    }
}