using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.SketchObjectManagement;

/// <summary>
/// Refine the mesh of a line sketch object using the Parallel Transport algorithm.
/// </summary>
public class RefineMeshCommand : ICommand
{
    LineSketchObject LineSketchObject;
    List<Vector3> OriginalControlPoints;

    public RefineMeshCommand(LineSketchObject lineSketchObject) {
        this.LineSketchObject = lineSketchObject;
        OriginalControlPoints = lineSketchObject.getControlPoints();
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
        this.LineSketchObject.SetControlPoints(OriginalControlPoints);
    }
}
