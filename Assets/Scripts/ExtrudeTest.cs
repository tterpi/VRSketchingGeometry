using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KochanekBartelsSplines;

public class ExtrudeTest : MonoBehaviour
{
    private Vector3[] ControlPoints;

    KochanekBartelsSpline kochanekBartelsSpline;

    [SerializeField]
    private GameObject[] ControlPointObjects;

    private List<Vector3> InterpolatedPoints;

    // Start is called before the first frame update
    void Start()
    {
        List<Vector3> crossSectionShape = new List<Vector3> { new Vector3(1f, 0f, 0.5f), new Vector3(1f, 0f, -0.5f), new Vector3(0f, 0f, -1f), new Vector3(-1f, 0f, -0.5f), new Vector3(-1f, 0f, 0.5f), new Vector3(0f, 0f, 1f) };
        crossSectionShape.Reverse();
        List<Vector3> crossSectionShapeNormals = new List<Vector3>();
        foreach (Vector3 point in crossSectionShape) {
            crossSectionShapeNormals.Add(point.normalized);
        }

        kochanekBartelsSpline = new KochanekBartelsSpline();

        ControlPoints = new Vector3[ControlPointObjects.Length];
        for (int i = 0; i < ControlPointObjects.Length; i++)
        {
            ControlPoints[i] = ControlPointObjects[i].transform.position;
        }

        InterpolatedPoints = kochanekBartelsSpline.InterpolatedPoints;
        kochanekBartelsSpline.setControlPoints(ControlPoints);

        Meshing.LineExtruder lineExtruder = new Meshing.LineExtruder(crossSectionShape, crossSectionShapeNormals, new Vector3(.5f, .5f, .5f));
        Mesh mesh = lineExtruder.getMesh(InterpolatedPoints, false);

        MeshFilter meshFilter = GetComponent<MeshFilter>();

        //mesh.UploadMeshData(false);
        meshFilter.mesh = mesh;
    }
}
