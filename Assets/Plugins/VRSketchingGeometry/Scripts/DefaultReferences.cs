using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry
{
    /// <summary>
    /// This scriptable object contains references to frequently used prefabs, materials etc.
    /// </summary>
    [CreateAssetMenu(fileName = "DefaultReferences", menuName = "ScriptableObjects/DefaultReferences", order = 1)]
    public class DefaultReferences : ScriptableObject
    {
        public Material StandardSketchMaterial;
        public Material TwoSidedSketchMaterial;
        public GameObject LineSketchObjectPrefab;
        public GameObject LinearInterpolationLineSketchObjectPrefab;
        public GameObject SketchObjectGroupPrefab;
        public GameObject SketchObjectSelectionPrefab;
        public GameObject PatchSketchObjectPrefab;
        public GameObject RibbonSketchObjectPrefab;

        public string DefaultTextureDirectory;

        private Dictionary<SketchMaterialData, Material> MaterialsDict = new Dictionary<SketchMaterialData, Material>();

        private void OnEnable()
        {
            DefaultTextureDirectory = System.IO.Path.Combine(Application.dataPath, "textures");
        }

        public Material GetMaterial(SketchMaterialData.ShaderType type)
        {
            switch (type)
            {
                case SketchMaterialData.ShaderType.Standard:
                    return Instantiate(StandardSketchMaterial);
                case SketchMaterialData.ShaderType.TwoSided:
                    return Instantiate(TwoSidedSketchMaterial);
                default:
                    Debug.LogError("ShaderType is unknown!");
                    return null;
            }
        }

        public Material GetMaterialFromDictionary(SketchMaterialData sketchMaterialData) {
            if (MaterialsDict.ContainsKey(sketchMaterialData))
            {
                return MaterialsDict[sketchMaterialData];
            }
            else {
                Material material = GetMaterial(sketchMaterialData.Shader);
                sketchMaterialData.ApplyMaterialProperties(material, DefaultTextureDirectory);
                MaterialsDict.Add(sketchMaterialData, material);
                return material;
            }
        }
    }
}