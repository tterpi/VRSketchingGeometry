using UnityEngine;

namespace VRSketchingGeometry.Serialization
{
    /// <summary>
    /// Data class for the properties of a material.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class SketchMaterialData : System.IEquatable<SketchMaterialData>
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

            if (material.shader.name == "Custom/TwoSidedSurfaceShader") {
                Shader = ShaderType.TwoSided;
            }

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

        public void ApplyMaterialProperties(in Material material, string textureBasePath) {
            material.color = AlbedoColor;
            material.SetFloat("_Metallic", MetallicValue);
            material.SetFloat("_Glossiness", SmoothnessValue);
            material.SetFloat("_GlossMapScale", SmoothnessValue);
            material.SetTextureScale("_MainTex", UVTiling);

            if (textureBasePath == null) return;

            if (AlbedoMapName != null) {
                Texture2D tex = LoadTextureFromPng(System.IO.Path.Combine(textureBasePath, AlbedoMapName + ".png"));
                material.SetTexture("_MainTex", tex);
            }

            if (NormalMapName != null) {
                material.EnableKeyword("_NORMALMAP");
                Texture2D normalTex = LoadTextureFromPng(System.IO.Path.Combine(textureBasePath, NormalMapName + ".png"));
                material.SetTexture("_BumpMap", normalTex);
            }

            if (MetallicMapName != null) {
                material.EnableKeyword("_METALLICGLOSSMAP");
                Texture2D metallicTex = LoadTextureFromPng(System.IO.Path.Combine(textureBasePath, MetallicMapName + ".png"));
                material.SetTexture("_MetallicGlossMap", metallicTex);
            }

            if (DisplacementMapName != null) {
                material.EnableKeyword("_PARALLAXMAP");
                Texture2D displacementTex = LoadTextureFromPng(System.IO.Path.Combine(textureBasePath, DisplacementMapName + ".png"));
                material.SetTexture("_ParallaxMap", displacementTex);
            }
        }

        public static Texture2D LoadTextureFromPng(string path) {
            Texture2D tex = new Texture2D(10, 10, TextureFormat.DXT5, false);
            try
            {
                ImageConversion.LoadImage(tex, System.IO.File.ReadAllBytes(path));
            }
            catch (System.Exception e)
            {
                Debug.LogError("An error occured while loading a texture from: " + path + "\n" + e.ToString());
                return null;
            }
            return tex;
        }

        public override int GetHashCode()
        {
            return 
                (Shader, 
                AlbedoColor, 
                MetallicValue, 
                SmoothnessValue, 
                AlbedoMapName, 
                MetallicMapName, 
                NormalMapName, 
                DisplacementMapName, 
                UVTiling)
                .GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is SketchMaterialData data)
            {
                if (this.Shader == data.Shader &&
                    this.AlbedoColor == data.AlbedoColor &&
                    this.MetallicValue == data.MetallicValue &&
                    this.SmoothnessValue == data.SmoothnessValue &&
                    this.AlbedoMapName == data.AlbedoMapName &&
                    this.MetallicMapName == data.MetallicMapName &&
                    this.NormalMapName == data.NormalMapName &&
                    this.DisplacementMapName == data.DisplacementMapName &&
                    this.UVTiling == data.UVTiling
                    )
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }

        public bool Equals(SketchMaterialData other)
        {
            return this.Equals(other as object);
        }
    }
}