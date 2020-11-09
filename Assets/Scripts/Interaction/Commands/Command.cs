using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Commands {
    public abstract class Command
    {
        public abstract void Execute();
        public abstract void Undo();
        public abstract void Redo();
    }
}
