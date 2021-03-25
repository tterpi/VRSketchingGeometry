using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VRSketchingGeometry.Meshing
{
    /// <summary>
    /// Generate vertices that approximate a circle.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class CircularCrossSection
    {
        /// <summary>
        /// Get vertices that approximate a circle.
        /// The cross section will have resolution + 1 vertices.
        /// </summary>
        /// <param name="resolution">Number of vertices of the circle.</param>
        /// <param name="scale">Radius of the cross section.</param>
        /// <returns></returns>
        public static List<Vector3> GenerateVertices(int resolution, float scale = .5f)
        {
            List<Vector3> vertices = new List<Vector3>();
            for (int i = 0; i < resolution; i++)
            {
                float angle = (float)i / resolution * 2 * Mathf.PI;
                Vector3 vertex = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * scale;
                vertices.Add(vertex);
            }
            //duplicate first vertex as last vertex
            vertices.Add(vertices[0]);
            return vertices;
        }

    }
}
