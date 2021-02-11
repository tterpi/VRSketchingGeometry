using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Line {
    /// <summary>
    /// Add control point at the end of spline.
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
            this.Execute();
        }

        public void Undo()
        {
            LineSketchObject.deleteControlPoint();
        }
    }
}
