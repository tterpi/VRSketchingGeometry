using Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochanekBartelsPatch : MonoBehaviour
{
    public int width = 4;
    public int height = 4;
    public int resolutionWidth = 4;
    public int resolutionHeight = 4;

    public List<GameObject> controlPointObjects;

    /// <summary>
    /// Generate the vertices of a patch surface from a grid of control points.
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <param name="width">Number of control points horizontally</param>
    /// <param name="height">Number of control point vertically</param>
    /// <param name="resolutionWidth">Number of points to generate between two control points horizontally</param>
    /// <param name="resolutionHeight">Number of points to generate between two control points vertically</param>
    /// <returns></returns>
    public static Vector3[] getVerticesOfPatch(List<Vector3> controlPoints, int width, int height, int resolutionWidth, int resolutionHeight) {

        //create horizontal splines through the control points
        List<KochanekBartelsSpline> horizontalSplines = new List<KochanekBartelsSpline>();
        for (int i = 0; i < height; i++) {
            KochanekBartelsSpline horizontalSpline = new KochanekBartelsSpline(resolutionWidth);
            horizontalSpline.setControlPoints(controlPoints.GetRange(i*width, width).ToArray());
            horizontalSplines.Add(horizontalSpline);
        }


        //create vertical splines through the generated interpolated points of the horizontal splines
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

        Vector3[] vertices = getVerticesOfPatch(controlPoints, width, height, resolutionWidth, resolutionHeight);
        //List<Vector3> normals = new List<Vector3>();

        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    normals.Add(Vector3.up);
        //}

        List<int> triangles = new List<int>();

        for (int i = 0; i < (width - 1) * resolutionWidth - 1; i++)
        {
            for (int y = 0; y < (height - 1) * resolutionHeight - 1; y++)
            {
                int index = i * (height - 1) * resolutionHeight + y;
                triangles.Add(index);
                triangles.Add(index + 1);
                triangles.Add(index + (height - 1) * resolutionHeight);

                triangles.Add(index + 1);
                triangles.Add(index + 1 + (height - 1) * resolutionHeight);
                triangles.Add(index + (height - 1) * resolutionHeight);
            }
        }

        Mesh patchMesh = new Mesh();
        patchMesh.SetVertices(vertices);
        //patchMesh.SetNormals(normals);
        patchMesh.SetTriangles(triangles.ToArray(), 0);

        patchMesh.RecalculateNormals();
        this.GetComponent<MeshFilter>().mesh = patchMesh;


    }
}
