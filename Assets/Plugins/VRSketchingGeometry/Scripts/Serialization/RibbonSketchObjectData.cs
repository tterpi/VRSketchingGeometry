using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;

namespace VRSketchingGeometry.Serialization
{
    public class RibbonSketchObjectData : SketchObjectData
    {
        public List<Vector3> ControlPoints;
        public List<Quaternion> ControlPointOrientations;
        public List<Vector3> CrossSectionVertices;
        public List<Vector3> CrossSectionNormals;
        public Vector3 CrossSectionScale = Vector3.one;
        public SketchMaterialData SketchMaterial;
    }
}