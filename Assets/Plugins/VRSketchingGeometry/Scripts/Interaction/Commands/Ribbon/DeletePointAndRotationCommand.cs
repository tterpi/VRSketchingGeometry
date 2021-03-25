using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Ribbon {
    /// <summary>
    /// Delete the last control point of the ribbon.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
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
            List<Vector3> Points = ribbonSketchObject.GetPoints();
            List<Quaternion> Rotations = ribbonSketchObject.GetRotations();
            Point = Points[Points.Count-1];
            Rotation = Rotations[Rotations.Count - 1];
        }

        public bool Execute()
        {
            this.RibbonSketchObject.DeleteControlPoint();
            if (this.RibbonSketchObject.GetPointsCount() == 0)
            {
                SketchWorld.ActiveSketchWorld.DeleteObject(this.RibbonSketchObject);
            }
            return true;
        }

        public void Redo()
        {
            this.Execute();
        }

        public void Undo()
        {
            if (SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.RibbonSketchObject))
            {
                SketchWorld.ActiveSketchWorld.RestoreObject(this.RibbonSketchObject);
            }
            this.RibbonSketchObject.AddControlPoint(Point, Rotation);
        }
    }
}
