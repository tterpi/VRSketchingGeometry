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

        private List<Vector3> Points;
        private List<Quaternion> Rotations;

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
            Rotations = rotations;
            Mesh mesh = RibbonMesh.GetMesh(points, rotations);
            UpdateMesh(mesh);
        }

        public void AddControlPoint(Vector3 point, Quaternion rotation) {
            Points.Add(point);
            Rotations.Add(rotation);
            Mesh mesh = RibbonMesh.AddPoint(point, rotation);
            UpdateMesh(mesh);
        }

        public void AddControlPoints(List<Vector3> points, List<Quaternion> rotations) {
            Points.AddRange(points);
            Rotations.AddRange(rotations);
            Mesh mesh = RibbonMesh.AddPoints(points, rotations);
            UpdateMesh(mesh);
        }

        public void DeleteControlPoint() {
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

