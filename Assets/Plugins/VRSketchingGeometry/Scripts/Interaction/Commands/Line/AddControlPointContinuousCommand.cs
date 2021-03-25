using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Line {
    /// <summary>
    /// Add control point at the end of spline if it is at least a certain distance away from the last control point.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
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
            return LineSketchObject.AddControlPointContinuous(NewControlPoint);
        }

        public void Redo()
        {
            if (SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.LineSketchObject))
            {
                SketchWorld.ActiveSketchWorld.RestoreObject(this.LineSketchObject);
            }
            LineSketchObject.AddControlPoint(NewControlPoint);
        }

        public void Undo()
        {
            LineSketchObject.DeleteControlPoint();
            if (this.LineSketchObject.getNumberOfControlPoints() == 0)
            {
                SketchWorld.ActiveSketchWorld.DeleteObject(this.LineSketchObject);
            }
        }
    }
}
