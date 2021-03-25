using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Patch {
    /// <summary>
    /// Add a segment at the end of the patch.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class AddSegmentCommand : ICommand
    {
        private PatchSketchObject PatchSketchObject;
        private List<Vector3> NewSegment;

        public AddSegmentCommand(PatchSketchObject patchSketchObject, List<Vector3> newSegment) {
            this.PatchSketchObject = patchSketchObject;
            this.NewSegment = newSegment;
        }

        public bool Execute()
        {
            this.PatchSketchObject.AddPatchSegment(NewSegment);
            return true;
        }

        public void Redo()
        {
            if (SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.PatchSketchObject))
            {
                SketchWorld.ActiveSketchWorld.RestoreObject(this.PatchSketchObject);
            }
            this.Execute();
        }

        public void Undo()
        {
            this.PatchSketchObject.RemovePatchSegment();
            if (this.PatchSketchObject.GetControlPointsCount() == 0) {
                SketchWorld.ActiveSketchWorld.DeleteObject(this.PatchSketchObject);
            }
        }
    }
}
