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

            //first segment
            Vector3 tangent = (points[1] - points[0]);
            List<Vector3> firstCrossSectionVertices = transformCrossSection(points[0], tangent);
            List<Vector3> firstCrossSectionNormals = transformNormals(crossSectionNormals, tangent);
            vertices.AddRange(firstCrossSectionVertices);
            normals.AddRange(firstCrossSectionNormals);

            //middle segments
            for (int i = 1; i < points.Count - 1; i++)
            {
                tangent = (points[i] - points[i - 1]) + (points[i + 1] - points[i]) / 2f;
                vertices.AddRange(transformCrossSection(points[i], tangent));
                normals.AddRange(transformNormals(crossSectionNormals, tangent));
            }

            //last segment
            tangent = (points[points.Count - 1] - points[points.Count - 2]);
            List<Vector3> lastCrossSectionVertices = transformCrossSection(points[points.Count - 1], tangent);
            List<Vector3> lastCrossSectionNormals = transformNormals(crossSectionNormals, tangent);
            vertices.AddRange(lastCrossSectionVertices);
            normals.AddRange(lastCrossSectionNormals);

            triangles = generateTriangles(crossSectionShape.Count, points.Count - 1);

            Mesh mesh = new Mesh();

            if (generateCaps)
            {
                generateCapsMesh(firstCrossSectionVertices, lastCrossSectionVertices);
                mesh = getMeshWithCaps();

            }
            else {
                mesh.SetVertices(vertices);
                mesh.SetNormals(normals);
                mesh.subMeshCount = 1;
                mesh.SetTriangles(triangles.ToArray(), 0);
            }

            return mesh;
        }

        /// <summary>
        /// This will replace a section of the already calculated mesh by a new section calculated from the given points.
        /// This should only be called when getMesh was called before at least once.
        /// </summary>
        /// <param name="points">The new points of the line to insert.</param>
        /// <param name="replaceIndex">First index to be replaced by the new points.</param>
        /// <param name="numOfPoints">Number of points to be replaced.</param>
        /// <returns></returns>
        public Mesh replacePoints(List<Vector3> points, int replaceIndex, int numOfPoints) {
            int insertIndex = (replaceIndex) * crossSectionShape.Count;

            //remove invalid vertices and normals
            vertices.RemoveRange((replaceIndex) *  crossSectionShape.Count, numOfPoints * crossSectionShape.Count);
            normals.RemoveRange((replaceIndex) * crossSectionNormals.Count, numOfPoints * crossSectionShape.Count);

            //generate and add new vertices and normals
            List<Vector3> newVertices, newNormals, verticesToInsert, normalsToInsert;
            verticesToInsert = new List<Vector3>();
            normalsToInsert = new List<Vector3>();

            (newVertices, newNormals) = transformCrossSection(points[0], points[0], points[1]);

            verticesToInsert.AddRange(newVertices);
            normalsToInsert.AddRange(newNormals);

            for (int i = 1; i < points.Count-1; i++)
            {
                (newVertices, newNormals) = transformCrossSection(points[i-1], points[i], points[i+1]);

                verticesToInsert.AddRange(newVertices);
                normalsToInsert.AddRange(newNormals);
            }

            (newVertices, newNormals) = transformCrossSection(points[points.Count-2], points[points.Count-1], points[points.Count-1]);

            verticesToInsert.AddRange(newVertices);
            normalsToInsert.AddRange(newNormals);

            vertices.InsertRange(insertIndex, verticesToInsert);
            normals.InsertRange(insertIndex, normalsToInsert);

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
        /// The tangent at point2 is calculated with poin1 and point3.
        /// </summary>
        /// <param name="point1">The point before point2 in the line.</param>
        /// <param name="point2">The position of the calculated cross section.</param>
        /// <param name="point3">The point after point2 in the line.</param>
        /// <returns>(points, normals)</returns>
        private (List<Vector3>, List<Vector3>) transformCrossSection(Vector3 point1, Vector3 point2, Vector3 point3) {
            Vector3 tangent = (point2 - point1) + (point3 - point2) / 2f;
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

