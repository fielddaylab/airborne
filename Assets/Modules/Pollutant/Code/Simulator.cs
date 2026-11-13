using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Airborne.Building;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.Collections;
using FieldDay.Data;
using FieldDay.SharedState;

namespace Airborne.Pollutants {
    public sealed class Simulator : SharedStateComponent {
        // max number of connections (max passages * 2) must fit into 64 bits
        private const byte StaticAssert_ConnectionsIs64 = (BuildingConsts.MaxPassages * 2 <= 64 ? 0 : -1);

        public const byte NullIndex = 0xFF;
        
        [NonSerialized] public PollutantLevels[] InitialRoomConditions;
        [NonSerialized] public SimulatedConnectionIndexPair[] PassageToConnectionMap;
        [NonSerialized] public int PollutantTypeCount;

        [NonSerialized] public SimulatedRoomNode[] Rooms;
        [NonSerialized] public SimulatedConnection[] Connections;
        [NonSerialized] public byte[] DiffusionOrder;

        [NonSerialized] public WorkList<SimulatedSource> SourceQueue;

        [NonSerialized] public BitSet64 EnabledConnections;

        [NonSerialized] public DoubleBuffered<PollutantLevels[]> RoomConditions;
    }

    public struct SimulatedRoomNode {
        public byte ConnectionsOffset;
        public byte ConnectionsLength;
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

    public struct SimulationGraph {
        public SimulatedRoomNode[] Rooms;
        public SimulatedConnection[] Connections;
        public byte[] DiffusionOrder;
        public WorkList<SimulatedSource> Sources;

        public BitSet64 EnabledConnections;
        public int RoomCount;
        public int PollutantTypeCount;
    }

    public struct SimulationBuffers {
        public PollutantLevels[] InputLevels;
        public PollutantLevels[] OutputLevels;
    }

    static public partial class SimulatorUtility {
        #region Construction

        /// <summary>
        /// Allocates buffers for handling pollutants.
        /// </summary>
        static public void AllocatePollutantBuffers(Simulator simulator, int roomCount, int featureCount, int pollutantCount) {
            Assert.NotNullOrDestroyed(simulator);
            Assert.True(simulator.InitialRoomConditions == null, "Initial states have already been allocated");

            simulator.PollutantTypeCount = pollutantCount;
            simulator.InitialRoomConditions = new PollutantLevels[roomCount];

            simulator.RoomConditions.Current = new PollutantLevels[roomCount];
            simulator.RoomConditions.Back = new PollutantLevels[roomCount];

            simulator.SourceQueue = new WorkList<SimulatedSource>(Unsafe.AlignUp32(featureCount + 16));
        }

