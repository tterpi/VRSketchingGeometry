using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SketchObjectManagement {
    /// <summary>
    /// Represents a group of SketchObjects.
    /// This mostly uses the built in behaviour of GameObjects but limits the interface to SketchObjects and other SketchObjectGroups.
    /// SketchObjectGroups can contain SketchObjects and other SketchObjectGroups
    /// </summary>
    public class SketchObjectGroup : MonoBehaviour
    {
        public void addToGroup(SketchObject sketchObject) {
            sketchObject.transform.SetParent(this.transform);
        }

        public void addToGroup(SketchObjectGroup sketchObjectGroup) {
            sketchObjectGroup.transform.SetParent(this.transform);
        }

        public void addToGroup(SketchObjectSelection sketchObjectSelection) {

        }

        public void removeFromGroup(SketchObject sketchObject) {
            //puts it back to the top level of the scene, should be the currently active sketch world in the future
            sketchObject.transform.SetParent(null);
        }

        public void removeFromGroup(SketchObjectGroup sketchObjectGroup)
        {
            //puts it back to the top level of the scene, should be the currently active sketch world in the future
            sketchObjectGroup.transform.SetParent(null);
        }
    }
}
