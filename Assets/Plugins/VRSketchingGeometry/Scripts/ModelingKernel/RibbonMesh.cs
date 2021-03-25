using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VRSketchingGeometry.Meshing
{
    /// <summary>
    /// Generate a mesh with a certain cross section along given points and rotations.
    /// Contrary to the LineExtruder and ParallelTransportTubeMesh, the rotations of the cross sections have to be given explicitly and are not calculated automatically.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class RibbonMesh
    {

        private Vector3 scale;
        private List<Vector3> Vertices;
        private List<Vector3> CrossSection;
        public List<Vector3> GetCrossSection() => new List<Vector3>(CrossSection);

        public Vector3 Scale { get => scale; private set => scale = value; }

        /// <summary>
        /// Get a mesh with a cross section that is a straight line along local z axis.
        /// </summary>
        /// <param name="points">Positions of the cross sections.</param>
        /// <param name="rotations">Orientation of the cross sections.</param>
        /// <param name="width">Width of the ribbon.</param>
        /// <returns></returns>
        internal static Mesh GetRibbonMesh(List<Vector3> points, List<Quaternion> rotations, float width) {

            List<Vector3> ribbonCrossSection = new List<Vector3> { new Vector3(0, 0, .5f), new Vector3(0, 0, 0), new Vector3(0, 0, -.5f) };
            return GetMesh(ribbonCrossSection, points, rotations, Vector3.one * width);
        }

        /// <summary>
        /// Transform the points of a cross section according to position, rotation and scale.
        /// </summary>
        /// <param name="crossSection"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static List<Vector3> TransformPoints(List<Vector3> crossSection, Vector3 position, Quaternion rotation, Vector3 scale) {
            List<Vector3> transformedCrossSection = new List<Vector3>();
            for (int j = 0; j < crossSection.Count; j++)
            {
                transformedCrossSection.Add(TransformPoint(crossSection[j], position, rotation, scale));
            }
            return transformedCrossSection;
        }

        /// <summary>
        /// Place the cross sections at the points oriented by the rotations.
        /// </summary>
        /// <param name="crossSection"></param>
        /// <param name="points"></param>
        /// <param name="rotations"></param>
        /// <param name="scale">Cross sections are scaled by this vector.</param>
        /// <returns></returns>
        public static List<Vector3> GetVertices(List<Vector3> crossSection, List<Vector3> points, List<Quaternion> rotations, Vector3 scale) {
            List<Vector3> vertices = new List<Vector3>();

            for (int i = 0; i < points.Count; i++)
            {
                List<Vector3> transformedCrossSection = TransformPoints(crossSection, points[i], rotations[i], scale);

                vertices.AddRange(transformedCrossSection);

            }
            return vertices;
        }

        /// <summary>
        /// Get the mesh with the cross sections placed at the points oriented by the rotations.
        /// Cross sections are scaled by scale.
        /// </summary>
        /// <param name="crossSection"></param>
        /// <param name="points"></param>
        /// <param name="rotations"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Mesh GetMesh(List<Vector3> crossSection, List<Vector3> points, List<Quaternion> rotations, Vector3 scale) {

            List<Vector3> vertices = GetVertices(crossSection, points, rotations, scale);

            return GetMeshFromVertices(vertices, crossSection.Count);
        }

        /// <summary>
        /// Generate a mesh object from the given vertices.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="crossSectionCount">Number of vertices per cross section.</param>
        /// <returns></returns>
        public static Mesh GetMeshFromVertices(List<Vector3> vertices, int crossSectionCount) {
            if (vertices.Count < crossSectionCount * 2) {
                return null;
            }

            //List<int> triangles = LineExtruder.GenerateTriangles(crossSectionCount, (vertices.Count / crossSectionCount) - 1);
            int[] triangles = Triangles.GenerateTrianglesCounterclockwise((vertices.Count / crossSectionCount), crossSectionCount);

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.SetUVs(0, TextureCoordinates.GenerateQuadrilateralUVsStretchU(vertices.Count, crossSectionCount));
            mesh.RecalculateTangents();

            return mesh;
        }

        /// <summary>
        /// Transform a single point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Vector3 TransformPoint(Vector3 point, Vector3 position, Quaternion rotation, Vector3 scale) {
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
            return matrix.MultiplyPoint3x4(point);
        }

        public RibbonMesh(List<Vector3> crossSection, Vector3 scale) {
            this.Vertices = new List<Vector3>();
            this.CrossSection = crossSection;
            this.Scale = scale;
        }

        /// <summary>
        /// Flat ribbon shaped mesh.
        /// Cross section is a straight line along local z axis.
        /// </summary>
        /// <param name="scale"></param>
        public RibbonMesh(Vector3 scale)
        {
            this.Vertices = new List<Vector3>();
            this.CrossSection = new List<Vector3> { new Vector3(0, 0, .5f), new Vector3(0, 0, 0), new Vector3(0, 0, -.5f) };
            this.Scale = scale;
        }

        /// <summary>
        /// Get mesh for points and rotations.
        /// Existing mesh is overwritten.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="rotations"></param>
        /// <returns></returns>
        public Mesh GetMesh(List<Vector3> points, List<Quaternion> rotations) {
            Vertices = GetVertices(CrossSection, points, rotations, Scale);
            return GetMeshFromVertices(Vertices, CrossSection.Count);
        }

        /// <summary>
        /// Add to the end of the existing mesh.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="rotations"></param>
        /// <returns></returns>
        public Mesh AddPoints(List<Vector3> points, List<Quaternion> rotations) {
            Vertices.AddRange(GetVertices(CrossSection, points, rotations, Scale));
            return GetMeshFromVertices(Vertices, CrossSection.Count);
        }

        /// <summary>
        /// Add single point to the end of the mesh.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public Mesh AddPoint(Vector3 point, Quaternion rotation) {
            Vertices.AddRange(TransformPoints(CrossSection, point, rotation, Scale));
            return GetMeshFromVertices(Vertices, CrossSection.Count);
        }

        /// <summary>
        /// Delete the last point of the mesh.
        /// </summary>
        /// <returns></returns>
        public Mesh DeletePoint() {
            if (Vertices.Count == 0) return null;

            Vertices.RemoveRange(Vertices.Count - CrossSection.Count, CrossSection.Count);
            return GetMeshFromVertices(Vertices, CrossSection.Count);
        }
    }
}
