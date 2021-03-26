using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.SketchObjectManagement
{
    /// <summary>
    /// Represents a selection of SketchObjects and SketchObjectGroups.
    /// When the selection is activated the game objects are parented to the selection game object and highlighted.
    /// When deactivating the object return to their original position in the hierarchy.
    /// There can only be one active selection at a time.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class SketchObjectSelection : MonoBehaviour
    {
        private static SketchObjectSelection activeSketchObjectSelection;

        private List<SelectableObject> SketchObjectsOfSelection = new List<SelectableObject>();
        public List<SelectableObject> GetObjectsOfSelection() => new List<SelectableObject>(SketchObjectsOfSelection);

        public static SketchObjectSelection ActiveSketchObjectSelection
        {
            get { return activeSketchObjectSelection; }
            set
            {
                activeSketchObjectSelection = value;
            }
        }

        [SerializeField]
#pragma warning disable CS0649
        private GameObject boundsVisualizationObject;
#pragma warning restore CS0649

        internal void AddToSelection(SelectableObject selectableObject) {
            SketchObjectsOfSelection.Add(selectableObject);
        }

        internal void RemoveFromSelection(SelectableObject selectableObject) {
            selectableObject.revertHighlight();
            selectableObject.resetToParentGroup();
            SketchObjectsOfSelection.Remove(selectableObject);
        }

        /// <summary>
        /// Deactivate selection and clear the added objects.
        /// </summary>
        public void RemoveAllFromSelection() {
            Deactivate();
            SketchObjectsOfSelection.Clear();
        }

        /// <summary>
        /// Deactivate this selection and delete all selected objects via the active sketch world.
        /// Selection doesn't have to be active.
        /// </summary>
        internal void DeleteObjectsOfSelection()
        {
            Deactivate();
            foreach (SelectableObject selectedObject in SketchObjectsOfSelection) {
                SketchWorld.ActiveSketchWorld.DeleteObject(selectedObject);
            }
        }

        /// <summary>
        /// Highlight all objects added to this selection and set this selection as active selection.
        /// </summary>
        internal void Activate()
        {
            if (ActiveSketchObjectSelection != this)
            {
                if (ActiveSketchObjectSelection != null)
                {
                    ActiveSketchObjectSelection.Deactivate();
                }
                ActiveSketchObjectSelection = this;
            }

            foreach (SelectableObject selected in SketchObjectsOfSelection) {
                selected.transform.SetParent(this.transform);
            }

            SetUpBoundingBoxVisualization(GetBoundsOfSelection(this));
            boundsVisualizationObject.SetActive(true);
            this.gameObject.BroadcastMessage(nameof(IHighlightable.highlight));
        }

        /// <summary>
        /// Reverts the high light of all selected objects and resets the active selection.
        /// </summary>
        internal void Deactivate()
        {
            if (ActiveSketchObjectSelection == this)
            {
                ActiveSketchObjectSelection = null;
            }

            gameObject.BroadcastMessage(nameof(IHighlightable.revertHighlight));
            boundsVisualizationObject.SetActive(false);

            foreach (SelectableObject selected in SketchObjectsOfSelection)
            {
                selected.resetToParentGroup();
            }

        }

        private void SetUpBoundingBoxVisualization(Bounds bounds) {
            boundsVisualizationObject.transform.SetParent(null);

            boundsVisualizationObject.transform.position = bounds.center;
            boundsVisualizationObject.transform.localScale = bounds.size;

            boundsVisualizationObject.transform.SetParent(this.transform, true);
        }

        private static Bounds GetBoundsOfSelection(SketchObjectSelection selection) {
            Bounds selectionBounds = new Bounds();
            MeshRenderer[] renderers = selection.gameObject.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer renderer in renderers) {
                selectionBounds.Encapsulate(renderer.bounds);
            }

            return selectionBounds;
        }

    }
}