using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.Commands.Group;
using VRSketchingGeometry.SketchObjectManagement;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class SketchObjectGroupCommandTest
    {
        private RibbonSketchObject Ribbon;
        private SketchObjectGroup Group;
        private CommandInvoker Invoker;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.Ribbon = GameObject.FindObjectOfType<RibbonSketchObject>();
            this.Group = GameObject.FindObjectOfType<SketchObjectGroup>();
            yield return null;
            Invoker = new CommandInvoker();
        }

        // A Test behaves as an ordinary method
        [Test]
        public void AddToGroupCommand()
        {
            Assert.AreNotEqual(Group.transform, Ribbon.transform.parent);
            ICommand addCommand = new AddToGroupCommand(Group, Ribbon);
            Invoker.ExecuteCommand(addCommand);
            Assert.AreEqual(Group.gameObject, Ribbon.ParentGroup);
            Assert.AreEqual(Group.transform, Ribbon.transform.parent);
        }

        [Test]
        public void AddToGroupCommandUndo()
        {
            Assert.AreNotEqual(Group.transform, Ribbon.transform.parent);
            ICommand addCommand = new AddToGroupCommand(Group, Ribbon);
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();
            Assert.AreEqual(null, Ribbon.ParentGroup);
            Assert.AreEqual(null, Ribbon.transform.parent);
        }

        [Test]
        public void AddToGroupCommandRedo()
        {
            Assert.AreNotEqual(Group.transform, Ribbon.transform.parent);
            ICommand addCommand = new AddToGroupCommand(Group, Ribbon);
            Invoker.ExecuteCommand(addCommand);
            Invoker.Undo();
            Invoker.Redo();
            Assert.AreEqual(Group.gameObject, Ribbon.ParentGroup);
            Assert.AreEqual(Group.transform, Ribbon.transform.parent);
        }

        [Test]
        public void RemoveFromGroupCommand()
        {
            SketchWorld.ActiveSketchWorld = null;
            Assert.AreNotEqual(Group.transform, Ribbon.transform.parent);
            ICommand addCommand = new AddToGroupCommand(Group, Ribbon);
            Invoker.ExecuteCommand(addCommand);

            ICommand removeCommand = new RemoveFromGroupCommand(Ribbon);
            Invoker.ExecuteCommand(removeCommand);

            Assert.AreEqual(null, Ribbon.ParentGroup);
            Assert.AreEqual(null, Ribbon.transform.parent);
        }

        [Test]
        public void RemoveFromGroupCommand_WithActiveSketchWorld()
        {
            Assert.AreNotEqual(Group.transform, Ribbon.transform.parent);
            ICommand addCommand = new AddToGroupCommand(Group, Ribbon);
            Invoker.ExecuteCommand(addCommand);

            ICommand removeCommand = new RemoveFromGroupCommand(Ribbon);
            Invoker.ExecuteCommand(removeCommand);

            Assert.IsTrue( Ribbon.ParentGroup.transform.IsChildOf(SketchWorld.ActiveSketchWorld.transform));
        }

        [Test]
        public void RemoveFromGroupCommandUndo()
        {
            SketchWorld.ActiveSketchWorld = null;
            Assert.AreNotEqual(Group.transform, Ribbon.transform.parent);
            ICommand addCommand = new AddToGroupCommand(Group, Ribbon);
            Invoker.ExecuteCommand(addCommand);

            ICommand removeCommand = new RemoveFromGroupCommand(Ribbon);
            Invoker.ExecuteCommand(removeCommand);
            Invoker.Undo();

            Assert.AreEqual(Group.gameObject, Ribbon.ParentGroup);
            Assert.AreEqual(Group.transform, Ribbon.transform.parent);
        }

        [Test]
        public void RemoveFromGroupCommandRedo()
        {
            SketchWorld.ActiveSketchWorld = null;
            Assert.AreNotEqual(Group.transform, Ribbon.transform.parent);
            ICommand addCommand = new AddToGroupCommand(Group, Ribbon);
            Invoker.ExecuteCommand(addCommand);

            ICommand removeCommand = new RemoveFromGroupCommand(Ribbon);
            Invoker.ExecuteCommand(removeCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.AreEqual(null, Ribbon.ParentGroup);
            Assert.AreEqual(null, Ribbon.transform.parent);
        }
    }
}
