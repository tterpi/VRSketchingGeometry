using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Ribbon {
    public class AddPointAndRotationContinuousCommand : ICommand
    {
        private RibbonSketchObject RibbonSketchObject;
        private Vector3 Point;
        private Quaternion Rotation;

        /// <summary>
        /// Create a command that represents adding a new control point to the end of a ribbon sketch object.
        /// </summary>
        /// <param name="ribbonSketchObject">The ribbon to add the control point to.</param>
        /// <param name="point">The point to add.</param>
        /// <param name="rotation">The rotation of the cross section at this point.</param>
        public AddPointAndRotationContinuousCommand(RibbonSketchObject ribbonSketchObject, Vector3 point, Quaternion rotation) {
            this.RibbonSketchObject = ribbonSketchObject;
            Point = point;
            Rotation = rotation;
        }

        public bool Execute()
        {
            return this.RibbonSketchObject.AddControlPointContinuous(Point, Rotation);
        }

        public void Redo()
        {
            this.Execute();
        }

        public void Undo()
        {
            this.RibbonSketchObject.DeleteControlPoint();
        }
    }
}
