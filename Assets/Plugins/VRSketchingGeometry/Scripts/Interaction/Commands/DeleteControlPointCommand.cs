using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands
{
    /// <summary>
    /// Delete control point at the end of spline.
    /// </summary>
    public class DeleteControlPointCommand : Command
    {
        private LineSketchObject LineSketchObject;
        private Vector3 OldControlPoint;

        public DeleteControlPointCommand(LineSketchObject lineSketchObject)
        {
            this.LineSketchObject = lineSketchObject;
            this.OldControlPoint = lineSketchObject.getControlPoints()[lineSketchObject.getNumberOfControlPoints() - 1];
        }

        public override void Execute()
        {
            LineSketchObject.deleteControlPoint();
        }

        public override void Redo()
        {
            this.Execute();
        }

        public override void Undo()
        {
            LineSketchObject.addControlPoint(OldControlPoint);
        }
    }
}
