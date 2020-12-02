using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands
{
    /// <summary>
    /// Delete control point at the end of spline.
    /// </summary>
    public class CreateSketchObjectCommand : ICommand
    {
        private SketchObject SketchObject = null;
        private SketchWorld SketchWorld;

        public CreateSketchObjectCommand(SketchObject newSketchObject, SketchWorld sketchWorld)
        {
            this.SketchObject = newSketchObject;
            this.SketchWorld = sketchWorld;
        }

        public void Execute()
        {
            SketchWorld.AddObject(this.SketchObject.gameObject);
        }

        public void Redo()
        {
            SketchWorld.RestoreObject(this.SketchObject.gameObject);
        }

        public void Undo()
        {
            SketchWorld.DeleteObject(this.SketchObject.gameObject);
        }
    }
}
