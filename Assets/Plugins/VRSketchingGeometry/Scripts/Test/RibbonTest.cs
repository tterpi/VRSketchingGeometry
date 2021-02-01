using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Meshing;

public class RibbonTest : MonoBehaviour
{
    public GameObject ControlPointParent;

    private void generateRibbon() {
        List<Vector3> points = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();
        foreach (Transform controlPoint in ControlPointParent.transform)
        {
            points.Add(controlPoint.position);
            rotations.Add(controlPoint.rotation);
        }

        GetComponent<MeshFilter>().mesh = RibbonMesh.GetRibbonMesh(points, rotations, .4f);
    }

    // Start is called before the first frame update
    void Start()
    {
            }

    // Update is called once per frame
    void Update()
    {
        generateRibbon();
    }
}
