using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.Serialization {
    public class Brush
    {
        public SketchMaterialData SketchMaterial;
    }

    public class LineBrush : Brush {
        public float CrossSectionScale;
        public List<Vector3> CrossSectionVertices;
        public List<Vector3> CrossSectionNormals;
    }

    public class RibbonBrush : Brush {
        public Vector3 CrossSectionScale;
        public List<Vector3> CrossSectionVertices;
    }
}