        /// <summary>
        /// Allocates the simulation graph.
        /// </summary>
        static public unsafe void AllocateGraph(Simulator simulator, BuildingManifest manifest) {
            Assert.NotNullOrDestroyed(simulator);
            Assert.True(simulator.Rooms == null, "Graph has already been allocated");

            BuildingManifestCounts counts = BuildingLookup.CountObjects(manifest);

            simulator.Rooms = new SimulatedRoomNode[counts.RoomCount];
            simulator.PassageToConnectionMap = new SimulatedConnectionIndexPair[counts.PassageCount];

            for(int i = 0; i < counts.PassageCount; i++) {
                simulator.PassageToConnectionMap[i] = SimulatedConnectionIndexPair.Empty;
            }

            TempConnection* tempConnectionBuffer = stackalloc TempConnection[counts.PassageCount * 2];
            int tempConnectionsAllocated = 0;

            for(int i = 0; i < counts.PassageCount; i++) {
                PassageInstance passage = manifest.Passages[i];
                if (passage.Source == null) {
                    Log.Warn("[SimulatorUtility] Passage '{0}' has null source!", passage.name);
                } else {
                    ref TempConnection writeConnection = ref tempConnectionBuffer[tempConnectionsAllocated++];
                    writeConnection.RoomStart = (byte) passage.Source.Index;
                    writeConnection.Rank = (byte) passage.Rank;
                    writeConnection.PassageIndex = (byte) i;
                    writeConnection.ConnectionIndex = (byte) (tempConnectionsAllocated - 1);

                    if (passage.Target == null) {
                        writeConnection.RoomEnd = Simulator.NullIndex;
                        writeConnection.Flags |= SimulatedConnectionFlags.DeletePollutants;
                        continue;
                    }

                    writeConnection.RoomEnd = (byte) passage.Target.Index;
                    writeConnection.Flags = default;

                    if (!passage.IsBidirectional) {
                        continue;
                    }

                    ref TempConnection flippedConnection = ref tempConnectionBuffer[tempConnectionsAllocated++];
                    flippedConnection = writeConnection;
                    flippedConnection.RoomStart = writeConnection.RoomEnd;
                    flippedConnection.RoomEnd = writeConnection.RoomStart;
                    flippedConnection.ConnectionIndex = (byte) (tempConnectionsAllocated - 1);
                }
            }

            simulator.Connections = new SimulatedConnection[tempConnectionsAllocated];
            simulator.DiffusionOrder = new byte[tempConnectionsAllocated];

            Assert.True(tempConnectionsAllocated <= 256, "More than 256 connections!");

            if (tempConnectionsAllocated > 0) {
                // sort to group by room and order by rank
                Unsafe.Quicksort(tempConnectionBuffer, tempConnectionsAllocated, &TempConnection.CompareGroupRank);

                // partition and copy temp connections into output buffer
                fixed (SimulatedConnection* connectionOutput = simulator.Connections) {
                    SimulatedConnection* writeHead = connectionOutput;
                    TempConnection* readHead = tempConnectionBuffer;

                    int groupHead = -1;
                    int groupCount = 0;
                    int currentRoom = -1;
                    for(int connectionIndex = 0; connectionIndex < tempConnectionsAllocated; connectionIndex++) {
                        TempConnection temp = *readHead;
                        // remap instantiation index to connection index, for use later
                        (*readHead++).ConnectionIndex = (byte) connectionIndex;
                        *writeHead++ = *(SimulatedConnection*) (&temp);

                        if (currentRoom != temp.RoomStart) {
                            Assert.True(temp.RoomStart > currentRoom, "Rooms are not sorted properly!");

                            // crossed boundary into next room, write out group info
                            if (groupCount > 0) {
                                Assert.True(currentRoom >= 0);
                                ref SimulatedRoomNode writeRoom = ref simulator.Rooms[currentRoom];
                                writeRoom.ConnectionsOffset = (byte) groupHead;
                                writeRoom.ConnectionsLength = (byte) groupCount;
                            }
                            currentRoom = temp.RoomStart;
                            groupHead = connectionIndex;
                            groupCount = 1;
                        } else {
                            groupCount++;
                        }

                        // write index into passage->connection pair map
                        WriteConnectionIndexToPassageMap(simulator.PassageToConnectionMap, (byte) connectionIndex, temp);
                    }

                    // write final group
                    if (groupCount > 0) {
                        Assert.True(currentRoom >= 0);
                        ref SimulatedRoomNode writeRoom = ref simulator.Rooms[currentRoom];
                        writeRoom.ConnectionsOffset = (byte) groupHead;
                        writeRoom.ConnectionsLength = (byte) groupCount;
                    }
                }

                // order by rank
                Unsafe.Quicksort(tempConnectionBuffer, tempConnectionsAllocated, &TempConnection.CompareRank);

                for(int i = 0; i < tempConnectionsAllocated; i++) {
                    simulator.DiffusionOrder[i] = tempConnectionBuffer[i].ConnectionIndex;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TempConnection {
            // byte-identical to SimulatedConnection
            public byte RoomStart;
            public byte RoomEnd;
            public byte Rank;
            public SimulatedConnectionFlags Flags;

            // for sorting and writing connection indices back
            public byte PassageIndex;
            public byte ConnectionIndex;

            static public int CompareGroupRank(TempConnection a, TempConnection b) {
                int roomCompare = a.RoomStart - b.RoomStart;
                if (roomCompare != 0) {
                    return roomCompare;
                }

                int rankCompare = a.Rank - b.Rank;
                if (rankCompare != 0) {
                    return rankCompare;
                }

                int endCompare = a.RoomEnd - b.RoomEnd;
                if (endCompare != 0) {
                    return endCompare;
                }

                return a.ConnectionIndex - b.ConnectionIndex;
            }

            static public int CompareRank(TempConnection a, TempConnection b) {
                int rankCompare = a.Rank - b.Rank;
                if (rankCompare != 0) {
                    return rankCompare;
                }

                int roomCompare = a.RoomStart - b.RoomStart;
                if (roomCompare != 0) {
                    return roomCompare;
                }

                int endCompare = a.RoomEnd - b.RoomEnd;
                if (endCompare != 0) {
                    return endCompare;
                }

                return a.ConnectionIndex - b.ConnectionIndex;
            }
        }

        static private void WriteConnectionIndexToPassageMap(SimulatedConnectionIndexPair[] pairs, byte connectionIndex, in TempConnection connection) {
            ref SimulatedConnectionIndexPair pair = ref pairs[connection.PassageIndex];
            if (pair.ConnectionA == Simulator.NullIndex) {
                pair.ConnectionA = connectionIndex;
            } else {
                Assert.True(pair.ConnectionB == Simulator.NullIndex, "Cannot assign more than two connections per passage");
                pair.ConnectionB = connectionIndex;
            }
        }

        #endregion // Construction

        #region Initial State

        /// <summary>
        /// Resets initial states to 0.
        /// </summary>
        static public void ResetInitialStates(Simulator simulator) {
            Assert.NotNullOrDestroyed(simulator);
            Unsafe.Clear(simulator.InitialRoomConditions);
        }

        /// <summary>
        /// Copies the initial room states into the current simulation state.
        /// </summary>
        static public unsafe void ApplyInitialStates(Simulator simulator) {
            Assert.NotNullOrDestroyed(simulator);

            int roomCount = simulator.InitialRoomConditions.Length;

            fixed(PollutantLevels* initialPtr = simulator.InitialRoomConditions)
            fixed(PollutantLevels* targetPtr = simulator.RoomConditions.Current) {
                Unsafe.FastCopyArray(initialPtr, roomCount, targetPtr);
            }
        }

        #endregion // Initial State

        #region Simulation

        /// <summary>
        /// Runs the simulation.
        /// </summary>
        static public unsafe void Simulate(Simulator simulator) {
            Assert.NotNullOrDestroyed(simulator);

            SimulationBuffers buffers;
            buffers.OutputLevels = simulator.RoomConditions.Next();
            buffers.InputLevels = simulator.RoomConditions.Back;

            SimulationGraph input;
            input.Rooms = simulator.Rooms;
            input.Connections = simulator.Connections;
            input.DiffusionOrder = simulator.DiffusionOrder;
            input.Sources = simulator.SourceQueue;
            input.EnabledConnections = simulator.EnabledConnections;
            input.PollutantTypeCount = simulator.PollutantTypeCount;
            input.RoomCount = simulator.Rooms.Length;

            StepSimulation(input, ref buffers);

            // TODO: clean up
        }

        /// <summary>
        /// Steps the simulation forward one tick.
        /// </summary>
        static private unsafe void StepSimulation(in SimulationGraph graph, ref SimulationBuffers buffers) {
            Unsafe.Clear(buffers.OutputLevels);

            fixed(PollutantLevels* outputLevelPtr = buffers.OutputLevels)
            fixed(PollutantLevels* inputLevelPtr = buffers.InputLevels) {
                Unsafe.FastCopyArray(inputLevelPtr, graph.RoomCount, outputLevelPtr);
            }

            // diffuse, emit, overflow
            PerformDiffusion(graph, ref buffers);
            EmitFromSources(buffers.OutputLevels, graph.Sources);
            HandleOverflows(graph, ref buffers);
        }

        static private unsafe void PerformDiffusion(in SimulationGraph graph, ref SimulationBuffers buffers) {
            using (Profiling.Time("Diffusion simulation", ProfileTimeUnits.Microseconds)) {
                // walk the graph in diffusion order (by rank, then room)
                for (int i = 0; i < graph.DiffusionOrder.Length; i++) {
                    int connectionIndex = graph.DiffusionOrder[i];
                    if (!graph.EnabledConnections.IsSet(connectionIndex)) {
                        continue;
                    }

                    SimulatedConnection connection = graph.Connections[connectionIndex];

                    // when transferring, remove from input AND output
                    // add ONLY to output
                    // this will keep pollutants from skipping over nodes in one step

                    ref PollutantLevels srcLevels = ref buffers.InputLevels[connection.RoomStart];
                    ref PollutantLevels srcLevelsOutput = ref buffers.OutputLevels[connection.RoomStart];
                    
                    PollutantLevels dstLevelsReadOnly = buffers.InputLevels[connection.RoomEnd];
                    ref PollutantLevels dstLevelsOutput = ref buffers.OutputLevels[connection.RoomEnd];

                    for(int p = 0; p < graph.PollutantTypeCount; p++) {
                        if (srcLevels[p] >= dstLevelsReadOnly[p] + SimulationConsts.DiffusionThreshold) {
                            srcLevels[p] -= 1;
                            srcLevelsOutput[p] -= 1;
                            dstLevelsOutput[p] += 1;
                        }
                    }
                }
            }
        }

        static private unsafe void EmitFromSources(PollutantLevels[] roomLevels, WorkList<SimulatedSource> sources) {
            int sourceCount = sources.Count;
            fixed (SimulatedSource* sourcePtr = sources.InternalArray) {
                SimulatedSource* head = sourcePtr;

                // apply changes
                while (sourceCount-- > 0) {
                    SimulatedSource source = *head++;
                    ref byte level = ref roomLevels[source.RoomIndex][source.PollutantIndex];
                    level = (byte) Math.Max(level + source.Adjustment, 0);
                    Assert.True(level <= 32);
                }
            }
        }

        static private unsafe void HandleOverflows(in SimulationGraph graph, ref SimulationBuffers buffers) {
            
        }

        #endregion // Simulation
    }

    static public class SimulationConsts {
        public const int MaxPollutantLevel = 4;
        public const int DiffusionThreshold = 2;
    }
}