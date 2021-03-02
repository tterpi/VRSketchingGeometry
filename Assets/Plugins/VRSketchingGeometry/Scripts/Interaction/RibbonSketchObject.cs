using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class RibbonSketchObject : SketchObject, ISerializableComponent, IBrushable
    {
        /// <summary>
        /// Mesh filter for the mesh of the ribbon
        /// </summary>
        protected MeshFilter meshFilter;

        /// <summary>
        /// Collider for the mesh of the ribbon
        /// </summary>
        protected MeshCollider meshCollider;

        private RibbonMesh RibbonMesh;

        private List<Vector3> Points = new List<Vector3>();
        private List<Quaternion> Rotations = new List<Quaternion>();
        public float MinimumControlPointDistance = .02f;

        protected override void Awake()
        {
            base.Awake();
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

            RibbonMesh = new RibbonMesh(Vector3.one * .3f);
        }

        private void UpdateMesh(Mesh mesh)
        {
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public void SetControlPoints(List<Vector3> newPoints, List<Quaternion> newRotations) {
            if (newPoints.Count != newRotations.Count) {
                Debug.LogError("RibbonSketchObject: Count of points and rotations is not equal.");
                return;
            }
            this.Points = newPoints.Select(ct => this.transform.InverseTransformPoint(ct)).ToList();
            this.Rotations = newRotations.Select(rotation => Quaternion.Inverse(this.transform.rotation) * rotation).ToList(); ;
            Mesh mesh = RibbonMesh.GetMesh(this.Points, this.Rotations);
            UpdateMesh(mesh);
        }

        public void AddControlPoint(Vector3 point, Quaternion rotation) {
            Points.Add(this.transform.InverseTransformPoint(point));
            Rotations.Add(Quaternion.Inverse(this.transform.rotation)*rotation);
            Mesh mesh = RibbonMesh.AddPoint(point, rotation);
            UpdateMesh(mesh);
        }

        /// <summary>
        /// Adds a control point to the spline if it is far enough away from the previous control point.
        /// The distance is controlled by minimumControlPointDistance.
        /// </summary>
        /// <param name="point"></param>
        public bool AddControlPointContinuous(Vector3 point, Quaternion rotation)
        {
            //Check that new control point is far enough away from previous control point
            if (
                Points.Count == 0 ||
                (transform.InverseTransformPoint(point) - Points[Points.Count -1]).magnitude > MinimumControlPointDistance
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

        public void AddControlPoints(List<Vector3> newPoints, List<Quaternion> newRotations) {
            this.Points.AddRange(newPoints);
            this.Rotations.AddRange(newRotations);
            Mesh mesh = RibbonMesh.AddPoints(this.Points, this.Rotations);
            UpdateMesh(mesh);
        }

        public void DeleteControlPoint() {
            Points.RemoveAt(Points.Count - 1);
            Rotations.RemoveAt(Rotations.Count - 1);
            if (Points.Count < 0) return;
            Mesh mesh = RibbonMesh.DeletePoint();
            UpdateMesh(mesh);
        }

        public void SetRibbonScale(Vector3 scale) {
            RibbonMesh = new RibbonMesh(scale);
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

        public List<Vector3> GetPoints() => new List<Vector3>(Points);
        public int GetPointsCount() => this.Points.Count;
        public List<Quaternion> GetRotations() => new List<Quaternion>(Rotations);

        public Brush GetBrush() {
            RibbonBrush brush = new RibbonBrush();
            brush.SketchMaterial = new SketchMaterialData(meshRenderer.sharedMaterial);
            brush.CrossSectionScale = RibbonMesh.Scale;
            brush.CrossSectionVertices = RibbonMesh.GetCrossSection();
            return brush;
        }

        public SerializableComponentData GetData()
        {
            RibbonSketchObjectData ribbonData = new RibbonSketchObjectData();
            ribbonData.ControlPoints = GetPoints();
            ribbonData.ControlPointOrientations = GetRotations();
            ribbonData.CrossSectionScale = RibbonMesh.Scale;
            ribbonData.CrossSectionVertices = RibbonMesh.GetCrossSection();
            ribbonData.Position = this.transform.position;
            ribbonData.Rotation = this.transform.rotation;
            ribbonData.Scale = this.transform.localScale;
            ribbonData.SketchMaterial = new SketchMaterialData(meshRenderer.sharedMaterial);
            return ribbonData;
        }

        public void ApplyData(SerializableComponentData data)
        {
            if (data is RibbonSketchObjectData ribbonData)
            {
                RibbonMesh = new RibbonMesh(ribbonData.CrossSectionVertices, ribbonData.CrossSectionScale);
                SetControlPoints(ribbonData.ControlPoints, ribbonData.ControlPointOrientations);
                meshRenderer.sharedMaterial = Defaults.GetMaterialFromDictionary(ribbonData.SketchMaterial);
                transform.position = ribbonData.Position;
                transform.rotation = ribbonData.Rotation;
                transform.localScale = ribbonData.Scale;
            }
            else {
                Debug.LogError("Invalid data for RibbonSketchObject.");
            }
        }
    }
}

