using System.Collections;
using System.Collections.Generic;
using FieldDay;
using FieldDay.Debugging;
using FieldDay.Systems;
using UnityEngine;

namespace Airborne
{
    public class TimelineDebugger : SystemComponent
    {
        public override unsafe void RegisterSystems(ref SystemRegistrationTable ecs) {
            ecs.Register(&Update, new SysUpdate(GameLoopPhase.DebugUpdate, 0), default);
        }

        static private void Update(float dt) {
            if (DebugInput.IsPressed(KeyCode.Alpha1)) {

            }
        }

        static private void DebugJumpToTimeIndex(int index) {

        }
    }
}
