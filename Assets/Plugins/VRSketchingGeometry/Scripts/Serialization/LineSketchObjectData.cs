using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;

namespace VRSketchingGeometry.Serialization
{
    public class LineSketchObjectData : SketchObjectData
    {
        public List<Vector3> ControlPoints;
        /// <summary>
        /// Currently not used.
        /// </summary>
        public List<Vector4> ControlPointOrientations;
        /// <summary>
        /// Currently not used.
        /// </summary>
        public List<KochanekBartelsControlPoint> KochanekBartelsControlPoints;
        public List<Vector3> CrossSectionVertices;
        public List<Vector3> CrossSectionNormals;
        public float CrossSectionScale = 1.0f;
        public SketchMaterial sketchMaterial;
    }
}