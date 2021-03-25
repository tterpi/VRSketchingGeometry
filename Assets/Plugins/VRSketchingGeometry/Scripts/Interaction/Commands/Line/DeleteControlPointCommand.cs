using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Line
{
    /// <summary>
    /// Delete control point at the end of spline.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class DeleteControlPointCommand : ICommand
    {
        private LineSketchObject LineSketchObject;
        private Vector3 OldControlPoint;

        public DeleteControlPointCommand(LineSketchObject lineSketchObject)
        {
            this.LineSketchObject = lineSketchObject;
        }

        public bool Execute()
        {
            this.OldControlPoint = LineSketchObject.GetControlPoints()[LineSketchObject.getNumberOfControlPoints() - 1];
            LineSketchObject.DeleteControlPoint();
            if (this.LineSketchObject.getNumberOfControlPoints() == 0)
            {
                SketchWorld.ActiveSketchWorld.DeleteObject(this.LineSketchObject);
            }
            return true;
        }

        public void Redo()
        {
            this.Execute();
        }

        public void Undo()
        {
            if (SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.LineSketchObject))
            {
                SketchWorld.ActiveSketchWorld.RestoreObject(this.LineSketchObject);
            }
            LineSketchObject.AddControlPoint(OldControlPoint);
        }
    }
}
