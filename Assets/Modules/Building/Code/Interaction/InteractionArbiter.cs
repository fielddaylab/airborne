using BeauUtil;
using FieldDay.Analytics;
using FieldDay.Collections;
using FieldDay.Scripting;
using FieldDay.SharedState;
using Leaf.Runtime;

namespace Airborne.Building {
    /// <summary>
    /// Maintains a queue of interaction requests.
    /// </summary>
    public sealed class InteractionArbiter : SharedStateComponent {
        public Pipe<InteractRequest> Requests = new Pipe<InteractRequest>(4, false);
    }
}