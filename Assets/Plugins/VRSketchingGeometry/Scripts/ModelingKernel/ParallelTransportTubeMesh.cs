using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VRSketchingGeometry.Meshing
{
    /// <summary>
    /// Methods for creating a tube like mesh using the parallel transport algorithm.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class ParallelTransportTubeMesh : ITubeMesh
    {
        private CrossSection CrossSection;
        private bool GenerateCaps;

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="crossSection">Cross section to be used for the tube mesh.</param>
        /// <param name="generateCaps">Should the ends of the tube be closed?</param>
        public ParallelTransportTubeMesh(CrossSection crossSection, bool generateCaps = true) {
            this.CrossSection = crossSection;
            this.GenerateCaps = generateCaps;
        }

        /// <summary>
        /// Generate a mesh for all points.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public Mesh GenerateMesh(List<Vector3> points)
        {
            if (points == null || points.Count == 0)
            {
                return null;
            }
            return GetMesh(points, this.CrossSection.Vertices, this.CrossSection.Normals, this.CrossSection.Scale, this.GenerateCaps);
        }

        /// <summary>
        /// The parameters except points are ignored.
        /// This recalculates the whole mesh just like <see cref="GenerateMesh(List{Vector3})"/>.
        /// </summary>
        /// <param name="points">All points of the complete line.</param>
        /// <param name="index">ignored</param>
        /// <param name="addCount">ignored</param>
        /// <param name="removeCount">ignored</param>
        /// <returns></returns>
        public Mesh ReplacePoints(List<Vector3> points, int index, int addCount, int removeCount)
        {
            return GenerateMesh(points);
        }

        /// <summary>
        /// Set the cross section and recalculate the mesh for all points.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="crossSection"></param>
        /// <returns></returns>
        public Mesh SetCrossSection(List<Vector3> points, CrossSection crossSection) {
            this.CrossSection = crossSection;
            return this.GenerateMesh(points);
        }

        /// <summary>
        /// Get a copy of the cross section used.
        /// </summary>
        /// <returns></returns>
        public CrossSection GetCrossSection() {
            return new CrossSection(this.CrossSection);
        }

        /// <summary>
        /// Generate a mesh for a spline according to the parallel transport algorithm.
        /// </summary>
        /// <param name="points">Points of the spline.</param>
        /// <param name="crossSectionShape">Vertices of the mesh cross section to be placed at each point.</param>
        /// <param name="crossSectionNormals">Normals of the cross section.</param>
        /// <param name="crossSectionScale">Vector to scale the cross section by. Can be used to control the diameter of the tube mesh.</param>
        /// <param name="generateCaps">Should the ends of the mesh be closed with planar meshes?</param>
        /// <returns></returns>
        public static Mesh GetMesh(List<Vector3> points, List<Vector3> crossSectionShape, List<Vector3> crossSectionNormals, Vector3 crossSectionScale, bool generateCaps)
        {

            List<Vector3> tangents = GetTangents(points);
            Vector3 initialNormal = Vector3.Cross(tangents[0], Vector3.right);
            if (initialNormal.magnitude == 0)
            {
                initialNormal = Vector3.Cross(tangents[0], Vector3.forward);
            }
            List<Vector3> normals = GetSplineNormals(initialNormal, tangents);

            List<Vector3> meshVertices = new List<Vector3>();
            List<Vector3> meshNormals = new List<Vector3>();

            for (int i = 0; i < points.Count; i++)
            {
                List<Vector3> transformedCrossSection = TransformPoints(crossSectionShape, points[i], tangents[i], normals[i], crossSectionScale);
                List<Vector3> transformedCrossSectionNormals = TransformNormals(crossSectionNormals, tangents[i], normals[i]);
                meshVertices.AddRange(transformedCrossSection);
                meshNormals.AddRange(transformedCrossSectionNormals);
            }

            //update triangles
            List<int> meshTriangles = new List<int>(Triangles.GenerateTrianglesCounterclockwise((meshVertices.Count / crossSectionShape.Count), crossSectionShape.Count));

            //mesh is empty or there is just a single cross section left because second to last control point or last control point was removed
            if (meshVertices.Count <= crossSectionShape.Count && meshNormals.Count <= crossSectionShape.Count)
            {
                return new Mesh();
            }

            Mesh mesh;

            if (generateCaps)
            {
                (List<Vector3> capVertices, List<Vector3> capNormals, List<int> capTriangles) = TubeMesh.GenerateCapsMesh(meshVertices.GetRange(0, crossSectionShape.Count), 
                    meshVertices.GetRange(meshVertices.Count - crossSectionShape.Count, crossSectionShape.Count),
                    meshVertices.Count);
                mesh = TubeMesh.GetMeshWithCaps(meshVertices, meshNormals, meshTriangles, capVertices, capNormals, capTriangles, crossSectionShape.Count);
            }
            else
            {
                mesh = new Mesh();
                mesh.SetVertices(meshVertices);
                mesh.SetNormals(meshNormals);
                mesh.subMeshCount = 1;
                mesh.SetTriangles(meshTriangles.ToArray(), 0);
                mesh.SetUVs(0, TextureCoordinates.GenerateQuadrilateralUVsStretchU(meshVertices.Count, crossSectionShape.Count));

            }
            mesh.RecalculateTangents();
            return mesh;
        }

        /// <summary>
        /// Calculate the tangent for the point between p1 and p2.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static Vector3 GetTangent(Vector3 p1, Vector3 p2)
        {
            return (p2 - p1).normalized;
        }

        /// <summary>
        /// Calculate the tangents of a spline.
        /// </summary>
        /// <param name="points">The points of the spline.</param>
        /// <returns></returns>
        private static List<Vector3> GetTangents(List<Vector3> points)
        {
            List<Vector3> tangents = new List<Vector3>();
            tangents.Add(GetTangent(points[0], points[1]));
            for (int i = 1; i < points.Count - 1; i++)
            {
                tangents.Add(GetTangent(points[i - 1], points[i + 1]));
            }
            tangents.Add(GetTangent(points[points.Count - 2], points[points.Count - 1]));
            return tangents;
        }

        /// <summary>
        /// Calculate the normals of a spline.
        /// </summary>
        /// <param name="initialNormal">The first normal at first tangent.</param>
        /// <param name="tangents">The tangents of the spline.</param>
        /// <returns></returns>
        private static List<Vector3> GetSplineNormals(Vector3 initialNormal, List<Vector3> tangents)
        {
            List<Vector3> normals = new List<Vector3>();
            Vector3 currentNormal = initialNormal;
            normals.Add(currentNormal);
            for (int i = 1; i < tangents.Count; i++)
            {
                Vector3 nextNormal = GetNextNormal(currentNormal, tangents[i - 1], tangents[i]);
                currentNormal = nextNormal;
                normals.Add(nextNormal);
            }
            return normals;
        }

        /// <summary>
        /// Parallel transport method for calculating a vector normal to the spline at the point of the next tangent.
        /// This method is directly based on the pseudo code in the 1995 paper by Hanson and Ma.
        /// </summary>
        /// <param name="currentNormal"></param>
        /// <param name="currentTangent"></param>
        /// <param name="nextTangent"></param>
        /// <returns>Normal for next tangent.</returns>
        private static Vector3 GetNextNormal(Vector3 currentNormal, Vector3 currentTangent, Vector3 nextTangent)
        {
            Vector3 nextNormal;
            Vector3 binormal = Vector3.Cross(currentTangent, nextTangent);
            if (binormal.magnitude == 0)
            {
                nextNormal = currentNormal;
            }
            else
            {
                Quaternion rotation = Quaternion.AngleAxis(Vector3.Angle(currentTangent, nextTangent), binormal);
                nextNormal = rotation * currentNormal;
            }
            return nextNormal;
        }

        /// <summary>
        /// Transform points so that up points along tangent and forward along spline normal.
        /// </summary>
        /// <param name="points">Mesh vertices.</param>
        /// <param name="position">New position of the origin of the vertices.</param>
        /// <param name="tangent">Tangent at a point of the spline.</param>
        /// <param name="splineNormal">Normal at a point of the spline.</param>
        /// <param name="scale">Vector to scale the vertices by.</param>
        /// <returns></returns>
        private static List<Vector3> TransformPoints(List<Vector3> points, Vector3 position, Vector3 tangent, Vector3 splineNormal, Vector3 scale)
        {

            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.LookRotation(splineNormal, tangent), scale);

            List<Vector3> pointsTransformed = new List<Vector3>();

            foreach (Vector3 point in points)
            {
                pointsTransformed.Add(matrix.MultiplyPoint3x4(point));
            }

            return pointsTransformed;
        }

        /// <summary>
        /// Transform normals so that the up points in the tangent direction and forward in the spline normal direction.
        /// </summary>
        /// <param name="normals">Mesh normals.</param>
        /// <param name="tangent">Tangent at a point of the spline.</param>
        /// <param name="splineNormal">Normal at a point of the spline.</param>
        /// <returns></returns>
        private static List<Vector3> TransformNormals(List<Vector3> normals, Vector3 tangent, Vector3 splineNormal)
        {

            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(splineNormal, tangent), Vector3.one);

            List<Vector3> normalsTransformed = new List<Vector3>();

            foreach (Vector3 normal in normals)
            {
                normalsTransformed.Add(matrix.MultiplyVector(normal));
            }

            return normalsTransformed;
        }
    }
}
