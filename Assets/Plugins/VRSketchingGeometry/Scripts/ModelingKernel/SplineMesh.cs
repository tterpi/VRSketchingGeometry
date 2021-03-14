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

        public Mesh Mesh { get => mesh; private set => mesh = value; }

        public SplineMesh(Spline spline) : this(spline, new Vector3(.4f, .4f, .4f))
        {}

        public SplineMesh(Spline spline, Vector3 crossSectionScale, int crossSectionResolution = 6)
        {
            List<Vector3> vertices = CircularCrossSection.GenerateVertices(crossSectionResolution);
            crossSectionShape = vertices;
            crossSectionShapeNormals = new List<Vector3>();
            foreach (Vector3 point in crossSectionShape)
            {
                crossSectionShapeNormals.Add(point.normalized);
            }

            Spline = spline;
            interpolatedPoints = Spline.InterpolatedPoints;

            lineExtruder = new LineExtruder(crossSectionShape, crossSectionShapeNormals, crossSectionScale);

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
        /// The diameter of the spline will depend on the native size of the set cross section.
        /// The default circular cross section has a default diameter of 1.
        /// </summary>
        /// <param name="scale"></param>
        public Mesh SetCrossSectionScale(Vector3 scale)
        {
            lineExtruder = new LineExtruder(crossSectionShape, crossSectionShapeNormals, scale);
            return lineExtruder.GetMesh(interpolatedPoints);
        }

        /// <summary>
        /// Get the cross section.
        /// </summary>
        /// <param name="crossSectionShape">A copy of the cross section shape is assigned to this variable.</param>
        /// <param name="crossSectionShapeNormals">A copy of the cross section normals is assigned to this variable.</param>
        public void GetCrossSectionShape(out List<Vector3> crossSectionShape, out List<Vector3> crossSectionShapeNormals) {
            crossSectionShape = new List<Vector3>(this.crossSectionShape);
            crossSectionShapeNormals = new List<Vector3>(this.crossSectionShapeNormals);
        }

        /// <summary>
        /// Change the cross section of the mesh. This will regenerete the mesh.
        /// </summary>
        /// <param name="crossSectionShape"></param>
        /// <param name="crossSectionNormals"></param>
        /// <param name="crossSectionDiameter">The requested diameter of the cross section.</param>
        /// <returns></returns>
        public Mesh SetCrossSection(List<Vector3> crossSectionShape, List<Vector3> crossSectionNormals, Vector3 crossSectionDiameter) {
            this.crossSectionShape = crossSectionShape;
            this.crossSectionShapeNormals = crossSectionNormals;
            lineExtruder = new LineExtruder(crossSectionShape, crossSectionShapeNormals, crossSectionDiameter);
            return lineExtruder.GetMesh(interpolatedPoints);
        }

        public Mesh RefineMesh() {
            return ParallelTransportTubeMesh.GetMesh(interpolatedPoints, crossSectionShape, crossSectionShapeNormals, lineExtruder.CrossSectionScale, true);
        }
    }
}