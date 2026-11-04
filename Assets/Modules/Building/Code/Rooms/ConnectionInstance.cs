using System;
using System.Collections;
using System.Collections.Generic;
using FieldDay.Components;
using UnityEngine;

namespace Airborne {
    public sealed class ConnectionInstance : BatchedComponent {
        public RoomInstance Source;
        public RoomInstance Destination;
        public bool IsBidirectional;
    }
}
