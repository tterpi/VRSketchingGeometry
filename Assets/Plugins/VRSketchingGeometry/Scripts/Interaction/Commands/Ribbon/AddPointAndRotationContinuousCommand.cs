using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Ribbon {
    /// <summary>
    /// Add new control point to the end of the ribbon,
    /// if the new point is at least a minimum distance away from the last control point of the ribbon.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
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
            if (SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.RibbonSketchObject))
            {
                SketchWorld.ActiveSketchWorld.RestoreObject(this.RibbonSketchObject);
            }
            this.Execute();
        }

        public void Undo()
        {
            this.RibbonSketchObject.DeleteControlPoint();
            if (this.RibbonSketchObject.GetPointsCount() == 0)
            {
                SketchWorld.ActiveSketchWorld.DeleteObject(this.RibbonSketchObject);
            }
        }
    }
}
