using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SketchObjectManagement {
    public abstract class SketchObject : MonoBehaviour, IGroupable
    {
        private GameObject parentGroup;

        public GameObject ParentGroup { get => parentGroup; set => parentGroup = value; }

        protected MeshRenderer meshRenderer;

        protected Material originalMaterial;

        [SerializeField]
        protected Material highlightMaterial;

        public void resetToParentGroup()
        {
            this.transform.SetParent(ParentGroup?.transform);
        }

        protected void setUpOriginalMaterialAndMeshRenderer()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            originalMaterial = meshRenderer.sharedMaterial;
        }

        public virtual void highlight()
        {
            meshRenderer.sharedMaterial = highlightMaterial;
        }

        public virtual void revertHighlight()
        {
            meshRenderer.sharedMaterial = originalMaterial;
        }
    }
}

