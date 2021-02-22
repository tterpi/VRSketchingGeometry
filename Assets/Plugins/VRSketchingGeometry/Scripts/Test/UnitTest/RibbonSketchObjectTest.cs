using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.Commands.Ribbon;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class RibbonSketchObjectTest
    {
        private RibbonSketchObject RibbonSketchObject;
        private CommandInvoker Invoker;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.RibbonSketchObject = GameObject.FindObjectOfType<RibbonSketchObject>();
            yield return null;
            Invoker = new CommandInvoker();
        }

        [Test]
        public void AddControlPointAndRotationCommand()
        {
            ICommand addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(0, 0, 0), Quaternion.Euler(45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(1, 0, 0), Quaternion.Euler(-45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            Assert.AreEqual(6, RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void AddControlPointAndRotationCommandUndo()
        {
            ICommand addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(0, 0, 0), Quaternion.Euler(45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(1, 0, 0), Quaternion.Euler(-45, 0, 0));
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();

            Assert.AreEqual(3, RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void AddControlPointAndRotationCommandRedo()
        {
            ICommand addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(0, 0, 0), Quaternion.Euler(45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(1, 0, 0), Quaternion.Euler(-45, 0, 0));
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.AreEqual(6, RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void DeleteControlPointAndRotationCommand()
        {
            ICommand addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(0, 0, 0), Quaternion.Euler(45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(1, 0, 0), Quaternion.Euler(-45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            ICommand deleteCommand = new DeletePointAndRotationCommand(this.RibbonSketchObject);
            Invoker.ExecuteCommand(deleteCommand);

            Assert.AreEqual(3, RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void DeleteControlPointAndRotationCommandUndo()
        {
            ICommand addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(0, 0, 0), Quaternion.Euler(45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(1, 0, 0), Quaternion.Euler(-45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            ICommand deleteCommand = new DeletePointAndRotationCommand(this.RibbonSketchObject);
            Invoker.ExecuteCommand(deleteCommand);
            Invoker.Undo();

            Assert.AreEqual(6, RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void DeleteControlPointAndRotationCommandRedo()
        {
            ICommand addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(0, 0, 0), Quaternion.Euler(45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(1, 0, 0), Quaternion.Euler(-45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            ICommand deleteCommand = new DeletePointAndRotationCommand(this.RibbonSketchObject);
            Invoker.ExecuteCommand(deleteCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.AreEqual(3, RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void AddControlPointAndRotationContinuousCommand()
        {
            ICommand addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(0, 0, 0), Quaternion.Euler(45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(1, 0, 0), Quaternion.Euler(-45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationContinuousCommand(this.RibbonSketchObject, new Vector3(2, 0, 0), Quaternion.identity);
            Invoker.ExecuteCommand(addCommand);

            Assert.AreEqual(9, RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void AddControlPointAndRotationContinuousCommandUndo()
        {
            ICommand addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(0, 0, 0), Quaternion.Euler(45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(1, 0, 0), Quaternion.Euler(-45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationContinuousCommand(this.RibbonSketchObject, new Vector3(2, 0, 0), Quaternion.identity);
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();

            Assert.AreEqual(6, RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void AddControlPointAndRotationContinuousCommandRedo()
        {
            ICommand addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(0, 0, 0), Quaternion.Euler(45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(1, 0, 0), Quaternion.Euler(-45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationContinuousCommand(this.RibbonSketchObject, new Vector3(2, 0, 0), Quaternion.identity);
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.AreEqual(9, RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void AddControlPointAndRotationContinuousCommand_DistanceTooSmall()
        {
            ICommand addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(0, 0, 0), Quaternion.Euler(45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.RibbonSketchObject, new Vector3(1, 0, 0), Quaternion.Euler(-45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationContinuousCommand(this.RibbonSketchObject, new Vector3(1.001f, 0, 0), Quaternion.identity);
            Invoker.ExecuteCommand(addCommand);

            Assert.AreEqual(6, RibbonSketchObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }
    }
}
