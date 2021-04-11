using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Line {
    /// <summary>
    /// Add control point at the end of the spline.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
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
            LineSketchObject.AddControlPoint(NewControlPoint);
            return true;
        }

        public void Redo()
        {
            if (SketchWorld.ActiveSketchWorld != null && SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.LineSketchObject)) {
                SketchWorld.ActiveSketchWorld.RestoreObject(this.LineSketchObject);
            }
            this.Execute();
        }

        public void Undo()
        {
            LineSketchObject.DeleteControlPoint();
            if (this.LineSketchObject.getNumberOfControlPoints() == 0) {
                SketchWorld.ActiveSketchWorld.DeleteObject(this.LineSketchObject);
            }
        }
    }
}
