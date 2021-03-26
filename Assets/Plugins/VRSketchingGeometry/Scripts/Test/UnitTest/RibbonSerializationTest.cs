using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.Commands.Ribbon;
using VRSketchingGeometry.Serialization;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.Utils;

namespace Tests
{
    public class RibbonSerializationTest
    {
        private RibbonSketchObject RibbonSketchObject;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.RibbonSketchObject = GameObject.FindObjectOfType<RibbonSketchObject>();
            yield return null;
        }

        [Test]
        public void GetData_ControlPoints()
        {
            this.RibbonSketchObject.transform.rotation = Quaternion.identity;
            this.RibbonSketchObject.SetControlPoints(
                new List<Vector3> { new Vector3(1, 2, 3), new Vector3(3, 2, 1), new Vector3(1, 1, 1), new Vector3(2, 2, 2) },
                new List<Quaternion> { Quaternion.Euler(0,0,0), Quaternion.Euler(0, 45, 0), Quaternion.Euler(25, 10, 5), Quaternion.Euler(0, 0, 35) }
                );
            RibbonSketchObjectData data = (this.RibbonSketchObject as ISerializableComponent).GetData() as RibbonSketchObjectData;
            Assert.AreEqual(new Vector3(1, 1, 1), data.ControlPoints[2]);
            Assert.AreEqual(Quaternion.Euler(25, 10, 5), data.ControlPointOrientations[2]);
        }

