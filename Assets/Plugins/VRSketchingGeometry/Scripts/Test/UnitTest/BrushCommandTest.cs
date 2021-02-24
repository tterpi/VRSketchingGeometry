using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.Serialization;
using VRSketchingGeometry.Commands.Line;
using VRSketchingGeometry.Commands.Ribbon;
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
            ICommand addCommand = new AddControlPointCommand(this.Line, new Vector3(0,0,0));
            Invoker.ExecuteCommand(addCommand);
            addCommand = new AddControlPointCommand(this.Line, new Vector3(1, 1, 1));
            Invoker.ExecuteCommand(addCommand);
            addCommand = new AddControlPointCommand(this.Line, new Vector3(2, 2, 0));
            Invoker.ExecuteCommand(addCommand);
            Assert.AreEqual((2*20 + 2) * 7, this.Line.GetComponent<MeshFilter>().sharedMesh.vertices.Length);

            LineBrush brush = this.Line.GetBrush() as LineBrush;
            brush.SketchMaterial.AlbedoColor = Color.green;
            brush.CrossSectionVertices.Add(Vector3.one);
            brush.CrossSectionNormals.Add(Vector3.one);
            ICommand SetBrushCommand = new SetBrushCommand(this.Line, brush);
            Invoker.ExecuteCommand(SetBrushCommand);

            Assert.AreEqual(Color.green, this.Line.GetComponent<MeshRenderer>().sharedMaterial.color);
            LineBrush updatedBrush = this.Line.GetBrush() as LineBrush;
            Assert.AreEqual(Color.green, updatedBrush.SketchMaterial.AlbedoColor);
            Assert.AreEqual((2 * 20 + 2) * 8, this.Line.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void SetBrushOnLineSketchObjectUndo()
        {
            ICommand addCommand = new AddControlPointCommand(this.Line, new Vector3(0, 0, 0));
            Invoker.ExecuteCommand(addCommand);
            addCommand = new AddControlPointCommand(this.Line, new Vector3(1, 1, 1));
            Invoker.ExecuteCommand(addCommand);
            addCommand = new AddControlPointCommand(this.Line, new Vector3(2, 2, 0));
            Invoker.ExecuteCommand(addCommand);
            Assert.AreEqual((2 * 20 + 2) * 7, this.Line.GetComponent<MeshFilter>().sharedMesh.vertices.Length);

            LineBrush brush = this.Line.GetBrush() as LineBrush;
            Color originalColor = brush.SketchMaterial.AlbedoColor;
            brush.SketchMaterial.AlbedoColor = Color.green;
            brush.CrossSectionVertices.Add(Vector3.one);
            brush.CrossSectionNormals.Add(Vector3.one);
            ICommand SetBrushCommand = new SetBrushCommand(this.Line, brush);
            Invoker.ExecuteCommand(SetBrushCommand);
            Invoker.Undo();

            Assert.AreEqual(originalColor, this.Line.GetComponent<MeshRenderer>().sharedMaterial.color);
            LineBrush updatedBrush = this.Line.GetBrush() as LineBrush;
            Assert.AreEqual(originalColor, updatedBrush.SketchMaterial.AlbedoColor);
            Assert.AreEqual((2 * 20 + 2) * 7, this.Line.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void SetBrushOnLineSketchObjectRedo()
        {
            ICommand addCommand = new AddControlPointCommand(this.Line, new Vector3(0, 0, 0));
            Invoker.ExecuteCommand(addCommand);
            addCommand = new AddControlPointCommand(this.Line, new Vector3(1, 1, 1));
            Invoker.ExecuteCommand(addCommand);
            addCommand = new AddControlPointCommand(this.Line, new Vector3(2, 2, 0));
            Invoker.ExecuteCommand(addCommand);
            Assert.AreEqual((2 * 20 + 2) * 7, this.Line.GetComponent<MeshFilter>().sharedMesh.vertices.Length);

            LineBrush brush = this.Line.GetBrush() as LineBrush;
            brush.SketchMaterial.AlbedoColor = Color.green;
            brush.CrossSectionVertices.Add(Vector3.one);
            brush.CrossSectionNormals.Add(Vector3.one);
            ICommand SetBrushCommand = new SetBrushCommand(this.Line, brush);
            Invoker.ExecuteCommand(SetBrushCommand);
            Invoker.Undo();
            Invoker.Redo();

            Assert.AreEqual(Color.green, this.Line.GetComponent<MeshRenderer>().sharedMaterial.color);
            LineBrush updatedBrush = this.Line.GetBrush() as LineBrush;
            Assert.AreEqual(Color.green, updatedBrush.SketchMaterial.AlbedoColor);
            Assert.AreEqual((2 * 20 + 2) * 8, this.Line.GetComponent<MeshFilter>().sharedMesh.vertices.Length);
        }

        [Test]
        public void SetBrushOnRibbonSketchObject() {
            ICommand addCommand = new AddPointAndRotationCommand(this.Ribbon, new Vector3(0,0,0), Quaternion.Euler(0,0,45) );
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.Ribbon, new Vector3(1, 1, 1), Quaternion.Euler(0, 0, -45));
            Invoker.ExecuteCommand(addCommand);

            addCommand = new AddPointAndRotationCommand(this.Ribbon, new Vector3(1, 1, 1), Quaternion.Euler(45, 0, 0));
            Invoker.ExecuteCommand(addCommand);

            Assert.AreEqual(3 * 3, this.Ribbon.GetComponent<MeshFilter>().sharedMesh.vertexCount);

            RibbonBrush brush = this.Ribbon.GetBrush() as RibbonBrush;
            brush.SketchMaterial.AlbedoColor = Color.cyan;
            brush.CrossSectionVertices.Add(Vector3.one);
            ICommand SetBrushCommand = new SetBrushCommand(this.Ribbon, brush);
            Invoker.ExecuteCommand(SetBrushCommand);

            Assert.AreEqual(4 * 3, this.Ribbon.GetComponent<MeshFilter>().sharedMesh.vertexCount);
            Assert.AreEqual(Color.cyan, this.Ribbon.GetComponent<MeshRenderer>().sharedMaterial.color);
            RibbonBrush updatedBrush = this.Ribbon.GetBrush() as RibbonBrush;
            Assert.AreEqual(Color.cyan, updatedBrush.SketchMaterial.AlbedoColor);
        }

        [Test]
        public void SetBrushOnPatchObject() {
            Brush brush = this.Patch.GetBrush();
            brush.SketchMaterial.AlbedoColor = Color.magenta;
            Assert.AreNotEqual(Color.magenta, this.Patch.GetComponent<MeshRenderer>().sharedMaterial.color);

            ICommand SetBrushCommand = new SetBrushCommand(this.Patch, brush);
            Invoker.ExecuteCommand(SetBrushCommand);

            Assert.AreEqual(Color.magenta, this.Patch.GetComponent<MeshRenderer>().sharedMaterial.color);
        }
    }
}
