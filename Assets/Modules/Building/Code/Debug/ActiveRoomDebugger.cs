using Airborne.Building;
using FieldDay;
using FieldDay.Debugging;
using FieldDay.Systems;
using UnityEngine;

namespace Airborne 
{
    public class ActiveRoomDebugger : SystemComponent 
    {
        public override unsafe void RegisterSystems(ref SystemRegistrationTable ecs) {
            ecs.Register(&Update, new SysUpdate(GameLoopPhase.DebugUpdate, 0), default);
        }
        
        static private void Update(float dt) {
            if (DebugInput.IsPressed(KeyCode.P)) {
                IncrementActiveRoom();
            }
        }
        
        private static void IncrementActiveRoom() {
            Find.State(out ActiveRoom state, out BuildingManifest building);
            int prevRoom = state.ActiveRoomIndex;
            
            state.ActiveRoomIndex = (state.ActiveRoomIndex + 1) % building.Rooms.Length;
            
            BuildingUtility.EnableRoom(BuildingLookup.GetRoomInstance(building, state.ActiveRoomIndex));
            BuildingUtility.DisableRoom(BuildingLookup.GetRoomInstance(building, prevRoom));
        }

        private static void DecrementActiveRoom() {
            
        }
    }
}