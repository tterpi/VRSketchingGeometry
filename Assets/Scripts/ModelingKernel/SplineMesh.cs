//-----------------------------------------------------------------------
//
// Original repository: https://github.com/tterpi/VRSketchingGeometry
//
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Splines;
using Meshing;

/// <summary>
/// Manages the communication between a spline object and the corresponding mesh.
/// Provides methods to manipulate the spline and mesh simultaneously.
/// </summary>
public class SplineMesh
{
    private Spline Spline;
    private List<Vector3> interpolatedPoints;

    private LineExtruder lineExtruder;
    private MeshFilter meshFilter;

    public SplineMesh(Spline spline, MeshFilter meshFilter)
    {
        List<Vector3> crossSectionShape = new List<Vector3> { new Vector3(1f, 0f, 0.5f), new Vector3(1f, 0f, -0.5f), new Vector3(0f, 0f, -1f), new Vector3(-1f, 0f, -0.5f), new Vector3(-1f, 0f, 0.5f), new Vector3(0f, 0f, 1f) };
        crossSectionShape.Reverse();
        List<Vector3> crossSectionShapeNormals = new List<Vector3>();
        foreach (Vector3 point in crossSectionShape) {
            crossSectionShapeNormals.Add(point.normalized);
        }

        Spline = spline;
        interpolatedPoints = Spline.InterpolatedPoints;

        lineExtruder = new LineExtruder(crossSectionShape, crossSectionShapeNormals, new Vector3(.2f, .2f, .2f));

        this.meshFilter = meshFilter;
    }

    private void updateMesh(SplineModificationInfo info) {
        //Debug.Log(info);
        Mesh newMesh = lineExtruder.replacePoints(interpolatedPoints, info.Index, info.AddCount, info.RemoveCount);
        if (newMesh != null) {
            meshFilter.mesh = newMesh;
        }
    }

    public void addControlPoint(Vector3 controlPoint) {
        SplineModificationInfo info = Spline.addControlPoint(controlPoint);
        updateMesh(info);
    }

    public void deleteControlPoint(int index) {
        SplineModificationInfo info = Spline.deleteControlPoint(index);
        updateMesh(info);
    }

    public void insertControlPoint(int index, Vector3 controlPoint) {
        SplineModificationInfo info = Spline.insertControlPoint(index, controlPoint);
        updateMesh(info);
    }

    public void setControlPoint(int index, Vector3 controlPoint) {
        SplineModificationInfo info = Spline.setControlPoint(index, controlPoint);
        updateMesh(info);
    }

    public void setControlPoints(Vector3[] controlPoints) {
        Spline.setControlPoints(controlPoints);
        meshFilter.mesh = lineExtruder.getMesh(interpolatedPoints);
    }

    public int getNumberOfControlPoints() {
        return Spline.getNumberOfControlPoints();
    }

    public List<Vector3> getControlPoints() {
        return Spline.getControlPoints();
    }
}
