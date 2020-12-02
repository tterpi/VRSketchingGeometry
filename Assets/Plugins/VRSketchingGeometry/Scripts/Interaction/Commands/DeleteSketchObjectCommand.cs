using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands
{
    /// <summary>
    /// Delete control point at the end of spline.
    /// </summary>
    public class DeleteSketchObjectCommand : ICommand
    {
        private SketchObject SketchObject = null;
        private SketchWorld SketchWorld;

        public DeleteSketchObjectCommand(SketchObject sketchObjectToDelete, SketchWorld sketchWorld)
        {
            this.SketchObject = sketchObjectToDelete;
            this.SketchWorld = sketchWorld;
        }

        public void Execute()
        {
            SketchWorld.DeleteObject(this.SketchObject.gameObject);
        }

        public void Redo()
        {
            this.Execute();
        }

        public void Undo()
        {
            SketchWorld.RestoreObject(this.SketchObject.gameObject);
        }
    }
}
