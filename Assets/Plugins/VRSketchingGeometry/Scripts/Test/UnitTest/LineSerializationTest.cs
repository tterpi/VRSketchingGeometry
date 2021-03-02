using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Serialization;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class LineSerializationTest
    {
        private LineSketchObject LineSketchObject;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.LineSketchObject = GameObject.FindObjectOfType<LineSketchObject>();
            yield return null;
        }

        [Test]
        public void GetData_ControlPoints()
        {
            this.LineSketchObject.SetControlPoints(new List<Vector3> { new Vector3(1, 2, 3), new Vector3(3, 2, 1), new Vector3(1, 1, 1), new Vector3(2, 2, 2) });
            LineSketchObjectData data = this.LineSketchObject.GetData() as LineSketchObjectData;
            Assert.AreEqual(new Vector3(3, 2, 1), data.ControlPoints[1]);
        }

        [Test]
        public void GetData_CrossSection()
        {
            List<Vector3> crossSection = new List<Vector3> { new Vector3(0, 0, 1), new Vector3(.5f, 0, 0), new Vector3(-.5f, 0, 0) };
            this.LineSketchObject.SetLineCrossSection(crossSection, crossSection, .3f);
            LineSketchObjectData data = this.LineSketchObject.GetData() as LineSketchObjectData;
            Assert.AreEqual(new Vector3(.5f, 0, 0), data.CrossSectionVertices[1]);
            Assert.AreEqual(new Vector3(0, 0, 1), data.CrossSectionVertices[0]);
            Assert.AreEqual(.3f, data.CrossSectionScale);
        }

        [Test]
        public void GetData_Position() {
            this.LineSketchObject.transform.position = new Vector3(1,2,3);
            LineSketchObjectData data = this.LineSketchObject.GetData() as LineSketchObjectData;
            Assert.AreEqual(new Vector3(1, 2, 3), data.Position);
        }

        [Test]
        public void GetData_Rotation()
        {
            this.LineSketchObject.transform.rotation = Quaternion.Euler(0,25,0);
            LineSketchObjectData data = this.LineSketchObject.GetData() as LineSketchObjectData;
            Assert.AreEqual(Quaternion.Euler(0, 25, 0), data.Rotation);
        }

        [Test]
        public void GetData_Scale()
        {
            this.LineSketchObject.transform.localScale = new Vector3(3,3,3);
            LineSketchObjectData data = this.LineSketchObject.GetData() as LineSketchObjectData;
            Assert.AreEqual(new Vector3(3, 3, 3), data.Scale);
        }
    }
}
