using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Meshing;

namespace VRSketchingGeometry.SketchObjectManagement
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class PatchSketchObject : SketchObject
    {
        public int Width = 4;
        public int Height = 0;
        public int ResolutionWidth = 4;
        public int ResolutionHeight = 4;

        private List<Vector3> ControlPoints = new List<Vector3>();

        /// <summary>
        /// Generates the patch mesh and assigns it to the MeshFilter of this GameObject.
        /// </summary>
        /// <param name="controlPoints"></param>
        /// <param name="width">Number of control points in x direction</param>
        /// <param name="height">Number of control points in y direction</param>
        public void UpdatePatchMesh(List<Vector3> controlPoints, int width, int height)
        {
            if (controlPoints.Count % width != 0) {
                Debug.LogWarning("The amount of control points is invalid. It's not a multiple of width.");
                return;
            }
            Mesh patchMesh = PatchMesh.GeneratePatchMesh(controlPoints, width, height, this.ResolutionWidth, this.ResolutionHeight);
            Mesh oldMesh = this.GetComponent<MeshFilter>().sharedMesh;
            Destroy(oldMesh);
            this.GetComponent<MeshFilter>().mesh = patchMesh;
            this.GetComponent<MeshCollider>().sharedMesh = patchMesh;
        }

        /// <summary>
        /// Add a segement to the patch at the end of the patch.
        /// </summary>
        /// <param name="newControlPoints"></param>
        public void AddPatchSegment(List<Vector3> newControlPoints) {
            if (newControlPoints.Count != Width)
            {
                Debug.LogWarning("Segment has to have width number of control points.");
                return;
            }
            this.ControlPoints.AddRange(newControlPoints);
            Height++;
            if (Height >= 3 && Width >= 3)
            {
                UpdatePatchMesh(this.ControlPoints, Width, Height);
            }
            else {
                Debug.LogError("Width and Height have to be at least 3.");
            }
        }


        /// <summary>
        /// Remove a segement at the end of the patch.
        /// </summary>
        public void RemovePatchSegment() {
            ControlPoints.RemoveRange(ControlPoints.Count - Width, Width);
            Height--;
            UpdatePatchMesh(ControlPoints, Width, Height);
        }

        /// <summary>
        /// Returns a copy of the control point array.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> getControlPoints() {
            return new List<Vector3>(ControlPoints);
        }
    }
}