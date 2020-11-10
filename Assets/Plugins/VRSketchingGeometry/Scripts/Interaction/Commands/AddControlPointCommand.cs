using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands {
    /// <summary>
    /// Add control point at the end of spline.
    /// </summary>
    public class AddControlPointCommand : Command
    {
        private LineSketchObject LineSketchObject;
        private Vector3 NewControlPoint;

        public AddControlPointCommand(LineSketchObject lineSketchObject, Vector3 controlPoint) {
            this.LineSketchObject = lineSketchObject;
            this.NewControlPoint = controlPoint;
        }

        public override void Execute()
        {
            LineSketchObject.addControlPoint(NewControlPoint);
        }

        public override void Redo()
        {
            this.Execute();
        }

        public override void Undo()
        {
            LineSketchObject.deleteControlPoint();
        }
    }
}
