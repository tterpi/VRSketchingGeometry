﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;

namespace VRSketchingGeometry.Serialization
{
    /// <summary>
    /// Contains the serialization data of a <see cref="VRSketchingGeometry.SketchObjectManagement.LineSketchObject"/>.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class LineSketchObjectData : SketchObjectData
    {
        public enum InterpolationType
        {
            Linear,
            Cubic
        }

        public InterpolationType Interpolation;
        public int InterpolationSteps;
        public List<Vector3> ControlPoints;
        public List<Vector3> CrossSectionVertices;
        public List<Vector3> CrossSectionNormals;
        public float CrossSectionScale = 1.0f;
        public SketchMaterialData SketchMaterial;
    }
}