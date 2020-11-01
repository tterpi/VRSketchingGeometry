using Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochanekBartelsPatch : MonoBehaviour
{
    public List<GameObject> controlPointObjects;

    public static Vector3[] getVerticesOfPatch(List<Vector3> controlPoints, int width, int height, int resolutionWidth, int resolutionHeight) {
        //Vector3[] vertices = new Vector3[resolutionHeight * resolutionWidth];

        List<KochanekBartelsSpline> horizontalSplines = new List<KochanekBartelsSpline>();
        for (int i = 0; i < height; i++) {
            KochanekBartelsSpline horizontalSpline = new KochanekBartelsSpline(resolutionWidth);
            horizontalSpline.setControlPoints(controlPoints.GetRange(i*width, width).ToArray());
            horizontalSplines.Add(horizontalSpline);
        }

        List<KochanekBartelsSpline> verticalSplines = new List<KochanekBartelsSpline>();
        for (int i = 0; i < (width-1) *(resolutionWidth); i++) {
            List<Vector3> verticalControlPoints = new List<Vector3>();
            foreach (KochanekBartelsSpline horizontalSpline in horizontalSplines) {
                verticalControlPoints.Add(horizontalSpline.InterpolatedPoints[i]);
            }
            KochanekBartelsSpline verticalSpline = new KochanekBartelsSpline(resolutionHeight);
            verticalSpline.setControlPoints(verticalControlPoints.ToArray());
            verticalSplines.Add(verticalSpline);
        }

        List<Vector3> vertices = new List<Vector3>();

        foreach (KochanekBartelsSpline verticalSpline in verticalSplines) {
            vertices.AddRange(verticalSpline.InterpolatedPoints);
        }

        return vertices.ToArray();
    }

    public void Start()
    {
        List<Vector3> controlPoints = new List<Vector3>();

        foreach (GameObject go in controlPointObjects) {
            controlPoints.Add(go.transform.position);
        }

        int width = 4;
        int height = 4;
        int resolutionWidth = 4;
        int resolutionHeight = 4;

        Vector3[] vertices = getVerticesOfPatch(controlPoints, width, height, 4, 4);
        List<Vector3> normals = new List<Vector3>();

        for (int i = 0; i < vertices.Length; i++)
        {
            normals.Add(Vector3.up);
        }

        List<int> triangles = new List<int>();

        for (int i = 0; i < (height -1)*resolutionHeight - 1; i++)
        {
            for (int y = 0; y < (width -1)*resolutionWidth - 1; y++)
            {
                int index = i * (width - 1) * resolutionWidth + y;
                triangles.Add(index);
                triangles.Add(index + 1);
                triangles.Add(index + (width - 1) * resolutionWidth);

                triangles.Add(index + 1);
                triangles.Add(index + 1 + (width - 1) * resolutionWidth);
                triangles.Add(index + (width - 1) * resolutionWidth);
            }
        }

        Mesh patchMesh = new Mesh();
        patchMesh.SetVertices(vertices);
        patchMesh.SetNormals(normals);
        patchMesh.SetTriangles(triangles.ToArray(), 0);

        patchMesh.RecalculateNormals();
        this.GetComponent<MeshFilter>().mesh = patchMesh;


    }
}