        [Test]
        public void GetData_CrossSection()
        {
            this.RibbonSketchObject.SetControlPoints(
                new List<Vector3> { new Vector3(1, 2, 3), new Vector3(3, 2, 1), new Vector3(1, 1, 1), new Vector3(2, 2, 2) },
                new List<Quaternion> { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 45, 0), Quaternion.Euler(25, 10, 5), Quaternion.Euler(0, 0, 35) }
                );
            this.RibbonSketchObject.SetCrossSection(new List<Vector3> { new Vector3(-.3f, 0, 0), new Vector3(.3f, 0, 0) }, Vector3.one);
            RibbonSketchObjectData data = (this.RibbonSketchObject as ISerializableComponent).GetData() as RibbonSketchObjectData;
            Assert.AreEqual(new Vector3(.3f,0,0), data.CrossSectionVertices[1]);
        }

        [Test]
        public void GetData_Position()
        {
            this.RibbonSketchObject.SetControlPoints(
                new List<Vector3> { new Vector3(1, 2, 3), new Vector3(3, 2, 1), new Vector3(1, 1, 1), new Vector3(2, 2, 2) },
                new List<Quaternion> { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 45, 0), Quaternion.Euler(25, 10, 5), Quaternion.Euler(0, 0, 35) }
                );
            this.RibbonSketchObject.SetCrossSection(new List<Vector3> { new Vector3(-.3f, 0, 0), new Vector3(.3f, 0, 0) }, Vector3.one);
            this.RibbonSketchObject.transform.position = new Vector3(1,5,3);
            RibbonSketchObjectData data = (this.RibbonSketchObject as ISerializableComponent).GetData() as RibbonSketchObjectData;
            Assert.AreEqual(new Vector3(1,5,3), data.Position);
        }

        [Test]
        public void GetData_Scale()
        {
            this.RibbonSketchObject.SetControlPoints(
                new List<Vector3> { new Vector3(1, 2, 3), new Vector3(3, 2, 1), new Vector3(1, 1, 1), new Vector3(2, 2, 2) },
                new List<Quaternion> { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 45, 0), Quaternion.Euler(25, 10, 5), Quaternion.Euler(0, 0, 35) }
                );
            this.RibbonSketchObject.SetCrossSection(new List<Vector3> { new Vector3(-.3f, 0, 0), new Vector3(.3f, 0, 0) }, Vector3.one);
            this.RibbonSketchObject.transform.localScale = new Vector3(1, 5, 3);
            RibbonSketchObjectData data = (this.RibbonSketchObject as ISerializableComponent).GetData() as RibbonSketchObjectData;
            Assert.AreEqual(new Vector3(1, 5, 3), data.Scale);
        }

        [Test]
        public void GetData_Rotation()
        {
            this.RibbonSketchObject.transform.rotation = Quaternion.Euler(20,40,60);
            RibbonSketchObjectData data = (this.RibbonSketchObject as ISerializableComponent).GetData() as RibbonSketchObjectData;
            Assert.That(data.Rotation, Is.EqualTo(Quaternion.Euler(20, 40, 60)).Using(QuaternionEqualityComparer.Instance));
        }

        [Test]
        public void ApplyData_ControlPoints() {
            this.RibbonSketchObject.transform.rotation = Quaternion.identity;
            RibbonSketchObjectData data = (this.RibbonSketchObject as ISerializableComponent).GetData() as RibbonSketchObjectData;
            data.ControlPoints = new List<Vector3> { new Vector3(1, 2, 3), new Vector3(3, 2, 1), new Vector3(1, 1, 1), new Vector3(2, 2, 2) };
            data.ControlPointOrientations = new List<Quaternion> { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 45, 0), Quaternion.Euler(25, 10, 5), Quaternion.Euler(0, 0, 35) };
            (this.RibbonSketchObject as ISerializableComponent).ApplyData(data);
            Assert.AreEqual(new Vector3(1, 1, 1), this.RibbonSketchObject.GetPoints()[2]);
            Assert.That(this.RibbonSketchObject.GetRotations()[2], Is.EqualTo(Quaternion.Euler(25, 10, 5)).Using(QuaternionEqualityComparer.Instance));
            Assert.AreEqual(4 * 3, this.RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertexCount);
        }

        [Test]
        public void ApplyData_CrossSection()
        {
            this.RibbonSketchObject.transform.rotation = Quaternion.identity;
            RibbonSketchObjectData data = (this.RibbonSketchObject as ISerializableComponent).GetData() as RibbonSketchObjectData;
            data.ControlPoints = new List<Vector3> { new Vector3(1, 2, 3), new Vector3(3, 2, 1), new Vector3(1, 1, 1), new Vector3(2, 2, 2) };
            data.ControlPointOrientations = new List<Quaternion> { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 45, 0), Quaternion.Euler(25, 10, 5), Quaternion.Euler(0, 0, 35) };
            data.CrossSectionVertices = new List<Vector3> { new Vector3(-.3f, 0, 0), new Vector3(.3f, 0, 0) };
            (this.RibbonSketchObject as ISerializableComponent).ApplyData(data);
            Assert.AreEqual(4 * 2, this.RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertexCount);
            Assert.AreEqual(new Vector3(-.3f, 0, 0), this.RibbonSketchObject.GetCrossSection()[0]);
        }

        [Test]
        public void ApplyData_Position()
        {
            RibbonSketchObjectData data = (this.RibbonSketchObject as ISerializableComponent).GetData() as RibbonSketchObjectData;
            data.Position = new Vector3(1, 2, 3);
            (this.RibbonSketchObject as ISerializableComponent).ApplyData(data);
            Assert.AreEqual(new Vector3(1,2,3), this.RibbonSketchObject.transform.position);
        }

        [Test]
        public void ApplyData_Scale()
        {
            RibbonSketchObjectData data = (this.RibbonSketchObject as ISerializableComponent).GetData() as RibbonSketchObjectData;
            data.Scale = new Vector3(1, 2, 3);
            (this.RibbonSketchObject as ISerializableComponent).ApplyData(data);
            Assert.AreEqual(new Vector3(1, 2, 3), this.RibbonSketchObject.transform.localScale);
        }

        [Test]
        public void ApplyData_Rotation()
        {
            RibbonSketchObjectData data = (this.RibbonSketchObject as ISerializableComponent).GetData() as RibbonSketchObjectData;
            data.Rotation = Quaternion.Euler(10,20,30);
            (this.RibbonSketchObject as ISerializableComponent).ApplyData(data);
            Assert.That(this.RibbonSketchObject.transform.rotation, Is.EqualTo(Quaternion.Euler(10, 20, 30)).Using(QuaternionEqualityComparer.Instance));
        }
    }
}
