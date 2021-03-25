using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.SketchObjectManagement
{
    /// <summary>
    /// Classes that implement this interface provide methods for visually highlighting their geometry.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public interface IHighlightable
    {
        void highlight();
        void revertHighlight();
    }
}
