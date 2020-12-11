using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Meshing {
    public class TextureCoordinates
    {
        /// <summary>
        /// Create texture coordinates for a quadrilateral mesh.
        /// </summary>
        /// <param name="verticesCount">Total number of vertices.</param>
        /// <param name="width">Number of vertices along the x axis.</param>
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
    }
}
