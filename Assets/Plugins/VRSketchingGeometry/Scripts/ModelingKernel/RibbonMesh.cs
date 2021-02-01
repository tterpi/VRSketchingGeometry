using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VRSketchingGeometry.Meshing
{
    class RibbonMesh
    {

        public static Mesh GetRibbonMesh(List<Vector3> points, List<Quaternion> rotations, float width) {

            List<Vector3> ribbonCrossSection = new List<Vector3> { new Vector3(0, 0, .5f), new Vector3(0, 0, 0), new Vector3(0, 0, -.5f) };
            return GetMesh(ribbonCrossSection, points, rotations, Vector3.one * width);
        }

        public static Mesh GetMesh(List<Vector3> crossSection, List<Vector3> points, List<Quaternion> rotations, Vector3 scale) {

            List<Vector3> vertices = new List<Vector3>();

            for (int i = 0; i < points.Count; i++)
            {
                List<Vector3> transformedCrossSection = new List<Vector3>();
                for (int j = 0; j < crossSection.Count; j++)
                {
                    transformedCrossSection.Add(TransformPoint(crossSection[j], points[i], rotations[i], scale));
                }

                vertices.AddRange(transformedCrossSection);

            }

            List<int> triangles = LineExtruder.GenerateTriangles(crossSection.Count, points.Count-1);

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.RecalculateNormals();
            mesh.SetUVs(0, TextureCoordinates.GenerateQuadrilateralUVsStretchU(vertices.Count, crossSection.Count));
            mesh.RecalculateTangents();

            return mesh;
        }

        public static Vector3 TransformPoint(Vector3 point, Vector3 position, Quaternion rotation, Vector3 scale) {
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
            return matrix.MultiplyPoint3x4(point);
        }

    }
}
