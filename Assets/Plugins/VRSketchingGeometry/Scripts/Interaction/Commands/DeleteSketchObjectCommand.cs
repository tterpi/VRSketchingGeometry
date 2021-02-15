using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands
{
    /// <summary>
    /// Delete a sketch object.
    /// </summary>
    public class DeleteObjectCommand : ICommand
    {
        private IGroupable ObjectToDelete = null;
        private SketchWorld SketchWorld;

        public DeleteObjectCommand(IGroupable objectToDelete, SketchWorld sketchWorld)
        {
            this.ObjectToDelete = objectToDelete;
            this.SketchWorld = sketchWorld;
        }

        public bool Execute()
        {
            if (this.ObjectToDelete is MonoBehaviour component)
            {
                SketchWorld.DeleteObject(component.gameObject);
                return true;
            }
            else {
                return false;
            }
        }

        public void Redo()
        {
            this.Execute();
        }

        public void Undo()
        {
            if (this.ObjectToDelete is MonoBehaviour component) {
                SketchWorld.RestoreObject(component.gameObject);
            }
        }
    }
}
