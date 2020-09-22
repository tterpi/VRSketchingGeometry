using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SketchObjectManagement;

namespace SketchObjectManagement
{
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

            this.gameObject.BroadcastMessage(nameof(SketchObject.highlight));
        }

        public void deactivate()
        {
            if (ActiveSketchObjectSelection == this)
            {
                ActiveSketchObjectSelection = null;
            }

            gameObject.BroadcastMessage(nameof(SketchObject.revertHighlight));

            foreach (GameObject selected in sketchObjectsOfSelection)
            {
                selected.GetComponent<IGroupable>().resetToParentGroup();
            }

        }

    }
}