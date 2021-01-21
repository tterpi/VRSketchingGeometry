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
    public class LineExtruder
    {

        private Vector3 crossSectionScale;
        private List<Vector3> vertices;
        private List<Vector3> normals;
        private List<int> triangles;
        private bool generateCaps;

        private List<Vector3> capVertices;
        private List<Vector3> capNormals;
        private List<int> capTriangles;

        private List<Vector3> crossSectionShape;
        private List<Vector3> crossSectionNormals;

        public static List<Vector3> transformPoints(List<Vector3> points, Vector3 position, Vector3 tangent, Vector3 scale)
        {

            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.FromToRotation(Vector3.up, tangent), scale);

            List<Vector3> pointsTransformed = new List<Vector3>();

            foreach (Vector3 point in points)
            {
                pointsTransformed.Add(matrix.MultiplyPoint3x4(point));
            }

            return pointsTransformed;
        }

        public static List<Vector3> transformNormals(List<Vector3> normals, Vector3 tangent)
        {

            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.FromToRotation(Vector3.up, tangent), Vector3.one);

            List<Vector3> normalsTransformed = new List<Vector3>();

            foreach (Vector3 normal in normals)
            {
                normalsTransformed.Add(matrix.MultiplyVector(normal));
            }

            return normalsTransformed;
        }

        public static List<int> generateTriangles(int numOfCrossSectionVertices, int numOfSections)
        {

            List<int> triangles = new List<int>();

            //for each cross section
            for (int j = 0; j < numOfSections; j++)
            {
                //for each vertex of the current cross section
                for (int i = j * numOfCrossSectionVertices; i < (j + 1) * numOfCrossSectionVertices; i++)
                {
                    //if not the last vertex of the cross section
                    if (!(i % numOfCrossSectionVertices == numOfCrossSectionVertices - 1))
                    {
                        triangles.Add(i);
                        triangles.Add(i + 1 + numOfCrossSectionVertices);
                        triangles.Add(i + 1);

                        triangles.Add(i);
                        triangles.Add(i + numOfCrossSectionVertices);
                        triangles.Add(i + 1 + numOfCrossSectionVertices);
                    }
                }
            }

            return triangles;
        }

        public LineExtruder(List<Vector3> crossSectionShape, List<Vector3> crossSectionNormals, Vector3 crossSectionScale, bool generateCaps = true) {
            this.crossSectionShape = crossSectionShape;
            this.crossSectionNormals = crossSectionNormals;
            this.crossSectionScale = crossSectionScale;
            this.generateCaps = generateCaps;

            this.vertices = new List<Vector3>();
            this.normals = new List<Vector3>();
            this.triangles = new List<int>();
        }

        private List<Vector3> transformCrossSection(Vector3 position, Vector3 tangent) {
            return transformPoints(crossSectionShape, position, tangent, crossSectionScale);
        }

        public Mesh getMesh(List<Vector3> points) {

            return replacePoints(points, 0, points.Count, vertices.Count / crossSectionShape.Count);
        }

        /// <summary>
        /// Replaces and/or removes points from an existing mesh.
        /// This should only be called when getMesh was called before at least once.
        /// </summary>
        /// <param name="points">All points of the complete line.</param>
        /// <param name="index">First point to be added or removed.</param>
        /// <param name="addCount">Number of points to add at index.</param>
        /// <param name="removeCount">Number of points to remove at index before adding the new ones.</param>
        /// <returns></returns>
        public Mesh replacePoints(List<Vector3> points, int index, int addCount, int removeCount) {

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

            for (int i = pointIndex; i < pointIndex + pointAddCount; i++)
            {
                if (i == 0)
                {
                    (newVertices, newNormals) = transformCrossSection(points[i], points[i], points[i+1]);
                }
                else if (i == points.Count - 1)
                {
                    (newVertices, newNormals) = transformCrossSection(points[i-1], points[i], points[i]);
                }
                else {
                    (newVertices, newNormals) = transformCrossSection(points[i - 1], points[i], points[i + 1]);
                }
                
                verticesToInsert.AddRange(newVertices);
                normalsToInsert.AddRange(newNormals);
            }

            vertices.InsertRange(verticesIndex, verticesToInsert);
            normals.InsertRange(verticesIndex, normalsToInsert);

            //update triangles
            triangles = generateTriangles(crossSectionShape.Count, (vertices.Count / crossSectionShape.Count) - 1);

            //mesh is empty or there is just a single cross section left because second to last control point or last control point was removed
            if (vertices.Count <= crossSectionShape.Count && normals.Count <= crossSectionShape.Count) {
                return new Mesh();
            }

            Mesh mesh;

            if (generateCaps)
            {
                generateCapsMesh(vertices.GetRange(0, crossSectionShape.Count), vertices.GetRange(vertices.Count - crossSectionShape.Count, crossSectionShape.Count));
                mesh = getMeshWithCaps();
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
        private (List<Vector3>, List<Vector3>) transformCrossSection(Vector3 point1, Vector3 point2, Vector3 point3) {
            Vector3 tangent = ((point2 - point1).normalized + (point3 - point2).normalized).normalized;
            return (transformCrossSection(point2, tangent),transformNormals(crossSectionNormals, tangent));
        }

        private void generateCapsMesh(List<Vector3> firstCrossSectionVertices,List<Vector3> lastCrossSectionVertices) {

            capVertices = new List<Vector3>();
            capNormals = new List<Vector3>();
            capTriangles = new List<int>();
            //generate caps
            //begin cap
            List<int> beginCapTriangles = new List<int>();

            for (int i = 1; i <= firstCrossSectionVertices.Count - 3; i++)
            {
                int firstVertex = vertices.Count;

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
                int firstVertex = vertices.Count + capVertices.Count;

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
        }

        private Mesh getMeshWithCaps() {

            List<Vector3> allVertices = new List<Vector3>(vertices);
            allVertices.AddRange(capVertices);

            List<Vector3> allNormals = new List<Vector3>(normals);
            allNormals.AddRange(capNormals);

            List<int> allTriangles = new List<int>(triangles);
            allTriangles.AddRange(capTriangles);

            List<Vector2> uvs = TextureCoordinates.GenerateQuadrilateralUVsStretchU(vertices.Count, crossSectionShape.Count);

            uvs.AddRange(generateEndCapUVs(crossSectionShape.Count));

            Mesh mesh = new Mesh();

            mesh.SetVertices(allVertices);
            mesh.SetNormals(allNormals);
            mesh.subMeshCount = 1;
            mesh.SetTriangles(allTriangles.ToArray(), 0);
            mesh.SetUVs(0, uvs);

            return mesh;
        }

        private Mesh getMeshWithCaps(List<Vector3> vertices, List<Vector3> normals, List<int> triangles)
        {

            List<Vector3> allVertices = new List<Vector3>(vertices);
            allVertices.AddRange(capVertices);

            List<Vector3> allNormals = new List<Vector3>(normals);
            allNormals.AddRange(capNormals);

            List<int> allTriangles = new List<int>(triangles);
            allTriangles.AddRange(capTriangles);

            List<Vector2> uvs = TextureCoordinates.GenerateQuadrilateralUVsStretchU(vertices.Count, crossSectionShape.Count);

            uvs.AddRange(generateEndCapUVs(crossSectionShape.Count));

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
        private static List<Vector2> generateEndCapUVs(int crossSectionCount) {
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

        public Mesh getMeshParallelTransport(List<Vector3> points) {

            List<Vector3> tangents = getTangents(points);
            Vector3 initialNormal = Vector3.Cross(tangents[0], Vector3.right);
            if (initialNormal.magnitude == 0) {
                initialNormal = Vector3.Cross(tangents[0], Vector3.forward);
            }
            List<Vector3> normals = getSplineNormals(initialNormal,tangents);

            List<Vector3> meshVertices = new List<Vector3>();
            List<Vector3> meshNormals = new List<Vector3>();
            //transform cross sections and cross section normals
            //add to mesh, generate caps
            //get uvs and mesh tangents

            for (int i = 0; i < points.Count; i++)
            {
                List<Vector3> transformedCrossSection = transformPointsParallelTransport(crossSectionShape, points[i], tangents[i], normals[i], crossSectionScale);
                List<Vector3> transformedCrossSectionNormals = transformNormalsParallelTransport(crossSectionNormals, tangents[i], normals[i]);
                meshVertices.AddRange(transformedCrossSection);
                meshNormals.AddRange(transformedCrossSectionNormals);
            }

            //update triangles
            List<int> meshTriangles = generateTriangles(crossSectionShape.Count, (meshVertices.Count / crossSectionShape.Count) - 1);

            //mesh is empty or there is just a single cross section left because second to last control point or last control point was removed
            if (meshVertices.Count <= crossSectionShape.Count && meshNormals.Count <= crossSectionShape.Count)
            {
                return new Mesh();
            }

            Mesh mesh;

            if (generateCaps)
            {
                generateCapsMesh(meshVertices.GetRange(0, crossSectionShape.Count), meshVertices.GetRange(vertices.Count - crossSectionShape.Count, crossSectionShape.Count));
                mesh = getMeshWithCaps(meshVertices, meshNormals, meshTriangles);
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

        private static Vector3 GetTangent(Vector3 p1, Vector3 p2) {
            return (p2 - p1).normalized;
        }

        private static Vector3 GetTangent(Vector3 p1, Vector3 p2, Vector3 p3) {
            Vector3 tangent = ((p2 - p1).normalized + (p3 - p2).normalized).normalized;
            return tangent;
        }

        private List<Vector3> getTangents(List<Vector3> points) {
            List<Vector3> tangents = new List<Vector3>();
            tangents.Add(GetTangent(points[0], points[1]));
            for (int i = 1; i < points.Count-1; i++)
            {
                tangents.Add(GetTangent(points[i - 1], points[i + 1]));
            }
            tangents.Add(GetTangent(points[points.Count-2], points[points.Count-1]));
            return tangents;
        }

        private List<Vector3> getSplineNormals(Vector3 initialNormal, List<Vector3> tangents) {
            List<Vector3> normals = new List<Vector3>();
            Vector3 currentNormal = initialNormal;
            normals.Add(currentNormal);
            for (int i = 1; i < tangents.Count; i++)
            {
                Vector3 nextNormal = ParallelTransport(currentNormal, tangents[i-1], tangents[i]);
                currentNormal = nextNormal;
                normals.Add(nextNormal);
            }
            return normals;
        }

        /// <summary>
        /// Parallel transport step according to Hanson and Ma.
        /// </summary>
        /// <param name="currentNormal"></param>
        /// <param name="currentTangent"></param>
        /// <param name="nextTangent"></param>
        /// <returns>Normal for next tangent.</returns>
        private static Vector3 ParallelTransport(Vector3 currentNormal, Vector3 currentTangent, Vector3 nextTangent) {
            Vector3 nextNormal;
            Vector3 binormal = Vector3.Cross(currentTangent, nextTangent);
            if (binormal.magnitude == 0)
            {
                nextNormal = currentNormal;
            }
            else {
                binormal.Normalize();
                float theta = Mathf.Acos(Vector3.Dot(currentTangent, nextTangent));
                nextNormal = Quaternion.AngleAxis(Mathf.Rad2Deg * theta, binormal) * currentNormal;
            }
            return nextNormal;
        }

        public static List<Vector3> transformPointsParallelTransport(List<Vector3> points, Vector3 position, Vector3 tangent, Vector3 splineNormal, Vector3 scale)
        {

            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.LookRotation(splineNormal, tangent), scale);

            List<Vector3> pointsTransformed = new List<Vector3>();

            foreach (Vector3 point in points)
            {
                pointsTransformed.Add(matrix.MultiplyPoint3x4(point));
            }

            return pointsTransformed;
        }

        public static List<Vector3> transformNormalsParallelTransport(List<Vector3> normals, Vector3 tangent, Vector3 splineNormal)
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

