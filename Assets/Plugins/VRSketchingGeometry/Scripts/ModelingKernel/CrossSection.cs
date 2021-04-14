using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Meshing
{
    /// <summary>
    /// Represents a cross section for a ITubeMesh implementation.
    /// </summary>
    public class CrossSection
    {
        public List<Vector3> Vertices;
        public List<Vector3> Normals;
        public Vector3 Scale;

        public CrossSection(List<Vector3> vertices, List<Vector3> normals, Vector3 scale) {
            this.Vertices = vertices;
            this.Normals = normals;
            this.Scale = scale;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="crossSection"></param>
        public CrossSection(CrossSection crossSection) {
            this.Vertices = new List<Vector3>(crossSection.Vertices);
            this.Normals = new List<Vector3>(crossSection.Normals);
            this.Scale = crossSection.Scale;
        }
    }
}
