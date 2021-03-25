using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Ribbon {
    /// <summary>
    /// Add a new control point to the end of the ribbon.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class AddPointAndRotationCommand : ICommand
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
        public AddPointAndRotationCommand(RibbonSketchObject ribbonSketchObject, Vector3 point, Quaternion rotation) {
            this.RibbonSketchObject = ribbonSketchObject;
            Point = point;
            Rotation = rotation;
        }

        public bool Execute()
        {
            this.RibbonSketchObject.AddControlPoint(Point, Rotation);
            return true;
        }

        public void Redo()
        {
            if (SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.RibbonSketchObject)) {
                SketchWorld.ActiveSketchWorld.RestoreObject(this.RibbonSketchObject);
            }
            this.Execute();
        }

        public void Undo()
        {
            this.RibbonSketchObject.DeleteControlPoint();
            if (this.RibbonSketchObject.GetPointsCount() == 0) {
                SketchWorld.ActiveSketchWorld.DeleteObject(this.RibbonSketchObject);
            }
        }

        /// <summary>
        /// This will only return a command object if the distance between the previous and new control point is larger than minimumDistance.
        /// </summary>
        /// <param name="ribbonSketchObject"></param>
        /// <param name="point"></param>
        /// <param name="rotation"></param>
        /// <param name="minimumDistanceToLastControlPoint"></param>
        /// <returns>A command or null if the distance is smaller than minimumDistance.</returns>
        public static AddPointAndRotationCommand GetAddPointAndRotationCommandContinuous(RibbonSketchObject ribbonSketchObject, Vector3 point, Quaternion rotation, float minimumDistanceToLastControlPoint){
            if ((ribbonSketchObject.GetPoints()[ribbonSketchObject.GetPoints().Count - 1] - point).magnitude >= minimumDistanceToLastControlPoint)
            {
                return new AddPointAndRotationCommand(ribbonSketchObject, point, rotation);
            }
            else {
                return null;
            }
        }
    }
}
