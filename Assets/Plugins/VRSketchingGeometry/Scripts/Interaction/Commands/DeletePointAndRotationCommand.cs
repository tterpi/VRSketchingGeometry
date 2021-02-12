using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Ribbon {
    /// <summary>
    /// Delete the last control point of the ribbon.
    /// </summary>
    public class DeletePointAndRotationCommand : ICommand
    {
        private RibbonSketchObject RibbonSketchObject;
        private Vector3 Point;
        private Quaternion Rotation;

        /// <summary>
        /// Create a command that represents deleting the last control point at the end of a ribbon sketch object.
        /// </summary>
        /// <param name="ribbonSketchObject">The ribbon to add the control point to.</param>
        public DeletePointAndRotationCommand(RibbonSketchObject ribbonSketchObject) {
            this.RibbonSketchObject = ribbonSketchObject;
            Point = ribbonSketchObject.Points[ribbonSketchObject.Points.Count-1];
            Rotation = ribbonSketchObject.Rotations[ribbonSketchObject.Rotations.Count - 1];
        }

        public bool Execute()
        {
            this.RibbonSketchObject.DeleteControlPoint();
            return true;
        }

        public void Redo()
        {
            this.Execute();
        }

        public void Undo()
        {
            this.RibbonSketchObject.AddControlPoint(Point, Rotation);
        }
    }
}
