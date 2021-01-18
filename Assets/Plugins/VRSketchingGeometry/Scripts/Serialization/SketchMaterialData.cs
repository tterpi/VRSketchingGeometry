using UnityEngine;

namespace VRSketchingGeometry.Serialization
{

    public class SketchMaterialData
    {
        public enum ShaderType
        {
            Standard,
            TwoSided
        }

        public ShaderType Shader = ShaderType.Standard;

        public Color AlbedoColor = Color.white;
        public float MetallicValue = .0f;
        public float SmoothnessValue = .123f;
        public Vector2 UVTiling = Vector2.one;
        public string AlbedoMapName;
        public string MetallicMapName;
        public string NormalMapName;
        public string DisplacementMapName;

        public SketchMaterialData() { }

        public SketchMaterialData(Material material) {
            AlbedoColor = material.color;
            MetallicValue = material.GetFloat("_Metallic");
            UVTiling = material.GetTextureScale("_MainTex");
            AlbedoMapName = material.GetTexture("_MainTex")?.name;
            NormalMapName = material.GetTexture("_BumpMap")?.name;
            MetallicMapName = material.GetTexture("_MetallicGlossMap")?.name;
            DisplacementMapName = material.GetTexture("_ParallaxMap")?.name;

            if (MetallicMapName != null && MetallicMapName.Length > 0) {
                SmoothnessValue = material.GetFloat("_GlossMapScale");
            }
            else
            {
                SmoothnessValue = material.GetFloat("_Glossiness");
            }

        }

        public void ApplyMaterialProperties(in Material material) {
            material.color = AlbedoColor;
            material.SetFloat("_Metallic", MetallicValue);
            material.SetFloat("_Glossiness", SmoothnessValue);
            material.SetTextureScale("_MainTex", UVTiling);
        }
    }
}