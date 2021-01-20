using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VRSketchingGeometry.Serialization
{
    public class PatchSketchObjectData : SketchObjectData
    {
        public int Width;
        public int Height;
        public int ResolutionWidth;
        public int ResolutionHeight;
        public List<Vector3> ControlPoints;
        public SketchMaterialData Material;
    }
}
