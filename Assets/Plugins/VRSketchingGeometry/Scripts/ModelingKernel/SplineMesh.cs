//-----------------------------------------------------------------------
//
// Original repository: https://github.com/tterpi/VRSketchingGeometry
//
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;
using VRSketchingGeometry.Meshing;

namespace VRSketchingGeometry.Meshing
{

    /// <summary>
    /// Manages the communication between a spline object and the corresponding mesh.
    /// Provides methods to manipulate the spline and mesh simultaneously.
    /// </summary>
    public class SplineMesh
    {
        private Spline Spline;
        private List<Vector3> interpolatedPoints;

        private LineExtruder lineExtruder;
        private Mesh mesh;

        private List<Vector3> crossSectionShape;
        private List<Vector3> crossSectionShapeNormals;
        private float nativeCrossSectionShapeDiameter = 2f;

        public Mesh Mesh { get => mesh; private set => mesh = value; }

        public SplineMesh(Spline spline) : this(spline, new Vector3(.4f, .4f, .4f))
        {}

        public SplineMesh(Spline spline, Vector3 crossSectionScale)
        {
            crossSectionShape = new List<Vector3> { new Vector3(1f, 0f, 0.5f), new Vector3(1f, 0f, -0.5f), new Vector3(0f, 0f, -1f), new Vector3(-1f, 0f, -0.5f), new Vector3(-1f, 0f, 0.5f), new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 0.5f) };
            crossSectionShape.Reverse();
            crossSectionShapeNormals = new List<Vector3>();
            foreach (Vector3 point in crossSectionShape)
            {
                crossSectionShapeNormals.Add(point.normalized);
            }

            Spline = spline;
            interpolatedPoints = Spline.InterpolatedPoints;

            lineExtruder = new LineExtruder(crossSectionShape, crossSectionShapeNormals, crossSectionScale / nativeCrossSectionShapeDiameter);
        }

        private Mesh updateMesh(SplineModificationInfo info)
        {
            //Debug.Log(info);
            Mesh newMesh = lineExtruder.ReplacePoints(interpolatedPoints, info.Index, info.AddCount, info.RemoveCount);
            return newMesh;
        }

        public Mesh addControlPoint(Vector3 controlPoint)
        {
            SplineModificationInfo info = Spline.addControlPoint(controlPoint);
            return updateMesh(info);
        }

        public Mesh deleteControlPoint(int index)
        {
            SplineModificationInfo info = Spline.deleteControlPoint(index);
            return updateMesh(info);
        }

        public Mesh insertControlPoint(int index, Vector3 controlPoint)
        {
            SplineModificationInfo info = Spline.insertControlPoint(index, controlPoint);
            return updateMesh(info);
        }

        public Mesh setControlPoint(int index, Vector3 controlPoint)
        {
            SplineModificationInfo info = Spline.setControlPoint(index, controlPoint);
            return updateMesh(info);
        }

        public Mesh setControlPoints(Vector3[] controlPoints)
        {
            Spline.setControlPoints(controlPoints);
            return lineExtruder.GetMesh(interpolatedPoints);
        }

        public int getNumberOfControlPoints()
        {
            return Spline.getNumberOfControlPoints();
        }

        public List<Vector3> getControlPoints()
        {
            return Spline.getControlPoints();
        }

        /// <summary>
        /// At scale 1 the line will have an diameter of 1.
        /// </summary>
        /// <param name="scale"></param>
        public Mesh setCrossSectionScale(Vector3 scale)
        {
            lineExtruder = new LineExtruder(crossSectionShape, crossSectionShapeNormals, scale / nativeCrossSectionShapeDiameter);
            return lineExtruder.GetMesh(interpolatedPoints);
        }

        public Mesh RefineMesh() {
            return ParallelTransportTubeMesh.GetMesh(interpolatedPoints, crossSectionShape, crossSectionShapeNormals, lineExtruder.CrossSectionScale, true);
        }
    }
}