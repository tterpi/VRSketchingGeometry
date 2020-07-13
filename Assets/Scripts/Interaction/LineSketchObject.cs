using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Splines;

public class LineSketchObject : MonoBehaviour
{
    [SerializeField]
    private SplineMesh SplineMesh;

    private MeshFilter meshFilter;

    private MeshCollider meshCollider;

    //TODO delete spline

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        SplineMesh = new SplineMesh(new KochanekBartelsSpline(), meshFilter);
        Vector3[] controlPoints = { new Vector3(0, 0, 0), new Vector3(.2f, .2f, 0), new Vector3(.4f, 0, 0) };
        SplineMesh.setControlPoints(controlPoints);

        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

    public void addControlPoint(Vector3 point) {
        Vector3 transformedPoint = transform.InverseTransformPoint(point);
        SplineMesh.addControlPoint(transformedPoint);
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

    public void deleteControlPoint() {
        SplineMesh.deleteControlPoint(SplineMesh.getNumberOfControlPoints() - 1);
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

}
