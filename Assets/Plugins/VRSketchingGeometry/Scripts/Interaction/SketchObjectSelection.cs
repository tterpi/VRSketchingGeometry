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
    public class SketchObjectSelection : MonoBehaviour
    {
        private static SketchObjectSelection activeSketchObjectSelection;

        private List<GameObject> SketchObjectsOfSelection = new List<GameObject>();
        public List<GameObject> GetObjectsOfSelection() => new List<GameObject>(SketchObjectsOfSelection);

        public static SketchObjectSelection ActiveSketchObjectSelection
        {
            get { return activeSketchObjectSelection; }
            set
            {
                activeSketchObjectSelection = value;
            }
        }


        [SerializeField]
        private GameObject boundsVisualizationObject;

        public void AddToSelection(SketchObject sketchObject)
        {
            AddToSelection(sketchObject.gameObject);
        }

        public void AddToSelection(SketchObjectGroup sketchObjectGroup)
        {
            AddToSelection(sketchObjectGroup.gameObject);
        }

        private void AddToSelection(GameObject gameObject)
        {
            SketchObjectsOfSelection.Add(gameObject);
        }

        public void RemoveFromSelection(SketchObject sketchObject)
        {
            sketchObject.revertHighlight();
            sketchObject.resetToParentGroup();
            RemoveFromSelection(sketchObject.gameObject);
        }

        public void RemoveFromSelection(SketchObjectGroup sketchObjectGroup)
        {
            sketchObjectGroup.BroadcastMessage(nameof(SketchObject.revertHighlight));
            sketchObjectGroup.resetToParentGroup();
            RemoveFromSelection(sketchObjectGroup.gameObject);
        }

        private void RemoveFromSelection(GameObject gameObject) {
            SketchObjectsOfSelection.Remove(gameObject);
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
        public void DeleteObjectsOfSelection()
        {
            Deactivate();
            foreach (GameObject selectedObject in SketchObjectsOfSelection) {
                SketchWorld.ActiveSketchWorld.DeleteObject(selectedObject);
            }
        }

        /// <summary>
        /// Highlight all objects added to this selection and set this selection as active selection.
        /// </summary>
        public void Activate()
        {
            if (ActiveSketchObjectSelection != this)
            {
                if (ActiveSketchObjectSelection != null)
                {
                    ActiveSketchObjectSelection.Deactivate();
                }
                ActiveSketchObjectSelection = this;
            }

            foreach (GameObject selected in SketchObjectsOfSelection) {
                selected.transform.SetParent(this.transform);
            }

            SetUpBoundingBoxVisualization(GetBoundsOfSelection(this));
            boundsVisualizationObject.SetActive(true);
            this.gameObject.BroadcastMessage(nameof(IHighlightable.highlight));
        }

        /// <summary>
        /// Reverts the high light of all selected objects and resets the active selection.
        /// </summary>
        public void Deactivate()
        {
            if (ActiveSketchObjectSelection == this)
            {
                ActiveSketchObjectSelection = null;
            }

            gameObject.BroadcastMessage(nameof(IHighlightable.revertHighlight));
            boundsVisualizationObject.SetActive(false);

            foreach (GameObject selected in SketchObjectsOfSelection)
            {
                selected.GetComponent<IGroupable>().resetToParentGroup();
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