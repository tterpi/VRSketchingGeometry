using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Line
{
    /// <summary>
    /// Refine the mesh of a line sketch object using the Parallel Transport algorithm.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class RefineMeshCommand : ICommand
    {
        LineSketchObject LineSketchObject;
        List<Vector3> OriginalControlPoints;

        public RefineMeshCommand(LineSketchObject lineSketchObject)
        {
            this.LineSketchObject = lineSketchObject;
            OriginalControlPoints = lineSketchObject.GetControlPoints();
        }

        public bool Execute()
        {
            LineSketchObject.RefineMesh();
            return true;
        }

        public void Redo()
        {
            this.Execute();
        }

        public void Undo()
        {
            this.LineSketchObject.SetControlPointsLocalSpace(OriginalControlPoints);
        }
    }
}