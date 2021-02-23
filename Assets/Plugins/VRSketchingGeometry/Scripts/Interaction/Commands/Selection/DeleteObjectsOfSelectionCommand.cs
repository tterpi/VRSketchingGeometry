using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Commands;

namespace VRSketchingGeometry.Commands.Selection
{
    /// <summary>
    /// Delete all objects in the selection. Objects are restored and selection is activated when undone.
    /// </summary>
    public class DeleteObjectsOfSelectionCommand : ICommand
    {
        SketchObjectSelection Selection;
        List<GameObject> selectedObjects;

        public DeleteObjectsOfSelectionCommand(SketchObjectSelection selection)
        {
            this.Selection = selection;
            this.selectedObjects = selection.GetObjectsOfSelection();
        }

        public bool Execute()
        {
            this.Selection.DeleteObjectsOfSelection();
            return true;
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            foreach (GameObject selectedObject in selectedObjects)
            {
                SketchWorld.ActiveSketchWorld.RestoreObject(selectedObject);
            }
            this.Selection.Activate();
        }
    }
}