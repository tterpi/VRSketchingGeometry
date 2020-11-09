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

        private List<GameObject> sketchObjectsOfSelection = new List<GameObject>();

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

        public void addToSelection(SketchObject sketchObject)
        {
            addToSelection(sketchObject.gameObject);
        }

        public void addToSelection(SketchObjectGroup sketchObjectGroup)
        {
            addToSelection(sketchObjectGroup.gameObject);
        }

        private void addToSelection(GameObject gameObject)
        {
            //gameObject.transform.SetParent(this.transform);
            sketchObjectsOfSelection.Add(gameObject);
        }

        public void removeFromSelection(SketchObject sketchObject)
        {
            //removeFromSelection(sketchObject.gameObject);
            sketchObject.revertHighlight();
            sketchObject.transform.SetParent(sketchObject.ParentGroup.transform);
            removeFromSelection(sketchObject.gameObject);
        }

        public void removeFromSelection(SketchObjectGroup sketchObjectGroup)
        {
            //removeFromSelection(sketchObjectGroup.gameObject);
            sketchObjectGroup.BroadcastMessage(nameof(SketchObject.revertHighlight));
            sketchObjectGroup.transform.SetParent(sketchObjectGroup.ParentGroup.transform);
            removeFromSelection(sketchObjectGroup.gameObject);
        }

        private void removeFromSelection(GameObject gameObject) {
            sketchObjectsOfSelection.Remove(gameObject);
        }

        public void removeAllFromSelection() {
            deactivate();
            sketchObjectsOfSelection.Clear();
        }

        public void deleteObjectsOfSelection()
        {
            //deactivate game objects
            //parent to bin object
            foreach (Transform child in this.transform) {
                SketchWorld.ActiveSketchWorld.deleteObject(child.gameObject);
            }
        }

        public void activate()
        {
            if (ActiveSketchObjectSelection != this)
            {
                if (ActiveSketchObjectSelection != null)
                {
                    ActiveSketchObjectSelection.deactivate();
                }
                ActiveSketchObjectSelection = this;
            }

            foreach (GameObject selected in sketchObjectsOfSelection) {
                selected.transform.SetParent(this.transform);
            }

            setUpBoundingBoxVisualization(getBoundsOfSelection(this));
            this.gameObject.BroadcastMessage(nameof(IHighlightable.highlight));
        }

        public void deactivate()
        {
            if (ActiveSketchObjectSelection == this)
            {
                ActiveSketchObjectSelection = null;
            }

            gameObject.BroadcastMessage(nameof(IHighlightable.revertHighlight));
            boundsVisualizationObject.SetActive(false);

            foreach (GameObject selected in sketchObjectsOfSelection)
            {
                selected.GetComponent<IGroupable>().resetToParentGroup();
            }

        }

        private void setUpBoundingBoxVisualization(Bounds bounds) {
            //boundsVisualizationObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boundsVisualizationObject.transform.SetParent(null);

            boundsVisualizationObject.transform.position = bounds.center;
            boundsVisualizationObject.transform.localScale = bounds.size;

            boundsVisualizationObject.transform.SetParent(this.transform, true);
        }

        private static Bounds getBoundsOfSelection(SketchObjectSelection selection) {
            Bounds selectionBounds = new Bounds();
            MeshRenderer[] renderers = selection.gameObject.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer renderer in renderers) {
                selectionBounds.Encapsulate(renderer.bounds);
            }

            return selectionBounds;
        }

    }
}