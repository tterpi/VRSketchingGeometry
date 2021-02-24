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
    public class PatchSketchObject : SketchObject, ISerializableComponent, IBrushable
    {
        public int Width = 4;
        public int Height = 0;
        public int ResolutionWidth = 4;
        public int ResolutionHeight = 4;

        public float MinimumDistanceToLastSegment = .2f;

        private List<Vector3> ControlPoints = new List<Vector3>();

        /// <summary>
        /// Generates the patch mesh and assigns it to the MeshFilter of this GameObject.
        /// </summary>
        /// <param name="controlPoints"></param>
        /// <param name="width">Number of control points in x direction</param>
        /// <param name="height">Number of control points in y direction</param>
        private void UpdatePatchMesh(List<Vector3> controlPoints, int width, int height)
        {
            if (height < 3 || width < 3 || controlPoints.Count / width != height) {
                Debug.LogWarning("The amount of control points is invalid! \n There must at least be 3x3 control points. Amount of control ponits must be a multiple of width.");
                SetMesh(null);
                return;
            }

            Mesh patchMesh = PatchMesh.GeneratePatchMesh(controlPoints, width, height, this.ResolutionWidth, this.ResolutionHeight);
            SetMesh(patchMesh);
        }

        private void SetMesh(Mesh mesh) {
            Mesh oldMesh = this.GetComponent<MeshFilter>().sharedMesh;
            Destroy(oldMesh);
            this.GetComponent<MeshFilter>().sharedMesh = mesh;
            this.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        /// <summary>
        /// Regenerates the mesh from the currently set control points.
        /// </summary>
        public void UpdatePatchMesh() {
            UpdatePatchMesh(this.ControlPoints, this.Width, this.Height);
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

        public bool AddPatchSegmentContinuous(List<Vector3> newControlPoints) {
            if (newControlPoints.Count != Width) return false;

            bool distanceExceeded = true;

            List<Vector3> lastSegment = this.GetLastSegment();

            for (int i = 0; i < lastSegment.Count; i++)
            {
                if ((lastSegment[i] - newControlPoints[i]).magnitude < MinimumDistanceToLastSegment)
                {
                    distanceExceeded = false;
                }
            }


            if (distanceExceeded)
            {
                this.AddPatchSegment(newControlPoints);
                return true;
            }
            else
            {
                return false;
            }
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
        public List<Vector3> GetControlPoints() {
            return new List<Vector3>(ControlPoints);
        }

        public int GetControlPointsCount() {
            return this.ControlPoints.Count();
        }

        /// <summary>
        /// Get a segment of control points of length Width.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<Vector3> GetSegment(int index) {
            return ControlPoints.GetRange(index * Width, Width);
        }

        /// <summary>
        /// Get the last segment of control points of length width.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetLastSegment() {
            return GetSegment(ControlPoints.Count/Width -1);
        }

        public Brush GetBrush() {
            Brush brush = new Brush();
            brush.SketchMaterial = new SketchMaterialData(meshRenderer.sharedMaterial);
            return brush;
        }

        public void SetBrush(Brush brush) {
            meshRenderer.sharedMaterial = Defaults.GetMaterialFromDictionary(brush.SketchMaterial);
        }

        public SerializableComponentData GetData()
        {
            PatchSketchObjectData data = new PatchSketchObjectData
            {
                ControlPoints = this.GetControlPoints(),
                Width = this.Width,
                Height = this.Height,
                ResolutionWidth = this.ResolutionWidth,
                ResolutionHeight = this.ResolutionHeight,
                SketchMaterial = new SketchMaterialData(meshRenderer.sharedMaterial),

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

                meshRenderer.material = Defaults.GetMaterialFromDictionary(patchData.SketchMaterial);

                originalMaterial = meshRenderer.sharedMaterial;

                transform.position = patchData.Position;
                transform.rotation = patchData.Rotation;
                transform.localScale = patchData.Scale;
            }
        }
    }
}