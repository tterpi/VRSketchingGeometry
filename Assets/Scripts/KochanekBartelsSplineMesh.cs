using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Splines;
using Meshing;

public class KochanekBartelsSplineMesh : MonoBehaviour
{
    KochanekBartelsSpline kochanekBartelsSpline;
    private List<Vector3> interpolatedPoints;

    private LineExtruder lineExtruder;
    private MeshFilter meshFilter;

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
        interpolatedPoints = kochanekBartelsSpline.InterpolatedPoints;

        lineExtruder = new LineExtruder(crossSectionShape, crossSectionShapeNormals, new Vector3(.2f, .2f, .2f));

        meshFilter = GetComponent<MeshFilter>();
    }

    private void updateMesh(SplineModificationInfo info) {
        meshFilter.mesh = lineExtruder.replacePoints(interpolatedPoints.GetRange(info.Index, info.AddCount), info.Index, info.RemoveCount);
    }

    public void addControlPoint(Vector3 controlPoint) {
        SplineModificationInfo info = kochanekBartelsSpline.addControlPoint(controlPoint);
        updateMesh(info);
    }

    public void deleteControlPoint(int index) {
        SplineModificationInfo info = kochanekBartelsSpline.deleteControlPoint(index);
        updateMesh(info);
    }

    public void insertControlPoint(int index, Vector3 controlPoint) {
        SplineModificationInfo info = kochanekBartelsSpline.insertControlPoint(index, controlPoint);
        updateMesh(info);
    }

    public void setControlPoint(int index, Vector3 controlPoint) {
        SplineModificationInfo info = kochanekBartelsSpline.setControlPoint(index, controlPoint);
        updateMesh(info);
    }

    public void setControlPoints(Vector3[] controlPoints) {
        kochanekBartelsSpline.setControlPoints(controlPoints);
        meshFilter.mesh = lineExtruder.getMesh(interpolatedPoints);
    }
}
