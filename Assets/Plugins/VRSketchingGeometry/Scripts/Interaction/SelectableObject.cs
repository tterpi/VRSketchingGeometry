using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.SketchObjectManagement
{
    public abstract class SelectableObject : MonoBehaviour, ISelectable
    {
        public abstract SketchObjectGroup ParentGroup { get; set; }

        public abstract void highlight();
        public abstract void resetToParentGroup();
        public abstract void revertHighlight();
    }
}
