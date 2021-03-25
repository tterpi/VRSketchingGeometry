using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Commands {
    /// <summary>
    /// A command according to the command pattern.
    /// It supports undo and redo.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public interface ICommand
    {
        /// <summary>
        /// Execute this command.
        /// </summary>
        /// <returns>
        /// Returns true if execution was successful.
        /// Returns false if execution was unsuccessful, command will be discarded in this case.
        /// </returns>
        bool Execute();
        void Undo();
        void Redo();
    }
}
