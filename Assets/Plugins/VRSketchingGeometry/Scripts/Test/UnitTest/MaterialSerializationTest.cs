using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.Serialization;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class MaterialSerializationTest
    {
        private DefaultReferences Defaults;


        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.Defaults = GameObject.FindObjectOfType<SketchWorld>().defaults;
            yield return null;
        }

        [Test]
        public void ApplyMaterialProperties_AlbedoColor()
        {
            SketchMaterialData data = new SketchMaterialData();
            data.Shader = SketchMaterialData.ShaderType.Standard;
            data.AlbedoColor = Color.magenta;

            Material material = Defaults.GetMaterial(data.Shader);
            data.ApplyMaterialProperties(material, null);
            Assert.AreEqual(Color.magenta, material.GetColor("_Color"));
        }

        [Test]
        public void ApplyMaterialProperties_TwoSided_AlbedoColor()
        {
            SketchMaterialData data = new SketchMaterialData();
            data.Shader = SketchMaterialData.ShaderType.TwoSided;
            data.AlbedoColor = Color.magenta;

            Material material = Defaults.GetMaterial(data.Shader);
            data.ApplyMaterialProperties(material, null);
            Assert.AreEqual(Color.magenta, material.GetColor("_Color"));
        }

        [Test]
        public void ApplyMaterialProperties_Metallic()
        {
            SketchMaterialData data = new SketchMaterialData();
            data.Shader = SketchMaterialData.ShaderType.Standard;
            data.MetallicValue = .567f;

            Material material = Defaults.GetMaterial(data.Shader);
            data.ApplyMaterialProperties(material, null);
            Assert.AreEqual(.567f, material.GetFloat("_Metallic"));
        }

        [Test]
        public void ApplyMaterialProperties_Smoothness()
        {
            SketchMaterialData data = new SketchMaterialData();
            data.Shader = SketchMaterialData.ShaderType.Standard;
            data.SmoothnessValue = .567f;

            Material material = Defaults.GetMaterial(data.Shader);
            data.ApplyMaterialProperties(material, null);
            Assert.AreEqual(.567f, material.GetFloat("_Glossiness"));
            Assert.AreEqual(.567f, material.GetFloat("_GlossMapScale"));
        }

        [Test]
        public void ApplyMaterialProperties_TextureScale()
        {
            SketchMaterialData data = new SketchMaterialData();
            data.Shader = SketchMaterialData.ShaderType.Standard;
            data.UVTiling = new Vector2(2,5);

            Material material = Defaults.GetMaterial(data.Shader);
            data.ApplyMaterialProperties(material, null);
            Assert.AreEqual(new Vector2(2, 5), material.GetTextureScale("_MainTex"));
        }
    }
}
