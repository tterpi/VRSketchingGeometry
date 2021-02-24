using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.Commands.Patch;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class PatchSketchObjectCommandTest
    {
        private PatchSketchObject PatchSketchObject;
        private CommandInvoker Invoker;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.PatchSketchObject = GameObject.FindObjectOfType<PatchSketchObject>();
            yield return null;
            Invoker = new CommandInvoker();
        }

        [Test]
        public void AddSegmentsCommand()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject, 
                new List<Vector3> { new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(2,0,0) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 1, 2), new Vector3(2, 0, 2) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3) });
            Invoker.ExecuteCommand(addCommand);

            Assert.AreEqual(8*8,this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void AddSegmentsCommandUndo()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 1, 2), new Vector3(2, 0, 2) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3) });
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();

            Assert.AreEqual(null, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh);
        }

        [Test]
        public void AddSegmentsCommandRedo()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 1, 2), new Vector3(2, 0, 2) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3) });
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.AreEqual(8 * 8, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void AddFirstSegmentCommand()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            Assert.AreEqual(null, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh);
            Assert.AreEqual(3, this.PatchSketchObject.GetControlPointsCount());
        }

        [Test]
        public void AddFirstSegmentCommandUndo()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();

            Assert.AreEqual(null, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh);
            Assert.AreEqual(0, this.PatchSketchObject.GetControlPointsCount());
            Assert.IsTrue(SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.PatchSketchObject));
        }

        [Test]
        public void AddFirstSegmentCommandRedo()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.AreEqual(null, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh);
            Assert.AreEqual(3, this.PatchSketchObject.GetControlPointsCount());
            Assert.IsFalse(SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.PatchSketchObject));
        }

        [Test]
        public void DeleteSegmentCommand()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 1, 2), new Vector3(2, 0, 2) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3) });
            Invoker.ExecuteCommand(addCommand);

            DeleteSegmentCommand deleteCommand = new DeleteSegmentCommand(this.PatchSketchObject);
            Invoker.ExecuteCommand(deleteCommand);

            Assert.AreEqual(null, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh);
            Assert.AreEqual(6, this.PatchSketchObject.GetControlPoints().Count);
        }

        [Test]
        public void DeleteSegmentCommandUndo()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 1, 2), new Vector3(2, 0, 2) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3) });
            Invoker.ExecuteCommand(addCommand);

            DeleteSegmentCommand deleteCommand = new DeleteSegmentCommand(this.PatchSketchObject);
            Invoker.ExecuteCommand(deleteCommand);
            Invoker.Undo();

            Assert.AreEqual(8*8, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
            Assert.AreEqual(9, this.PatchSketchObject.GetControlPoints().Count);
        }

        [Test]
        public void DeleteSegmentCommandRedo()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 1, 2), new Vector3(2, 0, 2) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3) });
            Invoker.ExecuteCommand(addCommand);

            DeleteSegmentCommand deleteCommand = new DeleteSegmentCommand(this.PatchSketchObject);
            Invoker.ExecuteCommand(deleteCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.AreEqual(null, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh);
            Assert.AreEqual(6, this.PatchSketchObject.GetControlPoints().Count);
        }

        [Test]
        public void DeleteFirstSegmentCommand()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            DeleteSegmentCommand deleteCommand = new DeleteSegmentCommand(this.PatchSketchObject);
            Invoker.ExecuteCommand(deleteCommand);

            Assert.AreEqual(null, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh);
            Assert.AreEqual(0, this.PatchSketchObject.GetControlPoints().Count);
            Assert.IsTrue(SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.PatchSketchObject));
        }

        [Test]
        public void DeleteFirstSegmentCommandUndo()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            DeleteSegmentCommand deleteCommand = new DeleteSegmentCommand(this.PatchSketchObject);
            Invoker.ExecuteCommand(deleteCommand);
            Invoker.Undo();

            Assert.AreEqual(null, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh);
            Assert.AreEqual(3, this.PatchSketchObject.GetControlPoints().Count);
            Assert.IsFalse(SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.PatchSketchObject));
        }

        [Test]
        public void DeleteFirstSegmentCommandRedo()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            AddSegmentCommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            DeleteSegmentCommand deleteCommand = new DeleteSegmentCommand(this.PatchSketchObject);
            Invoker.ExecuteCommand(deleteCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.AreEqual(null, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh);
            Assert.AreEqual(0, this.PatchSketchObject.GetControlPoints().Count);
            Assert.IsTrue(SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.PatchSketchObject));
        }

        [Test]
        public void AddSegmentContinuousCommand()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            ICommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 1, 2), new Vector3(2, 0, 2) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentContinuousCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3) });
            Invoker.ExecuteCommand(addCommand);

            Assert.AreEqual(8*8, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
            Assert.AreEqual(9, this.PatchSketchObject.GetControlPoints().Count);
        }

        [Test]
        public void AddSegmentContinuousCommand2()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            ICommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 1, 2), new Vector3(2, 0, 2) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentContinuousCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3.5f), new Vector3(1, 0, 3.5f), new Vector3(2, 0, 3.5f) });
            Invoker.ExecuteCommand(addCommand);

            Assert.AreEqual(8 * 12, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
            Assert.AreEqual(12, this.PatchSketchObject.GetControlPoints().Count);
        }

        [Test]
        public void AddSegmentContinuousCommandUndo()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            ICommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 1, 2), new Vector3(2, 0, 2) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentContinuousCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3.5f), new Vector3(1, 0, 3.5f), new Vector3(2, 0, 3.5f) });
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();

            Assert.AreEqual(8 * 8, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
            Assert.AreEqual(9, this.PatchSketchObject.GetControlPoints().Count);
        }

        [Test]
        public void AddSegmentContinuousCommandRedo()
        {
            this.PatchSketchObject.Width = 3;
            this.PatchSketchObject.ResolutionWidth = 4;
            this.PatchSketchObject.ResolutionHeight = 4;

            ICommand addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 1, 2), new Vector3(2, 0, 2) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3) });
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddSegmentContinuousCommand(this.PatchSketchObject,
                new List<Vector3> { new Vector3(0, 0, 3.5f), new Vector3(1, 0, 3.5f), new Vector3(2, 0, 3.5f) });
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.AreEqual(8 * 12, this.PatchSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
            Assert.AreEqual(12, this.PatchSketchObject.GetControlPoints().Count);
        }
    }
}
