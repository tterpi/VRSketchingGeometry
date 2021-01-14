using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Serialization
{
    [CreateAssetMenu(fileName = "SerializationDefaultValues", menuName = "ScriptableObjects/SerializationDefaultValues", order = 1)]
    public class DefaultValues : ScriptableObject
    {
        public Material StandardSketchMaterial;
        public Material TwoSidedSketchMaterial;
        public GameObject LineSketchObjectPrefab;
        public GameObject LinearInterpolationLineSketchObjectPrefab;
        public GameObject SketchObjectGroupPrefab;
        public GameObject SketchObjectSelectionPrefab;
        public GameObject PatchSketchObjectPrefab;

        public Material GetMaterial(SketchMaterial.ShaderType type)
        {
            switch (type)
            {
                case SketchMaterial.ShaderType.Standard:
                    return Instantiate(StandardSketchMaterial);
                case SketchMaterial.ShaderType.TwoSided:
                    return Instantiate(TwoSidedSketchMaterial);
                default:
                    Debug.LogError("ShaderType is unknown!");
                    return null;
            }
        }
    }
}