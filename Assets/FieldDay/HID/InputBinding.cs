using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.Collections;
using FieldDay.Data;
using System;
using UnityEngine;

namespace FieldDay.HID {
    public struct InputBinding : IByteSerializable {
        public Int16EnumArray8<KeyCode> Keyboard;

        #region IByteSerializable

        void IByteReadable.ReadFrom(ref ByteReader reader) {
            reader.Read(ref Keyboard);
        }

        void IByteWritable.WriteTo(ref ByteWriter writer) {
            writer.Write(Keyboard);
        }

        #endregion // IByteSerializable
    }
}