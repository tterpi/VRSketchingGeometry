using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Meshing;

[ExecuteInEditMode]
public class LineSketchObjectSetup : MonoBehaviour
{
    public LineSketchObject line;
    public GameObject ControlPointParent;
    public int CrossSectionResolution;
    public int InterpolationSteps;
    public float LineDiameter;

    // Start is called before the first frame update
    void Start()
    {
        List<Vector3> crossSection = CircularCrossSection.GenerateVertices(CrossSectionResolution);

        line.SetLineCrossSection(crossSection,crossSection,LineDiameter);
        line.SetInterpolationSteps(InterpolationSteps);
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3> ControlPoints = new List<Vector3>();
        foreach (Transform controlPoint in ControlPointParent.transform) {
            ControlPoints.Add(controlPoint.position);
        }
        line.SetControlPoints(ControlPoints);
        line.RefineMesh();
    }
}
