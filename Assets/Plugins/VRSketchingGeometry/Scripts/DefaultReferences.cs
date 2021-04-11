using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry
{
    /// <summary>
    /// This scriptable object contains references to frequently used prefabs, materials etc.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    [CreateAssetMenu(fileName = "DefaultReferences", menuName = "ScriptableObjects/DefaultReferences", order = 1)]
    public class DefaultReferences : ScriptableObject
    {
        public Material StandardSketchMaterial;
        public Material TwoSidedSketchMaterial;
        public GameObject SketchWorldPrefab;
        public GameObject LineSketchObjectPrefab;
        public GameObject LinearInterpolationLineSketchObjectPrefab;
        public GameObject SketchObjectGroupPrefab;
        public GameObject SketchObjectSelectionPrefab;
        public GameObject PatchSketchObjectPrefab;
        public GameObject RibbonSketchObjectPrefab;

        /// <summary>
        /// The directory for textures that are referenced in a serialized sketch.
        /// </summary>
        public string DefaultTextureDirectory;

        private Dictionary<SketchMaterialData, Material> MaterialsDict = new Dictionary<SketchMaterialData, Material>();

        private void OnEnable()
        {
            DefaultTextureDirectory = System.IO.Path.Combine(Application.dataPath, "Textures");
        }

        /// <summary>
        /// Get a material object according to the shader type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get a material with the properties of the material data applied.
        /// </summary>
        /// <remarks>Returns an existing material instance if an identical material was created before.</remarks>
        /// <param name="sketchMaterialData"></param>
        /// <returns></returns>
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