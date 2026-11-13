using System;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.Components;
using FieldDay.Scripting;
using ScriptableBake;
using UnityEngine;

namespace Airborne.Building {
    public sealed class RoomInstance : ScriptActorComponent, IBaked {
        [HideInInspector] public StringHash32 Id;
        [HideInInspector] public ActionPost[] Posts;
        [HideInInspector] public int Index;
        [HideInInspector] public BitSet64 PassageMask;

#if UNITY_EDITOR

        int IBaked.Order { get { return -1001; } }

        bool IBaked.Bake(BakeFlags flags, BakeContext context) {
            Id = StringHash32.First(ScriptUtility.ActorId(this), name);
            Posts = GetComponentsInChildren<ActionPost>(true);
            Array.Sort(Posts, (a, b) => a.Id.CompareTo(b.Id));
            for(int i = 0; i < Posts.Length; i++) {
                Posts[i].Index = i;
            }
            PassageMask = default;
            return true;
        }

#endif // UNITY_EDITOR
    }

    static public partial class BuildingLookup {
        #region Post Name Resolution

        static public int GetPostIndex(RoomInstance room, StringHash32 postName) {
            Assert.NotNullOrDestroyed(room);
            for(int i = 0; i < room.Posts.Length; i++) {
                var post = room.Posts[i];
                if (post.Id == postName) {
                    return i;
                }
            }
            Assert.Fail("Post '{0}' not present in room '{1}'", postName, room.Id);
            return -1;
        }

        static public StringHash32 GetPostName(RoomInstance room, int postIndex) {
            Assert.NotNullOrDestroyed(room);
            Assert.True(postIndex >= 0 && postIndex < room.Posts.Length, "Post index {0} is out of range");
            return room.Posts[postIndex].Id;
        }

        static public ActionPost GetPostInstance(RoomInstance room, int postIndex) {
            Assert.NotNullOrDestroyed(room);
            Assert.True(postIndex >= 0 && postIndex < room.Posts.Length, "Post index {0} is out of range");
            return room.Posts[postIndex];
        }

        #endregion // Post Name Resolution
    }
}