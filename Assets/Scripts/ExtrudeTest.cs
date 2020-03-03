using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KochanekBartelsSplines;

public class ExtrudeTest : MonoBehaviour
{
    public float crossSectionScale = .3f;

    List<Vector3> transformPoints(List<Vector3> points, Vector3 position, Vector3 tangent) {

        Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.FromToRotation(Vector3.up, tangent), new Vector3(crossSectionScale, crossSectionScale, crossSectionScale));

        List<Vector3> pointsTransformed = new List<Vector3>();

        foreach (Vector3 point in points)
        {
            pointsTransformed.Add(matrix.MultiplyPoint3x4(point));
        }

        return pointsTransformed;
    }

    List<Vector3> transformNormals(List<Vector3> normals, Vector3 tangent)
    {

        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.FromToRotation(Vector3.up, tangent), Vector3.one);

        List<Vector3> normalsTransformed = new List<Vector3>();

        foreach (Vector3 normal in normals)
        {
            normalsTransformed.Add(matrix.MultiplyVector(normal));
        }

        return normalsTransformed;
    }

    List<int> generateTriangles(int numOfCrossSectionVertices, int numOfSections) {

        List<int> triangles = new List<int>();

        for (int j = 0; j < numOfSections; j++) {
            for (int i = j * numOfCrossSectionVertices; i < (j+1) * numOfCrossSectionVertices; i++)
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


    private Vector3[] ControlPoints;

    KochanekBartelsSpline kochanekBartelsSpline;

    [SerializeField]
    private GameObject[] ControlPointObjects;

    private List<Vector3> InterpolatedPoints;

    // Start is called before the first frame update
    void Start()
    {
        List<Vector3> crossSectionShape = new List<Vector3> { new Vector3(1f, 0f, 0.5f), new Vector3(1f, 0f, -0.5f), new Vector3(0f, 0f, -1f), new Vector3(-1f, 0f, -0.5f), new Vector3(-1f, 0f, 0.5f), new Vector3(0f, 0f, 1f) };
        crossSectionShape.Reverse();
        List<Vector3> crossSectionShapeNormals = new List<Vector3>();
        foreach (Vector3 point in crossSectionShape) {
            crossSectionShapeNormals.Add(point.normalized);
        }

        kochanekBartelsSpline = new KochanekBartelsSpline();

        ControlPoints = new Vector3[ControlPointObjects.Length];
        for (int i = 0; i < ControlPointObjects.Length; i++)
        {
            ControlPoints[i] = ControlPointObjects[i].transform.position;
        }

        InterpolatedPoints = kochanekBartelsSpline.InterpolatedPoints;
        kochanekBartelsSpline.setControlPoints(ControlPoints);

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();

        //first segment
        Vector3 tangent = (InterpolatedPoints[1] - InterpolatedPoints[0]);
        List<Vector3> firstCrossSectionVertices = transformPoints(crossSectionShape, InterpolatedPoints[0], tangent);
        List<Vector3> firstCrossSectionNormals = transformNormals(crossSectionShapeNormals, tangent);
        vertices.AddRange(firstCrossSectionVertices);
        normals.AddRange(firstCrossSectionNormals);

        //middle segments
        for (int i = 1; i < InterpolatedPoints.Count - 1; i++) {
            tangent = (InterpolatedPoints[i] - InterpolatedPoints[i - 1]) + (InterpolatedPoints[i + 1] - InterpolatedPoints[i]) / 2f;
            vertices.AddRange(transformPoints(crossSectionShape, InterpolatedPoints[i], tangent));
            normals.AddRange(transformNormals(crossSectionShapeNormals, tangent));
        }

        //last segment
        tangent = (InterpolatedPoints[InterpolatedPoints.Count-1] - InterpolatedPoints[InterpolatedPoints.Count-2]);
        List<Vector3> lastCrossSectionVertices = transformPoints(crossSectionShape, InterpolatedPoints[InterpolatedPoints.Count - 1], tangent);
        List<Vector3> lastCrossSectionNormals = transformNormals(crossSectionShapeNormals, tangent);
        vertices.AddRange(lastCrossSectionVertices);
        normals.AddRange(lastCrossSectionNormals);

        List<int> triangles = generateTriangles(crossSectionShape.Count, InterpolatedPoints.Count-1);

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


        MeshFilter meshFilter = GetComponent<MeshFilter>();
        //Mesh mesh = meshFilter.sharedMesh;
        Mesh mesh = new Mesh();

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.subMeshCount = 3;
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetTriangles(beginCapTriangles.ToArray(), 1);
        mesh.SetTriangles(endCapTriangles.ToArray(), 2);

        //mesh.UploadMeshData(false);
        meshFilter.mesh = mesh;
    }
}
