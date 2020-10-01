using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SketchObjectManagement {
    /// <summary>
    /// Represents a group of SketchObjects.
    /// This mostly uses the built in behaviour of GameObjects but limits the interface to SketchObjects and other SketchObjectGroups.
    /// SketchObjectGroups can contain SketchObjects and other SketchObjectGroups
    /// </summary>
    public class SketchObjectGroup : MonoBehaviour, IGroupable, IHighlightable
    {
        private GameObject parentGroup;

        public GameObject ParentGroup { get => parentGroup; set => parentGroup = value; }

        public void addToGroup(SketchObject sketchObject) {
            sketchObject.transform.SetParent(this.transform);
            sketchObject.ParentGroup = this.gameObject;
        }

        public void addToGroup(SketchObjectGroup sketchObjectGroup) {
            sketchObjectGroup.transform.SetParent(this.transform);
            sketchObjectGroup.parentGroup = this.gameObject;
        }

        public void addToGroup(SketchObjectSelection sketchObjectSelection) {
            foreach (Transform selected in sketchObjectSelection.transform) {
                SketchObject so = selected.gameObject.GetComponent<SketchObject>();
                SketchObjectGroup sog = selected.gameObject.GetComponent<SketchObjectGroup>();

                if (so != null)
                {
                    addToGroup(so);
                }
                else if (sog != null) {
                    addToGroup(sog);
                }
            }
        }


        public void removeFromGroup(SketchObject sketchObject) {
            removeFromGroup(sketchObject.gameObject);
        }

        public void removeFromGroup(SketchObjectGroup sketchObjectGroup)
        {
            removeFromGroup(sketchObjectGroup.gameObject);
        }

        private void removeFromGroup(GameObject gameObject) {
            if (SketchWorld.ActiveSketchWorld != null)
            {
                gameObject.transform.SetParent(SketchWorld.ActiveSketchWorld.transform);
            }
            else {
                gameObject.transform.SetParent(null);
            }
        }

        public void resetToParentGroup() {
            this.transform.SetParent(ParentGroup?.transform);
        }

        public void highlight()
        {
            this.gameObject.BroadcastMessage(nameof(IHighlightable.highlight));
        }

        public void revertHighlight()
        {
            this.gameObject.BroadcastMessage(nameof(IHighlightable.revertHighlight));
        }
    }
}
