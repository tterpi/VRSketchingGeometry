using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class RibbonSketchObject : SketchObject, ISerializableComponent
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

        private List<Vector3> points = new List<Vector3>();
        private List<Quaternion> rotations = new List<Quaternion>();
        public float MinimumControlPointDistance = .02f;

        public List<Vector3> Points { get => new List<Vector3>(points);private set => points = value; }
        public List<Quaternion> Rotations { get => new List<Quaternion>(rotations);private set => rotations = value; }

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

        public void SetControlPoints(List<Vector3> points, List<Quaternion> rotations) {
            Points = points;
            this.Rotations = rotations;
            Mesh mesh = RibbonMesh.GetMesh(points, rotations);
            UpdateMesh(mesh);
        }

        public void AddControlPoint(Vector3 point, Quaternion rotation) {
            Points.Add(point);
            Rotations.Add(rotation);
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

        public void AddControlPoints(List<Vector3> points, List<Quaternion> rotations) {
            Points.AddRange(points);
            this.Rotations.AddRange(rotations);
            Mesh mesh = RibbonMesh.AddPoints(points, rotations);
            UpdateMesh(mesh);
        }

        public void DeleteControlPoint() {
            if (points.Count <= 0) return;
            Mesh mesh = RibbonMesh.DeletePoint();
            UpdateMesh(mesh);
        }

        public void SetRibbonScale(Vector3 scale) {
            RibbonMesh = new RibbonMesh(scale);
            Mesh mesh = RibbonMesh.GetMesh(Points, Rotations);
            UpdateMesh(mesh);
        }

        public SerializableComponentData GetData()
        {
            RibbonSketchObjectData ribbonData = new RibbonSketchObjectData();
            ribbonData.ControlPoints = Points;
            ribbonData.ControlPointOrientations = Rotations;
            ribbonData.CrossSectionScale = RibbonMesh.Scale;
            ribbonData.CrossSectionVertices = RibbonMesh.CrossSection;
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

