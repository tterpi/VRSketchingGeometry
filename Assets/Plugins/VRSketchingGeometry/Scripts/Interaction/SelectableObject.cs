using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.SketchObjectManagement
{
    /// <summary>
    /// Abstract class for a object that can be selected.
    /// SketchObject and SketchObjectGroup are derived from this class.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public abstract class SelectableObject : MonoBehaviour, ISelectable
    {
        public abstract SketchObjectGroup ParentGroup { get; set; }

        public abstract void highlight();
        public abstract void resetToParentGroup();
        public abstract void revertHighlight();
    }
}
