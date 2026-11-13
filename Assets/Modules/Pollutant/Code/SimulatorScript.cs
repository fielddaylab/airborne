using Airborne.Building;
using Airborne.Pollutants;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Scripting;
using Leaf.Runtime;

namespace Airborne.Pollutants {
    static public class SimulatorScript {
        #region Pollutants

        [LeafMember("SetInitialPollutant")]
        static public void SetInitialPollutant(StringHash32 roomName, StringHash32 pollutantName, int level) {
            Find.State(out Simulator simulator, out BuildingScenario scenario, out BuildingManifest manifest);
            Assert.True(simulator.InitialRoomConditions != null);

            int roomIndex = BuildingLookup.GetRoomIndex(manifest, roomName);
            int pollutantIndex = ScenarioUtility.GetPollutantIndex(scenario, pollutantName);

            Assert.True(level >= 0 && level < SimulationConsts.MaxPollutantLevel, "Pollutant level {0} out of range", level);
            simulator.InitialRoomConditions[roomIndex][pollutantIndex] = (byte) level;
        }

        #endregion // Pollutants
    }
}