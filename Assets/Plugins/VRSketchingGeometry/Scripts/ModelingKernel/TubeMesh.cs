//-----------------------------------------------------------------------
//
// Original repository: https://github.com/tterpi/VRSketchingGeometry
//
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Meshing {
    /// <summary>
    /// Generates a tube like mesh according to a set of points on a line
    /// </summary>
    /// <remarks>Original author: tterpi
    /// This class was originally called LineExtruder. 
    /// It was renamed to make it consistant with the other mesh classes such as PatchMesh and RibbonMesh.
    /// </remarks>
    public class TubeMesh: ITubeMesh
    {
        private Vector3 crossSectionScale;
        public Vector3 CrossSectionScale { get => crossSectionScale; private set => crossSectionScale = value; }

        private List<Vector3> vertices;
        private List<Vector3> normals;
        private List<int> triangles;
        private bool generateCaps;

        private List<Vector3> capVertices;
        private List<Vector3> capNormals;
        private List<int> capTriangles;

        private List<Vector3> crossSectionShape;
        private List<Vector3> crossSectionNormals;

        /// <summary>
        /// Transform the points of a cross section for example.
        /// </summary>
        /// <param name="points">List of points to transform.</param>
        /// <param name="position">Position of the origin of the points.</param>
        /// <param name="tangent">Tangent to the spline at position.</param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static List<Vector3> TransformPoints(List<Vector3> points, Vector3 position, Vector3 tangent, Vector3 scale)
        {

            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.FromToRotation(Vector3.up, tangent), scale);

            List<Vector3> pointsTransformed = new List<Vector3>();

            foreach (Vector3 point in points)
            {
                pointsTransformed.Add(matrix.MultiplyPoint3x4(point));
            }

            return pointsTransformed;
        }

        /// <summary>
        /// Transform a list of normals.
        /// </summary>
        /// <param name="normals"></param>
        /// <param name="tangent"></param>
        /// <returns></returns>
        public static List<Vector3> TransformNormals(List<Vector3> normals, Vector3 tangent)
        {

            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.FromToRotation(Vector3.up, tangent), Vector3.one);

            List<Vector3> normalsTransformed = new List<Vector3>();

            foreach (Vector3 normal in normals)
            {
                normalsTransformed.Add(matrix.MultiplyVector(normal));
            }

            return normalsTransformed;
        }

        /// <summary>
        /// Constructor for a TubeMesh object.
        /// </summary>
        /// <param name="crossSectionShape">Vertices of the cross section.</param>
        /// <param name="crossSectionNormals">Normals of the cross section.</param>
        /// <param name="crossSectionScale">Scale of the cross section.</param>
        /// <param name="generateCaps">Should the holes at the end of the tube be closed.</param>
        public TubeMesh(List<Vector3> crossSectionShape, List<Vector3> crossSectionNormals, Vector3 crossSectionScale, bool generateCaps = true) {
            this.crossSectionShape = crossSectionShape;
            this.crossSectionNormals = crossSectionNormals;
            this.CrossSectionScale = crossSectionScale;
            this.generateCaps = generateCaps;

            this.vertices = new List<Vector3>();
            this.normals = new List<Vector3>();
            this.triangles = new List<int>();
        }

        /// <summary>
        /// Change the cross section used for generating the tube mesh.
        /// </summary>
        /// <remarks>Points have to be provided because this class does not keep a copy of the interpolated points.</remarks>
        /// <param name="points"></param>
        /// <param name="crossSection"></param>
        /// <returns></returns>
        public Mesh SetCrossSection(List<Vector3> points, CrossSection crossSection) {
            this.vertices = new List<Vector3>();
            this.normals = new List<Vector3>();
            this.triangles = new List<int>();

            this.crossSectionShape = crossSection.Vertices;
            this.crossSectionNormals = crossSection.Normals;
            this.CrossSectionScale = crossSection.Scale;
            return GenerateMesh(points);
        }

        /// <summary>
        /// Get a copy of the cross section currently in use.
        /// </summary>
        /// <returns></returns>
        public CrossSection GetCrossSection() {
            CrossSection crossSection = 
                new CrossSection(
                    new List<Vector3>(crossSectionShape), 
                    new List<Vector3>(crossSectionNormals), 
                    crossSectionScale
                );
            return crossSection;
        }

        private List<Vector3> TransformCrossSection(Vector3 position, Vector3 tangent) {
            return TransformPoints(crossSectionShape, position, tangent, CrossSectionScale);
        }

        /// <summary>
        /// Regenerate the whole mesh with the given points.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public Mesh GenerateMesh(List<Vector3> points) {
            if (points == null || points.Count == 0) {
                return null;
            }
            return ReplacePoints(points, 0, points.Count, vertices.Count / crossSectionShape.Count);
        }

        /// <summary>
        /// Replaces and/or removes points from an existing mesh.
        /// This should only be called when <see cref="GenerateMesh(List{Vector3})"/> was called before at least once.
        /// Different from <see cref="GenerateMesh(List{Vector3})"/> this will only recalculate the parts that were changed according to the parameters.
        /// </summary>
        /// <param name="points">All points of the complete line.</param>
        /// <param name="index">First point to be added or removed.</param>
        /// <param name="addCount">Number of points to add at index.</param>
        /// <param name="removeCount">Number of points to remove at index before adding the new ones.</param>
        /// <returns></returns>
        public Mesh ReplacePoints(List<Vector3> points, int index, int addCount, int removeCount) {

            if(addCount == 0 && removeCount == 0)
            {
                Debug.LogWarning("LineExtruder: Nothing was added or removed.");
                return null;
            }

            if (points.Count < 2 && addCount > 0) {
                Debug.LogWarning("LineExtruder: Mesh generation failed. Points contains less than 2 points.");
                return null;
            }

            int pointIndex = 0;
            int verticesIndex = 0;
            int pointAddCount = 0;
            int verticesRemoveCount = 0;

            //if there is a point before or after the section to be replaced, they have to be recalculated also to correct their orientation 
            int padding = 0;

            //if it doesn't replace the first point 
            if (index > 0) {
                verticesIndex = (index - 1) * crossSectionShape.Count;
                pointIndex = index -1;
                padding++;
            }
            //if it doesn't replace the last point
            if ((index + removeCount) * crossSectionShape.Count != vertices.Count)
            {
                padding++;
            }

            verticesRemoveCount = (removeCount + padding) * crossSectionShape.Count;
            pointAddCount = (addCount + padding);

            //remove invalid vertices and normals
            vertices.RemoveRange(verticesIndex, verticesRemoveCount);
            normals.RemoveRange(verticesIndex, verticesRemoveCount);

            //generate and add new vertices and normals
            List<Vector3> newVertices, newNormals, verticesToInsert, normalsToInsert;
            verticesToInsert = new List<Vector3>();
            normalsToInsert = new List<Vector3>();

            //This loop could be parallelized, the TransformCrossSection calls take a lot of time
            for (int i = pointIndex; i < pointIndex + pointAddCount; i++)
            {
                if (i == 0)
                {
                    (newVertices, newNormals) = TransformCrossSection(points[i], points[i], points[i+1]);
                }
                else if (i == points.Count - 1)
                {
                    (newVertices, newNormals) = TransformCrossSection(points[i-1], points[i], points[i]);
                }
                else {
                    (newVertices, newNormals) = TransformCrossSection(points[i - 1], points[i], points[i + 1]);
                }
                
                verticesToInsert.AddRange(newVertices);
                normalsToInsert.AddRange(newNormals);
            }

            vertices.InsertRange(verticesIndex, verticesToInsert);
            normals.InsertRange(verticesIndex, normalsToInsert);

            //update triangles
            triangles = new List<int>(Triangles.GenerateTrianglesCounterclockwise(vertices.Count / crossSectionShape.Count, crossSectionShape.Count));

            //mesh is empty or there is just a single cross section left because second to last control point or last control point was removed
            if (vertices.Count <= crossSectionShape.Count && normals.Count <= crossSectionShape.Count) {
                return new Mesh();
            }

            Mesh mesh;

            if (generateCaps)
            {
                GenerateCapsMesh(vertices.GetRange(0, crossSectionShape.Count), vertices.GetRange(vertices.Count - crossSectionShape.Count, crossSectionShape.Count));
                mesh = GetMeshWithCaps();
            }
            else {
                mesh = new Mesh();
                mesh.SetVertices(vertices);
                mesh.SetNormals(normals);
                mesh.subMeshCount = 1;
                mesh.SetTriangles(triangles.ToArray(), 0);
                mesh.SetUVs(0, TextureCoordinates.GenerateQuadrilateralUVsStretchU(vertices.Count, crossSectionShape.Count));

            }
            mesh.RecalculateTangents();
            return mesh;
        }

        /// <summary>
        /// This function transforms both vertices and normals of the cross section for point2.
        /// The tangent at point2 is calculated with point1 and point3.
        /// </summary>
        /// <param name="point1">The point before point2 in the line.</param>
        /// <param name="point2">The position of the calculated cross section.</param>
        /// <param name="point3">The point after point2 in the line.</param>
        /// <returns>(points, normals)</returns>
        private (List<Vector3>, List<Vector3>) TransformCrossSection(Vector3 point1, Vector3 point2, Vector3 point3) {
            Vector3 tangent = ((point2 - point1).normalized + (point3 - point2).normalized).normalized;
            return (TransformCrossSection(point2, tangent),TransformNormals(crossSectionNormals, tangent));
        }

        private void GenerateCapsMesh(List<Vector3> firstCrossSectionVertices,List<Vector3> lastCrossSectionVertices) {
            (capVertices, capNormals, capTriangles) = GenerateCapsMesh(firstCrossSectionVertices, lastCrossSectionVertices, vertices.Count);
        }

        /// <summary>
        /// Generate the mesh for the end caps that close the holes at the end of the tube.
        /// </summary>
        /// <param name="firstCrossSectionVertices"></param>
        /// <param name="lastCrossSectionVertices"></param>
        /// <param name="firstTriangleIndex"></param>
        /// <returns>Vertices, normals, triangles.</returns>
        public static (List<Vector3>, List<Vector3>, List<int>) GenerateCapsMesh(List<Vector3> firstCrossSectionVertices, List<Vector3> lastCrossSectionVertices, int firstTriangleIndex)
        {

            List<Vector3> capVertices = new List<Vector3>();
            List<Vector3> capNormals = new List<Vector3>();
            List<int> capTriangles = new List<int>();
            //generate caps
            //begin cap
            List<int> beginCapTriangles = new List<int>();

            for (int i = 1; i <= firstCrossSectionVertices.Count - 3; i++)
            {
                int firstVertex = firstTriangleIndex;

                beginCapTriangles.Add(firstVertex);
                beginCapTriangles.Add(firstVertex + i);
                beginCapTriangles.Add(firstVertex + i + 1);
            }
            capTriangles.AddRange(beginCapTriangles);

            capVertices.AddRange(firstCrossSectionVertices);

            Vector3 beginCapNormal = Vector3.Cross(firstCrossSectionVertices[1] - firstCrossSectionVertices[0], firstCrossSectionVertices[2] - firstCrossSectionVertices[0]).normalized;
            for (int i = 0; i < firstCrossSectionVertices.Count; i++)
            {
                capNormals.Add(beginCapNormal);
            }

            //end cap

            List<int> endCapTriangles = new List<int>();

            for (int i = 1; i <= lastCrossSectionVertices.Count - 3; i++)
            {
                int firstVertex = firstTriangleIndex + capVertices.Count;

                endCapTriangles.Add(firstVertex);
                endCapTriangles.Add(firstVertex + i + 1);
                endCapTriangles.Add(firstVertex + i);
            }
            capTriangles.AddRange(endCapTriangles);

            capVertices.AddRange(lastCrossSectionVertices);
            Vector3 endCapNormal = -Vector3.Cross(lastCrossSectionVertices[1] - lastCrossSectionVertices[0], lastCrossSectionVertices[2] - lastCrossSectionVertices[0]).normalized;
            for (int i = 0; i < lastCrossSectionVertices.Count; i++)
            {
                capNormals.Add(endCapNormal);
            }

            return (capVertices, capNormals, capTriangles);
        }

        private Mesh GetMeshWithCaps() {
            return GetMeshWithCaps(vertices, normals, triangles, capVertices, capNormals, capTriangles, crossSectionShape.Count);
        }

        /// <summary>
        /// Put all parts of the mesh together in a Mesh object.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="normals"></param>
        /// <param name="triangles"></param>
        /// <param name="capVertices"></param>
        /// <param name="capNormals"></param>
        /// <param name="capTriangles"></param>
        /// <param name="crossSectionVertexCount"></param>
        /// <returns></returns>
        public static Mesh GetMeshWithCaps(List<Vector3> vertices, List<Vector3> normals, List<int> triangles, List<Vector3> capVertices, List<Vector3> capNormals, List<int> capTriangles, int crossSectionVertexCount)
        {
            List<Vector3> allVertices = new List<Vector3>(vertices);
            allVertices.AddRange(capVertices);

            List<Vector3> allNormals = new List<Vector3>(normals);
            allNormals.AddRange(capNormals);

            List<int> allTriangles = new List<int>(triangles);
            allTriangles.AddRange(capTriangles);

            List<Vector2> uvs = TextureCoordinates.GenerateQuadrilateralUVsStretchU(vertices.Count, crossSectionVertexCount);

            uvs.AddRange(GenerateEndCapUVs(crossSectionVertexCount));

            Mesh mesh = new Mesh();

            mesh.SetVertices(allVertices);
            mesh.SetNormals(allNormals);
            mesh.subMeshCount = 1;
            mesh.SetTriangles(allTriangles.ToArray(), 0);
            mesh.SetUVs(0, uvs);

            return mesh;
        }

        /// <summary>
        /// Generate UVs for the end caps.
        /// </summary>
        /// <param name="crossSectionCount">Number of vertices per cross section.</param>
        /// <returns></returns>
        private static List<Vector2> GenerateEndCapUVs(int crossSectionCount) {
            List<Vector2> uvs = new List<Vector2>();
            for (int i = 0; i < crossSectionCount; i++)
            {
                uvs.Add(new Vector2(0, 0));
            }
            for (int i = 0; i < crossSectionCount; i++)
            {
                uvs.Add(new Vector2(1, 1));
            }

            return uvs;
        }
    }
}

