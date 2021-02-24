using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.Commands.Selection;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class SketchObjectSelectionTest
    {
        private RibbonSketchObject Ribbon;
        private SketchObjectSelection Selection;
        private CommandInvoker Invoker;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.Ribbon = GameObject.FindObjectOfType<RibbonSketchObject>();
            this.Selection = GameObject.FindObjectOfType<SketchObjectSelection>();
            yield return null;
            Invoker = new CommandInvoker();
        }

        [Test]
        public void ActivateSelectionCommandTest()
        {
            GameObject bounds = null;
            foreach (Transform transform in this.Selection.transform)
            {
                if (transform.name == "BoundsVisualizationObject") {
                    bounds = transform.gameObject;
                }
            }

            Assert.IsFalse(bounds.activeInHierarchy);
            this.Selection.AddToSelection(this.Ribbon);
            ICommand activateCommand = new ActivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(activateCommand);

            Assert.IsTrue(this.Ribbon.transform.IsChildOf(this.Selection.transform));
            Assert.IsTrue(bounds.activeInHierarchy);
            Assert.AreEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);

        }

        [Test]
        public void ActivateSelectionCommandTestUndo()
        {
            GameObject bounds = null;
            foreach (Transform transform in this.Selection.transform)
            {
                if (transform.name == "BoundsVisualizationObject")
                {
                    bounds = transform.gameObject;
                }
            }

            Assert.IsFalse(bounds.activeInHierarchy);
            this.Selection.AddToSelection(this.Ribbon);
            ICommand activateCommand = new ActivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(activateCommand);
            Invoker.Undo();

            Assert.IsFalse(this.Ribbon.transform.IsChildOf(this.Selection.transform));
            Assert.IsFalse(bounds.activeInHierarchy);
            Assert.AreNotEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);

        }

        [Test]
        public void ActivateSelectionCommandTestRedo()
        {
            GameObject bounds = null;
            foreach (Transform transform in this.Selection.transform)
            {
                if (transform.name == "BoundsVisualizationObject")
                {
                    bounds = transform.gameObject;
                }
            }

            Assert.IsFalse(bounds.activeInHierarchy);
            this.Selection.AddToSelection(this.Ribbon);
            ICommand activateCommand = new ActivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(activateCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.IsTrue(this.Ribbon.transform.IsChildOf(this.Selection.transform));
            Assert.IsTrue(bounds.activeInHierarchy);
            Assert.AreEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);

        }

        [Test]
        public void DeactivateSelectionCommandTest()
        {
            GameObject bounds = null;
            foreach (Transform transform in this.Selection.transform)
            {
                if (transform.name == "BoundsVisualizationObject")
                {
                    bounds = transform.gameObject;
                }
            }

            Assert.IsFalse(bounds.activeInHierarchy);
            this.Selection.AddToSelection(this.Ribbon);
            ICommand activateCommand = new ActivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(activateCommand);
            ICommand deactivateCommand = new DeactivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(deactivateCommand);

            Assert.IsFalse(this.Ribbon.transform.IsChildOf(this.Selection.transform));
            Assert.IsFalse(bounds.activeInHierarchy);
            Assert.AreNotEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);

        }

        [Test]
        public void DeactivateSelectionCommandTestUndo()
        {
            GameObject bounds = null;
            foreach (Transform transform in this.Selection.transform)
            {
                if (transform.name == "BoundsVisualizationObject")
                {
                    bounds = transform.gameObject;
                }
            }

            Assert.IsFalse(bounds.activeInHierarchy);
            this.Selection.AddToSelection(this.Ribbon);
            ICommand activateCommand = new ActivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(activateCommand);
            ICommand deactivateCommand = new DeactivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(deactivateCommand);
            Invoker.Undo();

            Assert.IsTrue(this.Ribbon.transform.IsChildOf(this.Selection.transform));
            Assert.IsTrue(bounds.activeInHierarchy);
            Assert.AreEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);

        }

        [Test]
        public void DeactivateSelectionCommandTestRedo()
        {
            GameObject bounds = null;
            foreach (Transform transform in this.Selection.transform)
            {
                if (transform.name == "BoundsVisualizationObject")
                {
                    bounds = transform.gameObject;
                }
            }

            Assert.IsFalse(bounds.activeInHierarchy);
            this.Selection.AddToSelection(this.Ribbon);
            ICommand activateCommand = new ActivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(activateCommand);
            ICommand deactivateCommand = new DeactivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(deactivateCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.IsFalse(this.Ribbon.transform.IsChildOf(this.Selection.transform));
            Assert.IsFalse(bounds.activeInHierarchy);
            Assert.AreNotEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);

        }

        [Test]
        public void DeleteObjectsOfSelectionCommand() {
            GameObject bounds = null;
            foreach (Transform transform in this.Selection.transform)
            {
                if (transform.name == "BoundsVisualizationObject")
                {
                    bounds = transform.gameObject;
                }
            }

            Assert.IsFalse(bounds.activeInHierarchy);
            this.Selection.AddToSelection(this.Ribbon);
            ICommand activateCommand = new ActivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(activateCommand);
            ICommand deleteCommand = new DeleteObjectsOfSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(deleteCommand);

            Assert.IsFalse(this.Ribbon.transform.IsChildOf(this.Selection.transform));
            Assert.IsFalse(bounds.activeInHierarchy);
            Assert.AreNotEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);
            Assert.IsTrue(this.Ribbon.transform.IsChildOf(SketchWorld.ActiveSketchWorld.transform));
        }

        [Test]
        public void DeleteObjectsOfSelectionCommandUndo()
        {
            GameObject bounds = null;
            foreach (Transform transform in this.Selection.transform)
            {
                if (transform.name == "BoundsVisualizationObject")
                {
                    bounds = transform.gameObject;
                }
            }

            Assert.IsFalse(bounds.activeInHierarchy);
            this.Selection.AddToSelection(this.Ribbon);
            ICommand activateCommand = new ActivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(activateCommand);
            ICommand deleteCommand = new DeleteObjectsOfSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(deleteCommand);
            Invoker.Undo();

            Assert.IsTrue(this.Ribbon.transform.IsChildOf(this.Selection.transform));
            Assert.IsTrue(bounds.activeInHierarchy);
            Assert.AreEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);
            Assert.IsFalse(this.Ribbon.transform.IsChildOf(SketchWorld.ActiveSketchWorld.transform));
        }

        [Test]
        public void DeleteObjectsOfSelectionCommandRedo()
        {
            GameObject bounds = null;
            foreach (Transform transform in this.Selection.transform)
            {
                if (transform.name == "BoundsVisualizationObject")
                {
                    bounds = transform.gameObject;
                }
            }

            Assert.IsFalse(bounds.activeInHierarchy);
            this.Selection.AddToSelection(this.Ribbon);
            ICommand activateCommand = new ActivateSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(activateCommand);
            ICommand deleteCommand = new DeleteObjectsOfSelectionCommand(this.Selection);
            Invoker.ExecuteCommand(deleteCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.IsFalse(this.Ribbon.transform.IsChildOf(this.Selection.transform));
            Assert.IsFalse(bounds.activeInHierarchy);
            Assert.AreNotEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);
            Assert.IsTrue(this.Ribbon.transform.IsChildOf(SketchWorld.ActiveSketchWorld.transform));
        }

        [Test]
        public void AddToSelectionCommandTest()
        {
            ICommand addCommand = new AddToSelectionAndHighlightCommand(this.Selection, this.Ribbon);
            Invoker.ExecuteCommand(addCommand);

            Assert.IsTrue(this.Selection.GetObjectsOfSelection().Contains(this.Ribbon));
            Assert.AreEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);
        }

        [Test]
        public void AddToSelectionCommandTestUndo()
        {
            ICommand addCommand = new AddToSelectionAndHighlightCommand(this.Selection, this.Ribbon);
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();

            Assert.IsFalse(this.Selection.GetObjectsOfSelection().Contains(this.Ribbon));
            Assert.AreNotEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);
        }

        [Test]
        public void AddToSelectionCommandTestRedo()
        {
            ICommand addCommand = new AddToSelectionAndHighlightCommand(this.Selection, this.Ribbon);
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.IsTrue(this.Selection.GetObjectsOfSelection().Contains(this.Ribbon));
            Assert.AreEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);
        }

        [Test]
        public void RemoveFromSelectionCommandTest()
        {
            ICommand addCommand = new AddToSelectionAndHighlightCommand(this.Selection, this.Ribbon);
            Invoker.ExecuteCommand(addCommand);
            ICommand removeCommand = new RemoveFromSelectionAndRevertHighlightCommand(this.Selection, this.Ribbon);
            Invoker.ExecuteCommand(removeCommand);

            Assert.IsFalse(this.Selection.GetObjectsOfSelection().Contains(this.Ribbon));
            Assert.AreNotEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);
        }

        [Test]
        public void RemoveFromSelectionCommandTestUndo()
        {
            ICommand addCommand = new AddToSelectionAndHighlightCommand(this.Selection, this.Ribbon);
            Invoker.ExecuteCommand(addCommand);
            ICommand removeCommand = new RemoveFromSelectionAndRevertHighlightCommand(this.Selection, this.Ribbon);
            Invoker.ExecuteCommand(removeCommand);
            Invoker.Undo();

            Assert.IsTrue(this.Selection.GetObjectsOfSelection().Contains(this.Ribbon));
            Assert.AreEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);
        }

        [Test]
        public void RemoveFromSelectionCommandTestRedo()
        {
            ICommand addCommand = new AddToSelectionAndHighlightCommand(this.Selection, this.Ribbon);
            Invoker.ExecuteCommand(addCommand);
            ICommand removeCommand = new RemoveFromSelectionAndRevertHighlightCommand(this.Selection, this.Ribbon);
            Invoker.ExecuteCommand(removeCommand);

            Assert.IsFalse(this.Selection.GetObjectsOfSelection().Contains(this.Ribbon));
            Assert.AreNotEqual("HighlightSelectionMaterial (Instance)", this.Ribbon.GetComponent<MeshRenderer>().material.name);
        }
    }
}
