using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SketchObjectManagement;

namespace SketchObjectManagement
{
    public class SketchObjectSelection : MonoBehaviour
    {
        private static SketchObjectSelection activeSketchObjectSelection;

        //private List<GameObject> sketchObjectsOfSelection = new List<GameObject>();

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
            gameObject.transform.SetParent(this.transform);
        }

        public void removeFromSelection(SketchObject sketchObject)
        {
            //removeFromSelection(sketchObject.gameObject);
            sketchObject.transform.SetParent(sketchObject.parentGroup.transform);
        }

        public void removeFromSelection(SketchObjectGroup sketchObjectGroup)
        {
            //removeFromSelection(sketchObjectGroup.gameObject);
            sketchObjectGroup.transform.SetParent(sketchObjectGroup.parentGroup.transform);
        }

        public void deleteObjectsOfSelection()
        {
            foreach (Transform child in this.transform) {
                SketchWorld.ActiveSketchWorld.deleteObject(child.gameObject);
            }
            //deactivate game objects
            //parent to bin object
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

            gameObject.BroadcastMessage(nameof(SketchObject.highlight));
        }

        public void deactivate()
        {
            if (ActiveSketchObjectSelection == this)
            {
                ActiveSketchObjectSelection = null;
            }

            gameObject.BroadcastMessage(nameof(SketchObject.revertHighlight));
        }

    }
}