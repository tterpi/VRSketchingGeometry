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
    public class BrushCommandTest
    {
        private RibbonSketchObject Ribbon;
        private LineSketchObject Line;
        private PatchSketchObject Patch;
        private CommandInvoker Invoker;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.Ribbon = GameObject.FindObjectOfType<RibbonSketchObject>();
            this.Line = GameObject.FindObjectOfType<LineSketchObject>();
            this.Patch = GameObject.FindObjectOfType<PatchSketchObject>();
            yield return null;
            Invoker = new CommandInvoker();
        }

        [Test]
        public void SetBrushOnLineSketchObject()
        {
            Assert.Fail();
        }
    }
}
