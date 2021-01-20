using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRSketchingGeometry;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class PatchSketchObject : SketchObject, ISerializableComponent
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
        private void UpdatePatchMesh(List<Vector3> controlPoints, int width, int height)
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
        /// Regenerates the mesh from the currently set control points.
        /// </summary>
        public void UpdatePatchMesh() {

            if (Height >= 3 && Width >= 3)
            {
                UpdatePatchMesh(this.ControlPoints, this.Width, this.Height);
            }
            else
            {
                Debug.LogError("Width and Height have to be at least 3.");
            }
        }

        /// <summary>
        /// Sets the control points.
        /// Control points have to be at least 3x3. 
        /// Control points are expected to be in world space.
        /// </summary>
        /// <param name="controlPoints"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetControlPoints(List<Vector3> controlPoints, int width, int height) {
            this.ControlPoints = controlPoints.Select(ct => this.transform.InverseTransformPoint(ct)).ToList();
            this.Width = width;
            this.Height = height;

            UpdatePatchMesh();
        }

        /// <summary>
        /// Add a segement to the patch at the end of the patch.
        /// Control points are expected to be in world space.
        /// </summary>
        /// <param name="newControlPoints"></param>
        public void AddPatchSegment(List<Vector3> newControlPoints) {
            if (newControlPoints.Count != Width)
            {
                Debug.LogWarning("Segment has to have width number of control points.");
                return;
            }
            this.ControlPoints.AddRange(newControlPoints.Select( ct => this.transform.InverseTransformPoint(ct)));
            Height++;
            UpdatePatchMesh();
        }


        /// <summary>
        /// Remove a segement at the end of the patch.
        /// </summary>
        public void RemovePatchSegment() {
            ControlPoints.RemoveRange(ControlPoints.Count - Width, Width);
            Height--;
            UpdatePatchMesh();
        }

        /// <summary>
        /// Returns a copy of the control points in local space.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> getControlPoints() {
            return new List<Vector3>(ControlPoints);
        }

        public SerializableComponentData GetData()
        {
            PatchSketchObjectData data = new PatchSketchObjectData
            {
                ControlPoints = this.getControlPoints(),
                Width = this.Width,
                Height = this.Height,
                ResolutionWidth = this.ResolutionWidth,
                ResolutionHeight = this.ResolutionHeight,
                Material = new SketchMaterialData(meshRenderer.material),

                Position = this.transform.position,
                Rotation = this.transform.rotation,
                Scale = this.transform.localScale
            };

            return data;
        }

        public void ApplyData(SerializableComponentData data)
        {
            if (data is PatchSketchObjectData patchData) {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                transform.localScale = Vector3.one;

                ResolutionWidth = patchData.ResolutionWidth;
                ResolutionHeight = patchData.ResolutionHeight;

                SetControlPoints(patchData.ControlPoints, patchData.Width, patchData.Height);

                meshRenderer.material = Defaults.GetMaterial(patchData.Material.Shader);
                patchData.Material.ApplyMaterialProperties(meshRenderer.material, Defaults.DefaultTextureDirectory);
                originalMaterial = meshRenderer.sharedMaterial;

                transform.position = patchData.Position;
                transform.rotation = patchData.Rotation;
                transform.localScale = patchData.Scale;
            }
        }
    }
}