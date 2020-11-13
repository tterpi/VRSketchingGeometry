using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Meshing;

namespace VRSketchingGeometry.SketchObjectManagement
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class PatchSketchObject : SketchObject
    {
        public int width = 4;
        public int height = 0;
        public int resolutionWidth = 4;
        public int resolutionHeight = 4;

        private List<Vector3> controlPoints;

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
            Mesh patchMesh = PatchMesh.GeneratePatchMesh(controlPoints, width, height, this.resolutionWidth, this.resolutionHeight);
            Mesh oldMesh = this.GetComponent<MeshFilter>().sharedMesh;
            Destroy(oldMesh);
            this.GetComponent<MeshFilter>().mesh = patchMesh;
            this.GetComponent<MeshCollider>().sharedMesh = patchMesh;
        }

        /// <summary>
        /// Add a segement to the patch at the end of the patch.
        /// </summary>
        /// <param name="controlPoints"></param>
        public void addPatchSegment(List<Vector3> controlPoints) {
            if (controlPoints.Count != width)
            {
                Debug.LogWarning("Segment has to have width number of control points.");
                return;
            }
            controlPoints.AddRange(controlPoints);
            height++;
            UpdatePatchMesh(controlPoints, width, height);
        }


        /// <summary>
        /// Remove a segement at the end of the patch.
        /// </summary>
        public void removePatchSegment() {
            controlPoints.RemoveRange(controlPoints.Count - width, width);
            height--;
            UpdatePatchMesh(controlPoints, width, height);
        }

        /// <summary>
        /// Returns a copy of the control point array.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> getControlPoints() {
            return new List<Vector3>(controlPoints);
        }
    }
}