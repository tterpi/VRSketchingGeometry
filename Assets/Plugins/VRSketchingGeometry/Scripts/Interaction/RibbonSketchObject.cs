using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement{
    /// <summary>
    /// Ribbon sketch object. Creates a flat ribbon shaped geometry along control points and rotations.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class RibbonSketchObject : SketchObject, ISerializableComponent, IBrushable
    {
        private RibbonMesh RibbonMesh;

        private List<Vector3> Points = new List<Vector3>();
        private List<Quaternion> Rotations = new List<Quaternion>();
        public float MinimumControlPointDistance = .02f;

        protected override void Awake()
        {
            base.Awake();

            RibbonMesh = new RibbonMesh(Vector3.one * .3f);
        }

        private void UpdateMesh(Mesh mesh)
        {
            UpdateSceneMesh(mesh);
        }

        /// <summary>
        /// Set all control points. Points and rotations should be in world space.
        /// </summary>
        /// <param name="newPoints"></param>
        /// <param name="newRotations"></param>
        public void SetControlPoints(List<Vector3> newPoints, List<Quaternion> newRotations) {
            List<Vector3> transformedPoints = newPoints.Select(ct => this.transform.InverseTransformPoint(ct)).ToList();
            List<Quaternion> transformedRotations = newRotations.Select(rotation => Quaternion.Inverse(this.transform.rotation) * rotation).ToList();
            SetControlPointsLocalSpace(transformedPoints, transformedRotations);
        }


        /// <summary>
        /// Set all control points. Points and rotations should be in local space.
        /// </summary>
        /// <param name="newPoints"></param>
        /// <param name="newRotations"></param>
        public void SetControlPointsLocalSpace(List<Vector3> newPoints, List<Quaternion> newRotations) {
            if (newPoints.Count != newRotations.Count)
            {
                Debug.LogError("RibbonSketchObject: Count of points and rotations is not equal.");
                return;
            }
            this.Points = new List<Vector3>(newPoints);
            this.Rotations = new List<Quaternion>(newRotations);
            Mesh mesh = RibbonMesh.GetMesh(this.Points, this.Rotations);
            UpdateMesh(mesh);
        }

        /// <summary>
        /// Add a single control point. Point and rotations should be in world space.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rotation"></param>
        internal void AddControlPoint(Vector3 point, Quaternion rotation) {
            Vector3 transformedPoint = this.transform.InverseTransformPoint(point);
            Quaternion transformedRotation = Quaternion.Inverse(this.transform.rotation) * rotation;
            Points.Add(transformedPoint);
            Rotations.Add(transformedRotation);
            Mesh mesh = RibbonMesh.AddPoint(transformedPoint, transformedRotation);
            UpdateMesh(mesh);
        }

        /// <summary>
        /// Adds a control point to the spline if it is far enough away from the previous control point.
        /// The distance is controlled by minimumControlPointDistance.
        /// </summary>
        /// <param name="point"></param>
        internal bool AddControlPointContinuous(Vector3 point, Quaternion rotation)
        {
            //Check that new control point is far enough away from previous control point
            if (
                Points.Count == 0 ||
                (this.transform.InverseTransformPoint(point) - Points[Points.Count -1]).magnitude > MinimumControlPointDistance
               )
            {
                AddControlPoint(point, rotation);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add multiple control points. Should be in world space.
        /// </summary>
        /// <param name="newPoints"></param>
        /// <param name="newRotations"></param>
        public void AddControlPoints(List<Vector3> newPoints, List<Quaternion> newRotations) {
            this.Points.AddRange(newPoints.Select(ct => this.transform.InverseTransformPoint(ct)));
            this.Rotations.AddRange(newRotations.Select(rotation => Quaternion.Inverse(this.transform.rotation) * rotation));
            Mesh mesh = RibbonMesh.AddPoints(this.Points, this.Rotations);
            UpdateMesh(mesh);
        }

        /// <summary>
        /// Delete the last control point.
        /// </summary>
        internal void DeleteControlPoint() {
            Points.RemoveAt(Points.Count - 1);
            Rotations.RemoveAt(Rotations.Count - 1);
            if (Points.Count < 0) return;
            Mesh mesh = RibbonMesh.DeletePoint();
            UpdateMesh(mesh);
        }

        /// <summary>
        /// Set the scale of the ribbon cross section.
        /// </summary>
        /// <param name="scale"></param>
        public void SetRibbonScale(Vector3 scale) {
            List<Vector3> currentCrossSections = this.GetCrossSection();
            RibbonMesh = new RibbonMesh(currentCrossSections, scale);
            Mesh mesh = RibbonMesh.GetMesh(Points, Rotations);
            UpdateMesh(mesh);
        }

        public void SetCrossSection(List<Vector3> CrossSectionVertices, Vector3 CrossSectionScale) {
            RibbonMesh = new RibbonMesh(CrossSectionVertices, CrossSectionScale);
            UpdateMesh(RibbonMesh.GetMesh(Points, Rotations));
        }

        public List<Vector3> GetCrossSection() {
            return this.RibbonMesh.GetCrossSection();
        }

        public void SetBrush(Brush brush) {
            meshRenderer.sharedMaterial = Defaults.GetMaterialFromDictionary(brush.SketchMaterial);
            if (brush is RibbonBrush ribbonBrush)
            {
                SetCrossSection(ribbonBrush.CrossSectionVertices, ribbonBrush.CrossSectionScale);
            }
        }

        /// <summary>
        /// Get the control points in local space.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetPoints() => new List<Vector3>(Points);
        public int GetPointsCount() => this.Points.Count;

        /// <summary>
        /// Get the rotations in local space.
        /// </summary>
        /// <returns></returns>
        public List<Quaternion> GetRotations() => new List<Quaternion>(Rotations);

        public Brush GetBrush() {
            RibbonBrush brush = new RibbonBrush();
            brush.SketchMaterial = new SketchMaterialData(meshRenderer.sharedMaterial);
            brush.CrossSectionScale = RibbonMesh.Scale;
            brush.CrossSectionVertices = RibbonMesh.GetCrossSection();
            return brush;
        }

        SerializableComponentData ISerializableComponent.GetData()
        {
            RibbonSketchObjectData ribbonData = new RibbonSketchObjectData();
            ribbonData.ControlPoints = GetPoints();
            ribbonData.ControlPointOrientations = GetRotations();
            ribbonData.CrossSectionScale = RibbonMesh.Scale;
            ribbonData.CrossSectionVertices = RibbonMesh.GetCrossSection();
            ribbonData.SetDataFromTransform(this.transform);
            ribbonData.SketchMaterial = new SketchMaterialData(meshRenderer.sharedMaterial);
            return ribbonData;
        }

        void ISerializableComponent.ApplyData(SerializableComponentData data)
        {
            if (data is RibbonSketchObjectData ribbonData)
            {
                RibbonMesh = new RibbonMesh(ribbonData.CrossSectionVertices, ribbonData.CrossSectionScale);
                SetControlPointsLocalSpace(ribbonData.ControlPoints, ribbonData.ControlPointOrientations);
                meshRenderer.sharedMaterial = Defaults.GetMaterialFromDictionary(ribbonData.SketchMaterial);
                ribbonData.ApplyDataToTransform(this.transform);
            }
            else {
                Debug.LogError("Invalid data for RibbonSketchObject.");
            }
        }
    }
}

