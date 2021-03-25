using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Commands;

namespace VRSketchingGeometry.Commands.Selection
{
    /// <summary>
    /// Activate the selection.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class ActivateSelectionCommand : ICommand
    {
        SketchObjectSelection Selection;

        public ActivateSelectionCommand(SketchObjectSelection selection)
        {
            this.Selection = selection;
        }

        public bool Execute()
        {
            this.Selection.Activate();
            return true;
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            this.Selection.Deactivate();
        }
    }
}