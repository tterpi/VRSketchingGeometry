using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.Serialization;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.Commands.Group;
using VRSketchingGeometry.SketchObjectManagement;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.Utils;

namespace Tests
{
    public class GroupSerializationTest
    {
        private LineSketchObject Line;
        private PatchSketchObject Patch;
        private RibbonSketchObject Ribbon;
        private SketchObjectGroup Group;
        private CommandInvoker Invoker;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.Ribbon = GameObject.FindObjectOfType<RibbonSketchObject>();
            this.Patch = GameObject.FindObjectOfType<PatchSketchObject>();
            this.Line = GameObject.FindObjectOfType<LineSketchObject>();
            this.Group = GameObject.FindObjectOfType<SketchObjectGroup>();
            yield return null;
            Invoker = new CommandInvoker();
        }

        [Test]
        public void GetData_MultipleChildSketchObjects()
        {
            this.Group.AddToGroup(this.Ribbon);
            this.Group.AddToGroup(this.Patch);
            this.Group.AddToGroup(this.Line);

            SketchObjectGroupData data = (this.Group as ISerializableComponent).GetData() as SketchObjectGroupData;
            Assert.AreEqual(3, data.SketchObjects.Count);
        }

        [Test]
        public void GetData_ChildGroup() {
            SketchObjectGroup newGroup = GameObject.Instantiate(this.Group);
            this.Group.AddToGroup(newGroup);
            SketchObjectGroupData data = (this.Group as ISerializableComponent).GetData() as SketchObjectGroupData;
            Assert.AreEqual(1, data.SketchObjectGroups.Count);
        }

        [Test]
        public void GetData_Position() {
            this.Group.transform.position = new Vector3(1,2,3);
            SerializableComponentData data = (this.Group as ISerializableComponent).GetData();
            Assert.AreEqual(new Vector3(1, 2, 3), data.Position);
        }

        [Test]
        public void GetData_Scale()
        {
            this.Group.transform.localScale = new Vector3(1, 2, 3);
            SerializableComponentData data = (this.Group as ISerializableComponent).GetData();
            Assert.AreEqual(new Vector3(1, 2, 3), data.Scale);
        }

        [Test]
        public void GetData_Rotation()
        {
            this.Group.transform.rotation = Quaternion.Euler(10,20,30);
            SerializableComponentData data = (this.Group as ISerializableComponent).GetData();
            Assert.That(data.Rotation, Is.EqualTo(Quaternion.Euler(10, 20, 30)).Using(QuaternionEqualityComparer.Instance));
        }

        [Test]
        public void ApplyData_MultipleChildSketchObjects()
        {
            SketchObjectGroup newGroup = GameObject.Instantiate(this.Group);

            this.Group.AddToGroup(this.Ribbon);
            this.Group.AddToGroup(this.Patch);
            this.Group.AddToGroup(this.Line);

            SketchObjectGroupData data = (this.Group as ISerializableComponent).GetData() as SketchObjectGroupData;

            (newGroup as ISerializableComponent).ApplyData(data);

            Assert.AreEqual(3, newGroup.transform.childCount);
            Assert.AreEqual(1, newGroup.GetComponentsInChildren<RibbonSketchObject>().Length);
            Assert.AreEqual(1, newGroup.GetComponentsInChildren<LineSketchObject>().Length);
            Assert.AreEqual(1, newGroup.GetComponentsInChildren<PatchSketchObject>().Length);
        }

        [Test]
        public void ApplyData_ChildGroup() {
            SketchObjectGroup newGroup = GameObject.Instantiate(this.Group);
            SketchObjectGroup targetGroup = GameObject.Instantiate(this.Group);

            this.Group.AddToGroup(newGroup);

            SerializableComponentData data = (this.Group as ISerializableComponent).GetData();

            (targetGroup as ISerializableComponent).ApplyData(data);

            Assert.AreEqual(1, targetGroup.transform.childCount);
            Assert.AreEqual(2, targetGroup.GetComponentsInChildren<SketchObjectGroup>().Length);
        }

        [Test]
        public void ApplyData_TransformedChildGroup()
        {
            this.Group.transform.position = new Vector3(1, 2, 3);
            this.Group.transform.rotation = Quaternion.Euler(20, 30, 40);
            this.Group.transform.localScale = new Vector3(5, 5, 5);

            SketchObjectGroup newGroup = GameObject.Instantiate(this.Group);
            this.Group.AddToGroup(newGroup);
            newGroup.transform.position = new Vector3(3,2,1);
            newGroup.transform.rotation = Quaternion.Euler(80, 90, 100);
            newGroup.transform.localScale = new Vector3(.5f,.5f,.5f);

            SketchObjectGroup targetGroup = GameObject.Instantiate(this.Group);


            SerializableComponentData data = (this.Group as ISerializableComponent).GetData();

            (targetGroup as ISerializableComponent).ApplyData(data);

            SketchObjectGroup deserializedChildGroup = targetGroup.transform.GetChild(0).GetComponent<SketchObjectGroup>();
            Assert.AreEqual(new Vector3(3, 2, 1), deserializedChildGroup.transform.position);
            Assert.AreEqual(new Vector3(.5f, .5f, .5f), deserializedChildGroup.transform.localScale);
            Assert.That(deserializedChildGroup.transform.rotation, Is.EqualTo(Quaternion.Euler(80, 90, 100)).Using(QuaternionEqualityComparer.Instance));
        }
    }
}
