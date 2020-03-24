using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meshing {
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

            for (int j = 0; j < numOfSections; j++)
            {
                for (int i = j * numOfCrossSectionVertices; i < (j + 1) * numOfCrossSectionVertices; i++)
                {
                    if (i % numOfCrossSectionVertices == numOfCrossSectionVertices - 1)
                    {
                        triangles.Add(i);
                        triangles.Add(i + 1);
                        triangles.Add(i - numOfCrossSectionVertices + 1);

                        triangles.Add(i);
                        triangles.Add(i + numOfCrossSectionVertices);
                        triangles.Add(i + 1);
                    }
                    else
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
        /// This will replace a section of the already calculated mesh by a new section calculated from the given points.
        /// This should only be called when getMesh was called before at least once.
        /// </summary>
        /// <param name="points">The new points of the line to insert.</param>
        /// <param name="index">First index to be replaced by the new points.</param>
        /// <param name="numOfPoints">Number of points to be replaced.</param>
        /// <returns></returns>
        public Mesh replacePoints(List<Vector3> points, int index, int addCount, int removeCount) {

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

            }
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

            for (int i = 1; i <= firstCrossSectionVertices.Count - 2; i++)
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

            for (int i = 1; i <= lastCrossSectionVertices.Count - 2; i++)
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

            Mesh mesh = new Mesh();

            mesh.SetVertices(allVertices);
            mesh.SetNormals(allNormals);
            mesh.subMeshCount = 1;
            mesh.SetTriangles(allTriangles.ToArray(), 0);

            return mesh;
        }
    }
}

