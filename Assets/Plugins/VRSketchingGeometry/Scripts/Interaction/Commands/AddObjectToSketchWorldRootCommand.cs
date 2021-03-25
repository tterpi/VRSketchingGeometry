using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands
{
    /// <summary>
    /// Add a new object to the sketch world root. The sketch object is deleted when undoing this command. 
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class AddObjectToSketchWorldRootCommand : ICommand
    {
        private SelectableObject NewObject = null;
        private SketchWorld SketchWorld;

        public AddObjectToSketchWorldRootCommand(SelectableObject newObject, SketchWorld sketchWorld)
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
            SketchWorld.RestoreObject(NewObject);
        }

        public void Undo()
        {
            SketchWorld.DeleteObject(NewObject);
        }
    }
}
