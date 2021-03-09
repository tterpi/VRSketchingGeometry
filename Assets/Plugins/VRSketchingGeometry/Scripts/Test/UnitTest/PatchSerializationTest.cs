using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.Commands.Patch;
using VRSketchingGeometry.Serialization;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class PatchSerializationTest
    {
        private PatchSketchObject PatchSketchObject;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.PatchSketchObject = GameObject.FindObjectOfType<PatchSketchObject>();
            yield return null;
        }

        [Test]
        public void GetData_ControlPoints()
        {
            List<Vector3> segment1 = new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0), new Vector3(3, 0, 0) };
            List<Vector3> segment2 = new List<Vector3> { new Vector3(0, 0, 1), new Vector3(1, 1, 1), new Vector3(2, 0, 1), new Vector3(3, 0, 1) };
            List<Vector3> segment3 = new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 0, 2), new Vector3(2, 1, 2), new Vector3(3, 0, 2) };
            List<Vector3> segment4 = new List<Vector3> { new Vector3(0, 1, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3), new Vector3(3, 0, 3) };

            this.PatchSketchObject.Width = 4;

            this.PatchSketchObject.AddPatchSegment(segment1);
            this.PatchSketchObject.AddPatchSegment(segment2);
            this.PatchSketchObject.AddPatchSegment(segment3);
            this.PatchSketchObject.AddPatchSegment(segment4);

            PatchSketchObjectData data = this.PatchSketchObject.GetData() as PatchSketchObjectData;

            Assert.AreEqual(4 * 4, data.ControlPoints.Count);
            Assert.AreEqual(4, data.Width);
            Assert.AreEqual(4, data.Height);
        }

        [Test]
        public void ApplyData_ControlPoints() {

            List<Vector3> segment1 = new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0), new Vector3(3, 0, 0) };
            List<Vector3> segment2 = new List<Vector3> { new Vector3(0, 0, 1), new Vector3(1, 1, 1), new Vector3(2, 0, 1), new Vector3(3, 0, 1) };
            List<Vector3> segment3 = new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 0, 2), new Vector3(2, 1, 2), new Vector3(3, 0, 2) };
            List<Vector3> segment4 = new List<Vector3> { new Vector3(0, 1, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3), new Vector3(3, 0, 3) };

            PatchSketchObjectData data = new PatchSketchObjectData();
            data.ControlPoints = new List<Vector3>();
            data.ControlPoints.AddRange(segment1);
            data.ControlPoints.AddRange(segment2);
            data.ControlPoints.AddRange(segment3);
            data.ControlPoints.AddRange(segment4);
            data.Width = 4;
            data.Height = 4;
            data.ResolutionHeight = 6;
            data.ResolutionWidth = 6;
            data.SketchMaterial = new SketchMaterialData();

            this.PatchSketchObject.ApplyData(data);
            Assert.That(this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh.vertexCount, Is.EqualTo(3 * 6 * 3 * 6));
        }
    }
}
