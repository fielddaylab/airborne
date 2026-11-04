using BeauUtil;
using BeauUtil.Debugger;
using System;
using System.Runtime.CompilerServices;

namespace FieldDay.Collections {
    public struct Int8EnumArray8<T> where T : unmanaged, Enum {
        public unsafe fixed sbyte RawValues[8];

        public unsafe ref T this[int index] {
            get {
                Assert.True(index >= 0 && index < 8, "Index out of range");
                return ref Unsafe.FastCast<sbyte, T>(ref RawValues[index]);
            }
        }

        public readonly int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return 8; }
        }
    }

    public struct Int16EnumArray8<T> where T : unmanaged, Enum {
        public unsafe fixed short RawValues[8];

        public unsafe ref T this[int index] {
            get {
                Assert.True(index >= 0 && index < 8, "Index out of range");
                return ref Unsafe.FastCast<short, T>(ref RawValues[index]);
            }
        }

        public readonly int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return 8; }
        }
    }

    public struct Int32EnumArray8<T> where T : unmanaged, Enum {
        public unsafe fixed int RawValues[8];

        public unsafe ref T this[int index] {
            get {
                Assert.True(index >= 0 && index < 8, "Index out of range");
                return ref Unsafe.FastCast<int, T>(ref RawValues[index]);
            }
        }

        public readonly int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return 8; }
        }
    }

    public struct Int64EnumArray8<T> where T : unmanaged, Enum {
        public unsafe fixed long RawValues[8];

        public unsafe ref T this[int index] {
            get {
                Assert.True(index >= 0 && index < 8, "Index out of range");
                return ref Unsafe.FastCast<long, T>(ref RawValues[index]);
            }
        }

        public readonly int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return 8; }
        }
    }
}