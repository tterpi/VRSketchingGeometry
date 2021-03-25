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
    public class AddToSelectionAndHighlightCommand : ICommand
    {
        private SketchObjectSelection Selection;
        private SelectableObject SelectableObject;

        public AddToSelectionAndHighlightCommand(SketchObjectSelection selection, SelectableObject selectableObject)
        {
            this.SelectableObject = selectableObject;
            this.Selection = selection;
        }

        public bool Execute()
        {
            this.Selection.AddToSelection(this.SelectableObject);
            this.SelectableObject.highlight();
            return true;
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            this.Selection.RemoveFromSelection(this.SelectableObject);
        }
    }
}