using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;


namespace VRSketchingGeometry.Meshing
{
    public class PatchMesh
    {
        /// <summary>
        /// Generate the vertices of a patch surface from a grid of control points.
        /// </summary>
        /// <param name="controlPoints"></param>
        /// <param name="width">Number of control points horizontally</param>
        /// <param name="height">Number of control point vertically</param>
        /// <param name="resolutionWidth">Number of points to generate between two control points horizontally</param>
        /// <param name="resolutionHeight">Number of points to generate between two control points vertically</param>
        /// <returns></returns>
        public static Vector3[] GenerateVerticesOfPatch(List<Vector3> controlPoints, int width, int height, int resolutionWidth, int resolutionHeight)
        {

            //create horizontal splines through the control points
            List<KochanekBartelsSpline> horizontalSplines = new List<KochanekBartelsSpline>();
            for (int i = 0; i < height; i++)
            {
                KochanekBartelsSpline horizontalSpline = new KochanekBartelsSpline(resolutionWidth);
                horizontalSpline.setControlPoints(controlPoints.GetRange(i * width, width).ToArray());
                horizontalSplines.Add(horizontalSpline);
            }

            //create vertical splines through the generated interpolated points of the horizontal splines
            List<KochanekBartelsSpline> verticalSplines = new List<KochanekBartelsSpline>();
            for (int i = 0; i < (width - 1) * (resolutionWidth); i++)
            {
                List<Vector3> verticalControlPoints = new List<Vector3>();
                foreach (KochanekBartelsSpline horizontalSpline in horizontalSplines)
                {
                    verticalControlPoints.Add(horizontalSpline.InterpolatedPoints[i]);
                }
                KochanekBartelsSpline verticalSpline = new KochanekBartelsSpline(resolutionHeight);
                verticalSpline.setControlPoints(verticalControlPoints.ToArray());
                verticalSplines.Add(verticalSpline);
            }

            List<Vector3> vertices = new List<Vector3>();

            foreach (KochanekBartelsSpline verticalSpline in verticalSplines)
            {
                vertices.AddRange(verticalSpline.InterpolatedPoints);
            }

            return vertices.ToArray();
        }

        /// <summary>
        /// Generate an indices array that defines a triangle topology for a quadrilateral patch
        /// </summary>
        /// <param name="numOfVerticalVertices"></param>
        /// <param name="numOfHorizontalVertices"></param>
        /// <returns></returns>
        public static int[] GenerateTrianglesOfPatch(int numOfVerticalVertices, int numOfHorizontalVertices)
        {
            List<int> triangles = new List<int>();

            for (int i = 0; i < numOfVerticalVertices - 1; i++)
            {
                for (int y = 0; y < numOfHorizontalVertices - 1; y++)
                {
                    int index = i * numOfHorizontalVertices + y;
                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + numOfHorizontalVertices);

                    triangles.Add(index + 1);
                    triangles.Add(index + 1 + numOfHorizontalVertices);
                    triangles.Add(index + numOfHorizontalVertices);
                }
            }
            return triangles.ToArray();
        }

        public static Mesh GeneratePatchMesh(List<Vector3> controlPoints, int width, int height, int resolutionWidth, int resolutionHeight)
        {
            Vector3[] vertices = GenerateVerticesOfPatch(controlPoints, width, height, resolutionWidth, resolutionHeight);

            int[] trianglesArray = GenerateTrianglesOfPatch((width - 1) * resolutionWidth, (height - 1) * resolutionHeight);

            Mesh patchMesh = new Mesh();
            patchMesh.SetVertices(vertices);
            patchMesh.SetTriangles(trianglesArray, 0);

            patchMesh.RecalculateNormals();

            return patchMesh;
        }
    }
}