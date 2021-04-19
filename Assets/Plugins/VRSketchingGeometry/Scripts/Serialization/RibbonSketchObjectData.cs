using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Splines;

namespace VRSketchingGeometry.Serialization
{
    /// <summary>
    /// Contains the serialization data of a <see cref="VRSketchingGeometry.SketchObjectManagement.PatchSketchObject"/>.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class RibbonSketchObjectData : SketchObjectData
    {
        public List<Vector3> ControlPoints;
        public List<Quaternion> ControlPointOrientations;
        public List<Vector3> CrossSectionVertices;
        public Vector3 CrossSectionScale = Vector3.one;
        public SketchMaterialData SketchMaterial;

        internal override ISerializableComponent InstantiateComponent(DefaultReferences defaults)
        {
            return GameObject.Instantiate(defaults.RibbonSketchObjectPrefab).GetComponent<ISerializableComponent>();
        }
    }
}