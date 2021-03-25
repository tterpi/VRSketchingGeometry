using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Patch {
    /// <summary>
    /// Delete the last segment of the patch.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class DeleteSegmentCommand : ICommand
    {
        private PatchSketchObject PatchSketchObject;
        private List<Vector3> OldSegment;

        /// <summary>
        /// Delete the last segment of the patch sketch object.
        /// </summary>
        /// <param name="patchSketchObject"></param>
        public DeleteSegmentCommand(PatchSketchObject patchSketchObject) {
            this.PatchSketchObject = patchSketchObject;
        }

        public bool Execute()
        {
            OldSegment = this.PatchSketchObject.GetLastSegment();
            this.PatchSketchObject.RemovePatchSegment();
            if (this.PatchSketchObject.GetControlPointsCount() == 0)
            {
                SketchWorld.ActiveSketchWorld.DeleteObject(this.PatchSketchObject);
            }
            return true;
        }

        public void Redo()
        {
            this.Execute();
        }

        public void Undo()
        {
            if (SketchWorld.ActiveSketchWorld.IsObjectDeleted(this.PatchSketchObject))
            {
                SketchWorld.ActiveSketchWorld.RestoreObject(this.PatchSketchObject);
            }
            this.PatchSketchObject.AddPatchSegment(OldSegment);
        }
    }
}
