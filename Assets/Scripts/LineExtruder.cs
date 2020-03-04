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

        public LineExtruder(List<Vector3> crossSectionShape, List<Vector3> crossSectionNormals, Vector3 crossSectionScale) {
            this.crossSectionShape = crossSectionShape;
            this.crossSectionNormals = crossSectionNormals;
            this.crossSectionScale = crossSectionScale;

            this.vertices = new List<Vector3>();
            this.normals = new List<Vector3>();
            this.triangles = new List<int>();
        }

        private List<Vector3> transformCrossSection(Vector3 position, Vector3 tangent) {
            return transformPoints(crossSectionShape, position, tangent, crossSectionScale);
        }

        public Mesh getMesh(List<Vector3> points, bool generateEndCaps = false) {

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

            List<int> triangles = generateTriangles(crossSectionShape.Count, points.Count - 1);

            Mesh mesh = new Mesh();

            if (generateEndCaps)
            {
                //generate caps
                //begin cap

                List<int> beginCapTriangles = new List<int>();

                for (int i = 1; i <= crossSectionShape.Count - 2; i++)
                {
                    int firstVertex = vertices.Count;

                    beginCapTriangles.Add(firstVertex);
                    beginCapTriangles.Add(firstVertex + i);
                    beginCapTriangles.Add(firstVertex + i + 1);
                }

                vertices.AddRange(firstCrossSectionVertices);

                Vector3 beginCapNormal = Vector3.Cross(firstCrossSectionVertices[1] - firstCrossSectionVertices[0], firstCrossSectionVertices[2] - firstCrossSectionVertices[0]).normalized;
                for (int i = 0; i < crossSectionShape.Count; i++)
                {
                    normals.Add(beginCapNormal);
                }

                //end cap

                List<int> endCapTriangles = new List<int>();

                for (int i = 1; i <= crossSectionShape.Count - 2; i++)
                {
                    int firstVertex = vertices.Count;

                    endCapTriangles.Add(firstVertex);
                    endCapTriangles.Add(firstVertex + i + 1);
                    endCapTriangles.Add(firstVertex + i);
                }

                vertices.AddRange(lastCrossSectionVertices);
                Vector3 endCapNormal = -Vector3.Cross(lastCrossSectionVertices[1] - lastCrossSectionVertices[0], lastCrossSectionVertices[2] - lastCrossSectionVertices[0]).normalized;
                for (int i = 0; i < lastCrossSectionVertices.Count; i++)
                {
                    normals.Add(endCapNormal);
                }

                mesh.SetVertices(vertices);
                mesh.SetNormals(normals);
                mesh.subMeshCount = 3;
                mesh.SetTriangles(triangles.ToArray(), 0);
                mesh.SetTriangles(beginCapTriangles.ToArray(), 1);
                mesh.SetTriangles(endCapTriangles.ToArray(), 2);

            }
            else {
                mesh.SetVertices(vertices);
                mesh.SetNormals(normals);
                mesh.subMeshCount = 1;
                mesh.SetTriangles(triangles.ToArray(), 0);
            }

            return mesh;
        }

    }
}

