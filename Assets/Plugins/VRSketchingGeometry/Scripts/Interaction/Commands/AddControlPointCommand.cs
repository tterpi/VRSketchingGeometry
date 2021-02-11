using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Line {
    /// <summary>
    /// Add control point at the end of spline.
    /// </summary>
    public class AddControlPointCommand : ICommand
    {
        private LineSketchObject LineSketchObject;
        private Vector3 NewControlPoint;

        public AddControlPointCommand(LineSketchObject lineSketchObject, Vector3 controlPoint) {
            this.LineSketchObject = lineSketchObject;
            this.NewControlPoint = controlPoint;
        }

        public bool Execute()
        {
            LineSketchObject.addControlPoint(NewControlPoint);
            return true;
        }

        public void Redo()
        {
            this.Execute();
        }

        public void Undo()
        {
            LineSketchObject.deleteControlPoint();
        }

        /// <summary>
        /// This will only return a command object if the distance between the previous and new control point is at least minimumDistance.
        /// </summary>
        /// <param name="lineSketchObject"></param>
        /// <param name="point"></param>
        /// <param name="rotation"></param>
        /// <param name="minimumDistanceToLastControlPoint"></param>
        /// <returns>A command or null if the distance is smaller than minimumDistance.</returns>
        public static AddControlPointCommand GetAddPointAndRotationCommandContinuous(LineSketchObject lineSketchObject, Vector3 point, float minimumDistanceToLastControlPoint)
        {
            if ((lineSketchObject.getControlPoints()[lineSketchObject.getNumberOfControlPoints() - 1] - point).magnitude >= minimumDistanceToLastControlPoint)
            {
                return new AddControlPointCommand(lineSketchObject, point);
            }
            else
            {
                return null;
            }
        }
    }
}
