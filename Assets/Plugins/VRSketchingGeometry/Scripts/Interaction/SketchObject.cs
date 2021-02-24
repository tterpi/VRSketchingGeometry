using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.SketchObjectManagement {
    /// <summary>
    /// Base class for diferent kinds of SketchObjects.
    /// </summary>
    public abstract class SketchObject : SelectableObject
    {
        public VRSketchingGeometry.DefaultReferences Defaults;

        private SketchObjectGroup parentGroup;

        public override SketchObjectGroup ParentGroup { get => parentGroup; set => parentGroup = value; }

        protected MeshRenderer meshRenderer;

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
    }
}

