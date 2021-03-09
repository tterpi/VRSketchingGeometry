using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;
using System.Threading;
using System.Threading.Tasks;


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
            return GenerateVerticesOfPatch_Parallel(controlPoints, width, height, resolutionWidth, resolutionHeight);
        }

        public static Vector3[] GenerateVerticesOfPatch_Optimized(List<Vector3> controlPoints, int width, int height, int resolutionWidth, int resolutionHeight)
        {
            //create horizontal splines through the control points
            List<List<Vector3>> horizontalPoints = new List<List<Vector3>>();
            KochanekBartelsSpline horizontalSpline = new KochanekBartelsSpline(resolutionWidth);
            for (int i = 0; i < height; i++)
            {
                horizontalSpline.setControlPoints(controlPoints.GetRange(i * width, width).ToArray());
                horizontalPoints.Add(new List<Vector3>(horizontalSpline.InterpolatedPoints));
            }

            //create vertical splines through the generated interpolated points of the horizontal splines
            KochanekBartelsSpline verticalSpline = new KochanekBartelsSpline(resolutionHeight);
            List<Vector3> vertices = new List<Vector3>();

            for (int i = 0; i < (width - 1) * (resolutionWidth); i++)
            {
                List<Vector3> verticalControlPoints = new List<Vector3>();
                foreach (List<Vector3> horizontalPointList in horizontalPoints)
                {
                    verticalControlPoints.Add(horizontalPointList[i]);
                }
                verticalSpline.setControlPoints(verticalControlPoints.ToArray());
                vertices.AddRange(verticalSpline.InterpolatedPoints);
            }

            return vertices.ToArray();
        }

        public static Vector3[] GenerateVerticesOfPatch_Parallel(List<Vector3> controlPoints, int width, int height, int resolutionWidth, int resolutionHeight)
        {
            //create horizontal splines through the control points
            List<Vector3>[] horizontalPoints = new List<Vector3>[height];
            Parallel.For(0, height, (i) =>
            {
                KochanekBartelsSpline horizontalSpline = new KochanekBartelsSpline(resolutionWidth);
                horizontalSpline.setControlPoints(controlPoints.GetRange(i * width, width).ToArray());
                horizontalPoints[i] = new List<Vector3>(horizontalSpline.InterpolatedPoints);
            });

            //create vertical splines through the generated interpolated points of the horizontal splines
            List<Vector3> vertices = new List<Vector3>();

            List<Vector3>[] verticesLists = new List<Vector3>[(width - 1) * (resolutionWidth)];
            Parallel.For(0, (width - 1) * (resolutionWidth), (i) => {
                KochanekBartelsSpline verticalSpline = new KochanekBartelsSpline(resolutionHeight);
                List<Vector3> verticalControlPoints = new List<Vector3>();
                foreach (List<Vector3> horizontalPointList in horizontalPoints)
                {
                    verticalControlPoints.Add(horizontalPointList[i]);
                }
                verticalSpline.setControlPoints(verticalControlPoints.ToArray());

                verticesLists[i] = new List<Vector3>(verticalSpline.InterpolatedPoints);
            });

            foreach (var verticesList in verticesLists)
            {
                vertices.AddRange(verticesList);
            }

            return vertices.ToArray();
        }

        public static Vector3[] GenerateVerticesOfPatch_Original(List<Vector3> controlPoints, int width, int height, int resolutionWidth, int resolutionHeight)
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

        public static Mesh GeneratePatchMesh(List<Vector3> controlPoints, int width, int height, int resolutionWidth, int resolutionHeight)
        {
            Vector3[] vertices = GenerateVerticesOfPatch(controlPoints, width, height, resolutionWidth, resolutionHeight);

            int[] trianglesArray = Triangles.GenerateTrianglesClockwise((width - 1) * resolutionWidth, (height - 1) * resolutionHeight);

            Mesh patchMesh = new Mesh();
            patchMesh.SetVertices(vertices);
            patchMesh.SetTriangles(trianglesArray, 0);
            patchMesh.SetUVs(0, TextureCoordinates.GenerateQuadrilateralUVs(vertices.Length, (height - 1) * (resolutionHeight)));

            patchMesh.RecalculateNormals();
            patchMesh.RecalculateTangents();

            return patchMesh;
        }
    }
}