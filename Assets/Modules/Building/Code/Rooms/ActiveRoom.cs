using FieldDay.SharedState;
using UnityEngine;

namespace Airborne.Building 
{
    public class ActiveRoom : SharedStateComponent {
        [HideInInspector] public int ActiveRoomIndex;
    }
    
    static public partial class BuildingUtility {
        static public void EnableRoom(RoomInstance room) {
            for (int i = 0; i < room.RenderElements.Length; i++) {
                room.RenderElements[i].OnRoomEnabled();
            }
        }

        static public void DisableRoom(RoomInstance room) {
            for (int i = 0; i < room.RenderElements.Length; i++) {
                room.RenderElements[i].OnRoomDisabled();
            }
        }

        // todo: figure out how to init this (also need to determine where a 'starting room index' variable lives)
        static public void InitActiveRoom(RoomInstance[] allRooms, RoomInstance activeRoom) {
            for (int i = 0; i < allRooms.Length; i++) {
                if (allRooms[i].Id == activeRoom.Id) EnableRoom(allRooms[i]);
                else DisableRoom(allRooms[i]);
            }
        }
    }
}