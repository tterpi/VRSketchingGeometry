using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrudeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        //Mesh mesh = meshFilter.sharedMesh;
        Mesh mesh = new Mesh();


        List<Vector3> crossSectionShape = new List<Vector3> { new Vector3(1f, 0f, 0.5f), new Vector3(1f, 0f, -0.5f), new Vector3(0f, 0f, -1f), new Vector3(-1f, 0f, -0.5f), new Vector3(-1f, 0f, 0.5f), new Vector3(0f, 0f, 1f) };
        crossSectionShape.Reverse();
        List<Vector3> crossSectionShapeNormals = new List<Vector3>();
        foreach (Vector3 point in crossSectionShape) {
            crossSectionShapeNormals.Add(point.normalized);
        }

        //extrude transform
        Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0f, 5f, 1f), Quaternion.Euler(45f, 0, 0f), Vector3.one);
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(45f, 0, 0f));

        List<Vector3> crossSectionTransformed = new List<Vector3>();
        List<Vector3> normalsTransformed = new List<Vector3>();


        foreach (Vector3 point in crossSectionShape) {
            crossSectionTransformed.Add( matrix.MultiplyPoint3x4(point));
        }

        foreach (Vector3 normal in crossSectionShapeNormals)
        {
            normalsTransformed.Add(matrix.MultiplyVector(normal));
        }

        List<Vector3> vertices = new List<Vector3>(crossSectionShape);
        vertices.AddRange(crossSectionTransformed);
        //mesh.SetVertices(vertices);

        List<Vector3> normals = new List<Vector3>(crossSectionShapeNormals);
        normals.AddRange(normalsTransformed);
        //mesh.SetNormals(normals);

        List<int> triangles = new List<int>();

        for (int i = 0; i < crossSectionShape.Count; i++) {
            if (i % crossSectionShape.Count == crossSectionShape.Count - 1)
            {
                triangles.Add(i);
                triangles.Add(i + 1);
                triangles.Add(i - crossSectionShape.Count + 1);

                triangles.Add(i);
                triangles.Add(i + crossSectionShape.Count);
                triangles.Add(i + 1);
            }
            else {
                triangles.Add(i);
                triangles.Add(i + 1 + crossSectionShape.Count);
                triangles.Add(i + 1);

                triangles.Add(i);
                triangles.Add(i + crossSectionShape.Count);
                triangles.Add(i + 1 + crossSectionShape.Count);
            }
        }

        //generate caps
        //begin cap

        List<int> beginCapTriangles = new List<int>();

        for (int i = 1; i <= crossSectionShape.Count - 2; i++) {
            int firstVertex = vertices.Count;

            beginCapTriangles.Add(firstVertex);
            beginCapTriangles.Add(firstVertex + i);
            beginCapTriangles.Add(firstVertex + i + 1);
        }

        vertices.AddRange(crossSectionShape);

        Vector3 beginCapNormal = Vector3.Cross(crossSectionShape[1] - crossSectionShape[0], crossSectionShape[2] - crossSectionShape[0]).normalized;
        for (int i = 0; i < crossSectionShape.Count; i++) {
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

        vertices.AddRange(crossSectionTransformed);
        Vector3 endCapNormal = -Vector3.Cross(crossSectionTransformed[1] - crossSectionTransformed[0], crossSectionTransformed[2] - crossSectionTransformed[0]).normalized;
        for (int i = 0; i < crossSectionTransformed.Count; i++)
        {
            normals.Add(endCapNormal);
        }


        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.subMeshCount = 3;
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetTriangles(beginCapTriangles.ToArray(), 1);
        mesh.SetTriangles(endCapTriangles.ToArray(), 2);

        //mesh.UploadMeshData(false);
        meshFilter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
