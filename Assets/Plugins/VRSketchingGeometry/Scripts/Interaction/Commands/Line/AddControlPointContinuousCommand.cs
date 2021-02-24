using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Line {
    /// <summary>
    /// Add control point at the end of spline if it is at least a certain distance away from the last control point.
    /// </summary>
    public class AddControlPointContinuousCommand : ICommand
    {
        private LineSketchObject LineSketchObject;
        private Vector3 NewControlPoint;

        public AddControlPointContinuousCommand(LineSketchObject lineSketchObject, Vector3 controlPoint) {
            this.LineSketchObject = lineSketchObject;
            this.NewControlPoint = controlPoint;
        }

        public bool Execute()
        {
            return LineSketchObject.addControlPointContinuous(NewControlPoint);
        }

        public void Redo()
        {
            if (SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.LineSketchObject))
            {
                SketchWorld.ActiveSketchWorld.RestoreObject(this.LineSketchObject);
            }
            LineSketchObject.addControlPoint(NewControlPoint);
        }

        public void Undo()
        {
            LineSketchObject.deleteControlPoint();
            if (this.LineSketchObject.getNumberOfControlPoints() == 0)
            {
                SketchWorld.ActiveSketchWorld.DeleteObject(this.LineSketchObject);
            }
        }
    }
}
