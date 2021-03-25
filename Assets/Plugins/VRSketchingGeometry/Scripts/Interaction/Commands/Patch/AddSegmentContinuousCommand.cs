using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Patch {
    /// <summary>
    /// Add a segment at the end of the patch if it is at least a minimum distance away form the last segment.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class AddSegmentContinuousCommand : ICommand
    {
        private PatchSketchObject PatchSketchObject;
        private List<Vector3> NewSegment;

        public AddSegmentContinuousCommand(PatchSketchObject patchSketchObject, List<Vector3> newSegment) {
            this.PatchSketchObject = patchSketchObject;
            this.NewSegment = newSegment;
        }

        public bool Execute()
        {
            return this.PatchSketchObject.AddPatchSegmentContinuous(NewSegment);
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
            if (this.PatchSketchObject.GetControlPointsCount() == 0)
            {
                SketchWorld.ActiveSketchWorld.DeleteObject(this.PatchSketchObject);
            }
        }
    }
}
