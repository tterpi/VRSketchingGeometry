using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands
{
    /// <summary>
    /// Delete a sketch object.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class DeleteObjectCommand : ICommand
    {
        private SelectableObject ObjectToDelete = null;
        private SketchWorld SketchWorld;

        public DeleteObjectCommand(SelectableObject objectToDelete, SketchWorld sketchWorld)
        {
            this.ObjectToDelete = objectToDelete;
            this.SketchWorld = sketchWorld;
        }

        public bool Execute()
        {
            SketchWorld.DeleteObject(ObjectToDelete);
            return true;
        }

        public void Redo()
        {
            this.Execute();
        }

        public void Undo()
        {
            SketchWorld.RestoreObject(ObjectToDelete);
        }
    }
}
