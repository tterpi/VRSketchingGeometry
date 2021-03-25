using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Group
{
    /// <summary>
    /// Add an object to a sketch object group.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class AddToGroupCommand : ICommand
    {
        SketchObjectGroup Group;
        IGroupable NewObject;
        SketchObjectGroup OriginalParent;

        public AddToGroupCommand(SketchObjectGroup group, IGroupable NewObject)
        {
            Group = group;
            this.NewObject = NewObject;
            this.OriginalParent = NewObject.ParentGroup;
        }

        public bool Execute()
        {
            Group.AddToGroup(NewObject);
            return true;
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            NewObject.ParentGroup = OriginalParent;
            NewObject.resetToParentGroup();
        }
    }
}