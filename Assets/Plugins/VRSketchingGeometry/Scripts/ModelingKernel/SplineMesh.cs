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
    /// <remarks>Original author: tterpi</remarks>
    public class SplineMesh
    {
        private Spline Spline;
        private List<Vector3> interpolatedPoints;

        private TubeMesh lineExtruder;
        private Mesh mesh;

        private List<Vector3> crossSectionShape;
        private List<Vector3> crossSectionShapeNormals;

        public Mesh Mesh { get => mesh; private set => mesh = value; }

        /// <summary>
        /// Contructor with default circular cross section.
        /// </summary>
        /// <remarks>Scale is 0.4 and cross section resolution is 6.</remarks>
        /// <param name="spline"></param>
        public SplineMesh(Spline spline) : this(spline, new Vector3(.4f, .4f, .4f))
        {}

        /// <summary>
        /// Contructor for a spline mesh.
        /// </summary>
        /// <param name="spline"></param>
        /// <param name="crossSectionScale"></param>
        /// <param name="crossSectionResolution">Number of vertices per circular cross section. 
        /// There will be one more vertex per cross section to create a seam for UV mapping reasons.
        /// </param>
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

            lineExtruder = new TubeMesh(crossSectionShape, crossSectionShapeNormals, crossSectionScale);

        }

        private Mesh UpdateMesh(SplineModificationInfo info)
        {
            //Debug.Log(info);
            Mesh newMesh = lineExtruder.ReplacePoints(interpolatedPoints, info.Index, info.AddCount, info.RemoveCount);
            return newMesh;
        }

        /// <summary>
        /// Add a control point to the end of the spline.
        /// </summary>
        /// <param name="controlPoint"></param>
        /// <returns></returns>
        public Mesh AddControlPoint(Vector3 controlPoint)
        {
            SplineModificationInfo info = Spline.AddControlPoint(controlPoint);
            return UpdateMesh(info);
        }

        /// <summary>
        /// Delete the control point at index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Mesh DeleteControlPoint(int index)
        {
            SplineModificationInfo info = Spline.DeleteControlPoint(index);
            return UpdateMesh(info);
        }

        /// <summary>
        /// Insert control point at index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="controlPoint"></param>
        /// <returns></returns>
        public Mesh InsertControlPoint(int index, Vector3 controlPoint)
        {
            SplineModificationInfo info = Spline.InsertControlPoint(index, controlPoint);
            return UpdateMesh(info);
        }

        /// <summary>
        /// Replace the control point at index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="controlPoint"></param>
        /// <returns></returns>
        public Mesh SetControlPoint(int index, Vector3 controlPoint)
        {
            SplineModificationInfo info = Spline.SetControlPoint(index, controlPoint);
            return UpdateMesh(info);
        }

        /// <summary>
        /// Set all control points.
        /// </summary>
        /// <param name="controlPoints"></param>
        /// <returns></returns>
        public Mesh SetControlPoints(Vector3[] controlPoints)
        {
            Spline.SetControlPoints(controlPoints);
            return lineExtruder.GenerateMesh(interpolatedPoints);
        }

        public int GetNumberOfControlPoints()
        {
            return Spline.GetNumberOfControlPoints();
        }

        public List<Vector3> GetControlPoints()
        {
            return Spline.GetControlPoints();
        }

        /// <summary>
        /// The diameter of the spline will depend on the native size of the set cross section.
        /// The default circular cross section has a default diameter of 1.
        /// </summary>
        /// <param name="scale"></param>
        public Mesh SetCrossSectionScale(Vector3 scale)
        {
            lineExtruder = new TubeMesh(crossSectionShape, crossSectionShapeNormals, scale);
            return lineExtruder.GenerateMesh(interpolatedPoints);
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
            lineExtruder = new TubeMesh(crossSectionShape, crossSectionShapeNormals, crossSectionDiameter);
            return lineExtruder.GenerateMesh(interpolatedPoints);
        }

        public Mesh RefineMesh() {
            return ParallelTransportTubeMesh.GetMesh(interpolatedPoints, crossSectionShape, crossSectionShapeNormals, lineExtruder.CrossSectionScale, true);
        }
    }
}