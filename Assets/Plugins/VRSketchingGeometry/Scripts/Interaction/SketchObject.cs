using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.SketchObjectManagement {
    /// <summary>
    /// Base class for different kinds of SketchObjects.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public abstract class SketchObject : SelectableObject
    {
        public VRSketchingGeometry.DefaultReferences Defaults;

        private SketchObjectGroup parentGroup;

        public override SketchObjectGroup ParentGroup { get => parentGroup; set => parentGroup = value; }

        protected MeshRenderer meshRenderer;
        protected MeshFilter meshFilter;
        protected MeshCollider meshCollider;

        protected Material originalMaterial;

        [SerializeField]
        protected Material highlightMaterial;

        protected virtual void Awake() {
            setUpOriginalMaterialAndMeshRenderer();
        }

        public override void resetToParentGroup()
        {
            this.transform.SetParent(ParentGroup?.transform);
        }

        protected void setUpOriginalMaterialAndMeshRenderer()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
            meshRenderer = GetComponent<MeshRenderer>();
            originalMaterial = meshRenderer.sharedMaterial;
        }

        public override void highlight()
        {
            meshRenderer.sharedMaterial = highlightMaterial;
        }

        public override void revertHighlight()
        {
            meshRenderer.sharedMaterial = originalMaterial;
        }

        /// <summary>
        /// Replace both the renderer and collider mesh with a new one.
        /// The old mesh is manually destroyed to prevent a memory leak.
        /// </summary>
        /// <param name="newMesh">The new mesh to replace the old one.</param>
        protected virtual void UpdateSceneMesh(Mesh newMesh) {
            Mesh oldMesh = meshFilter.sharedMesh;
            meshFilter.sharedMesh = newMesh;
            meshCollider.sharedMesh = newMesh;
            if (Application.isEditor && !Application.isPlaying)
            {
                DestroyImmediate(oldMesh);
            }
            else{
                Destroy(oldMesh);
            }
        }
    }
}

