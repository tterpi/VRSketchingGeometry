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
        public float SmoothnessValue = .5f;
        public Vector2 UVTiling = Vector2.one;
        public string AlbedoMapName;
        public string MetallicMapName;
        public string NormalMapName;
        public string DisplacementMapName;
    }
}