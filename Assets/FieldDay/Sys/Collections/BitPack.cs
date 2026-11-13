using System.Runtime.CompilerServices;
using TinyIL;

namespace FieldDay.Collections {
    /// <summary>
    /// Bit packing and unpacking utility methods.
    /// </summary>
    static public class BitPack {
        #region Combine

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public ulong Combine(ulong a, ulong b) {
            return a | b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public ulong Combine(ulong a, ulong b, ulong c) {
            return a | b | c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public ulong Combine(ulong a, ulong b, ulong c, ulong d) {
            return a | b | c | d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public ulong Combine(ulong a, ulong b, ulong c, ulong d, ulong e) {
            return a | b | c | d | e;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public uint Combine(uint a, uint b) {
            return a | b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public uint Combine(uint a, uint b, uint c) {
            return a | b | c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public uint Combine(uint a, uint b, uint c, uint d) {
            return a | b | c | d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public uint Combine(uint a, uint b, uint c, uint d, uint e) {
            return a | b | c | d | e;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public ushort Combine(ushort a, ushort b) {
            return (ushort) (a | b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public ushort Combine(ushort a, ushort b, ushort c) {
            return (ushort) (a | b | c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public ushort Combine(ushort a, ushort b, ushort c, ushort d) {
            return (ushort) (a | b | c | d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public ushort Combine(ushort a, ushort b, ushort c, ushort d, ushort e) {
            return (ushort) (a | b | c | d | e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public byte Combine(byte a, byte b) {
            return (byte)(a | b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public byte Combine(byte a, byte b, byte c) {
            return (byte)(a | b | c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public byte Combine(byte a, byte b, byte c, byte d) {
            return (byte)(a | b | c | d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public byte Combine(byte a, byte b, byte c, byte d, byte e) {
            return (byte)(a | b | c | d | e);
        }

        #endregion // Combine

        #region Pack

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [IntrinsicIL("ldc.i4.1; ldarg.1; shl; ldc.i4.1; sub; ldarg.0; and; ldarg.2; shl; ret;")]
        static public ulong Pack64(ulong data, int bitWidth, int offset) {
            ulong mask = (1u << bitWidth) - 1;
            return ((data & mask) << offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [IntrinsicIL("ldc.i4.1; ldarg.1; shl; ldc.i4.1; sub; ldarg.0; and; ldarg.2; shl; ret;")]
        static public uint Pack32(uint data, int bitWidth, int offset) {
            uint mask = (1u << bitWidth) - 1;
            return ((data & mask) << offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [IntrinsicIL("ldc.i4.1; ldarg.1; shl; ldc.i4.1; sub; ldarg.0; and; ldarg.2; shl; conv.u2; ret;")]
        static public ushort Pack16(ushort data, int bitWidth, int offset) {
            uint mask = (1u << bitWidth) - 1;
            return (ushort)(((data & mask) << offset));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [IntrinsicIL("ldc.i4.1; ldarg.1; shl; ldc.i4.1; sub; ldarg.0; and; ldarg.2; shl; conv.u1; ret;")]
        static public byte Pack8(byte data, int bitWidth, int offset) {
            uint mask = (1u << bitWidth) - 1;
            return (byte)(((data & mask) << offset));
        }

        #endregion // Pack

        #region Unpack

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [IntrinsicIL("ldarg.0; ldarg.2; shr; ldc.i4.1; ldarg.1; shl; ldc.i4.1; sub; and; ret;")]
        static public ulong Unpack64(ulong rawValue, int bitWidth, int offset) {
            ulong mask = (1u << bitWidth) - 1;
            return (rawValue >> offset) & mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [IntrinsicIL("ldarg.0; ldarg.2; shr; ldc.i4.1; ldarg.1; shl; ldc.i4.1; sub; and; ret;")]
        static public uint Unpack32(uint rawValue, int bitWidth, int offset) {
            uint mask = (1u << bitWidth) - 1;
            return (rawValue >> offset) & mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [IntrinsicIL("ldarg.0; ldarg.2; shr; ldc.i4.1; ldarg.1; shl; ldc.i4.1; sub; and; conv.u2; ret;")]
        static public ushort Unpack16(ushort rawValue, int bitWidth, int offset) {
            uint mask = (1u << bitWidth) - 1;
            return (ushort) ((rawValue >> offset) & mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [IntrinsicIL("ldarg.0; ldarg.2; shr; ldc.i4.1; ldarg.1; shl; ldc.i4.1; sub; and; conv.u1; ret;")]
        static public byte Unpack8(byte rawValue, int bitWidth, int offset) {
            uint mask = (1u << bitWidth) - 1;
            return (byte)((rawValue >> offset) & mask);
        }

        #endregion // Unpack
    }
}