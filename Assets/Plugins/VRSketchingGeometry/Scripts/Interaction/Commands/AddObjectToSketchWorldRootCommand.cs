using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands
{
    /// <summary>
    /// Add a new object to the sketch world root. The sketch object is deleted when undoing this command. 
    /// </summary>
    public class AddObjectToSketchWorldRootCommand : ICommand
    {
        private IGroupable NewObject = null;
        private SketchWorld SketchWorld;

        public AddObjectToSketchWorldRootCommand(IGroupable newObject, SketchWorld sketchWorld)
        {
            this.NewObject = newObject;
            this.SketchWorld = sketchWorld;
        }

        public bool Execute()
        {
            SketchWorld.AddObject(this.NewObject);
            return true;
        }

        public void Redo()
        {
            if (NewObject is MonoBehaviour component) {
                SketchWorld.RestoreObject(component.gameObject);
            }
        }

        public void Undo()
        {
            if (NewObject is MonoBehaviour component) {
                SketchWorld.DeleteObject(component.gameObject);
            }
        }
    }
}
