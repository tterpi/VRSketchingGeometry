using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

[ExecuteInEditMode]
public class RibbonSketchObjectSetup : MonoBehaviour
{
    public RibbonSketchObject Ribbon;
    public GameObject ControlPoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void SetUpRibbon() {
        List<Vector3> points = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();

        foreach (Transform controlPoint in ControlPoints.transform) {
            points.Add(controlPoint.position);
            rotations.Add(controlPoint.rotation);
        }

        Ribbon.SetControlPoints(points, rotations);
    }

    // Update is called once per frame
    void Update()
    {
        SetUpRibbon();
    }
}
