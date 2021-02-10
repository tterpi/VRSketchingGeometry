using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Commands {
    public interface ICommand
    {
        void Execute();
        void Undo();
        void Redo();
    }
}
