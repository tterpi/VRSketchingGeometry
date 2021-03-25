using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRSketchingGeometry.Meshing
{
    /// <summary>
    /// Methods for generating triangle index arrays for Mesh objects. 
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class Triangles
    {
        /// <summary>
        /// Generate an indices array that defines a quadrilateral triangle topology.
        /// The order of the vertices of a triangle is clockwise when the vertices are ordered left to right, top to bottom.
        /// </summary>
        /// <param name="numOfVerticalVertices"></param>
        /// <param name="numOfHorizontalVertices"></param>
        /// <returns></returns>
        public static int[] GenerateTrianglesClockwise(int numOfVerticalVertices, int numOfHorizontalVertices)
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

        /// <summary>
        /// Generate an indices array that defines a quadrilateral triangle topology.
        /// The order of the vertices of a triangle is counter-clockwise when the vertices are ordered left to right, top to bottom.
        /// </summary>
        /// <param name="numOfVerticalVertices"></param>
        /// <param name="numOfHorizontalVertices"></param>
        /// <returns></returns>
        public static int[] GenerateTrianglesCounterclockwise(int numOfVerticalVertices, int numOfHorizontalVertices)
        {
            List<int> triangles = new List<int>();

            for (int i = 0; i < numOfVerticalVertices - 1; i++)
            {
                for (int y = 0; y < numOfHorizontalVertices - 1; y++)
                {
                    int index = i * numOfHorizontalVertices + y;
                    triangles.Add(index + numOfHorizontalVertices);
                    triangles.Add(index + 1);
                    triangles.Add(index);

                    triangles.Add(index + numOfHorizontalVertices);
                    triangles.Add(index + 1 + numOfHorizontalVertices);
                    triangles.Add(index + 1);
                }
            }
            return triangles.ToArray();
        }
    }
}
