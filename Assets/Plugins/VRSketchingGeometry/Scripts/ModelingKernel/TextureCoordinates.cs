using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Meshing {
    /// <summary>
    /// Methods for generating UV texture coordinates for Mesh objects.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class TextureCoordinates
    {
        /// <summary>
        /// Create texture coordinates for a quadrilateral mesh.
        /// </summary>
        /// <param name="verticesCount">Total number of vertices.</param>
        /// <param name="width">Number of vertices along the U axis.</param>
        /// <returns></returns>
        public static List<Vector2> GenerateQuadrilateralUVs(int verticesCount, int width)
        {
            List<Vector2> uvs = new List<Vector2>();
            for (int i = 0; i < verticesCount; i++)
            {
                //along the width
                float u = i % width;
                //along the height
                float v = i / width;
                uvs.Add(new Vector2(u, v));
            }
            return uvs;
        }

        /// <summary>
        /// Create texture coordinates for a quadrilateral mesh. Goes from 0 to 1 in U direction and repeats in V direction.
        /// </summary>
        /// <param name="verticesCount">Total number of vertices.</param>
        /// <param name="width">Number of vertices along the U axis.</param>
        /// <returns></returns>
        public static List<Vector2> GenerateQuadrilateralUVsStretchU(int verticesCount, int width)
        {
            List<Vector2> uvs = new List<Vector2>();
            for (int i = 0; i < verticesCount; i++)
            {
                //along the width
                float u = (float)(i % width) / (width - 1);
                //along the height
                //float v = (float)(i / width) / ( verticesCount / width);
                float v = (i / width);
                uvs.Add(new Vector2(u, v));
            }
            return uvs;
        }
    }
}
