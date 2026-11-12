using System;
using Airborne.Building;
using BeauUtil;
using FieldDay.Collections;
using FieldDay.Data;
using FieldDay.SharedState;

namespace Airborne.Pollutants {
    public sealed class Simulator : SharedStateComponent {
        // max number of connections (max passages * 2) must fit into 128 bits
        private const byte StaticAssert_ConnectionsIs64 = (BuildingConsts.MaxPassages * 2 <= 128 ? 0 : -1);
        // max number of features must fit into 64 bits
        private const byte StaticAssert_FeaturesIs64 = (BuildingConsts.MaxFeatures <= 64 ? 0 : -1);

        public const byte NullIndex = 0xFF;
        
        [NonSerialized] public PollutantLevels[] InitialRoomConditions;
        [NonSerialized] public SimulatedConnectionIndexPair[] PassageToConnectionMap;
        [NonSerialized] public byte[] FeatureToSourceMap;

        [NonSerialized] public SimulatedRoom[] Rooms;
        [NonSerialized] public SimulatedConnection[] Connections;
        [NonSerialized] public SimulatedSource[] Sources;

        [NonSerialized] public BitSet128 EnabledConnections;
        [NonSerialized] public BitSet64 EnabledSources;

        [NonSerialized] public DoubleBuffered<PollutantLevels[]> RoomConditions;
    }

    public struct SimulatedRoom {
        public OffsetLengthU16 Connections;
    }

    public struct SimulatedConnection {
        public byte RoomStart;
        public byte RoomEnd;
        public byte Rank;
        public SimulatedConnectionFlags Flags;
    }

    public struct SimulatedSource {
        public byte RoomIndex;
        public byte PollutantIndex;
        public sbyte Adjustment;
    }

    public enum SimulatedConnectionFlags : byte {
        DeletePollutants = 0x01
    }

    public struct SimulatedConnectionIndexPair {
        public byte ConnectionA;
        public byte ConnectionB;

        static public readonly SimulatedConnectionIndexPair Empty = new SimulatedConnectionIndexPair() {
            ConnectionA = Simulator.NullIndex,
            ConnectionB = Simulator.NullIndex
        };
    }

    public struct SimulationInput {
        public SimulatedRoom[] Rooms;
        public SimulatedConnection[] Connections;
        public SimulatedSource[] Sources;

        public PollutantLevels[] Levels;
        public BitSet128 EnabledConnections;
        public BitSet64 EnabledSources;
    }

    public struct SimulationOutput {
        public PollutantLevels[] Levels;
    }

    static public partial class SimulatorUtility {
        static public void Simulate(in SimulationInput input, ref SimulationOutput output) {
            Unsafe.Clear(output.Levels);
        }
    }

    static public class SimulationConsts {
        [ConfigVar("Max Individual Pollutant Level", Category = "Simulation")] static public int MaxPollutantLevel = 5;
    }
}