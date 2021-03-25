using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.SketchObjectManagement
{
    /// <summary>
    /// Classes that implement this interface can be added to SketchObjectGroups.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public interface IGroupable
    {
        SketchObjectGroup ParentGroup { get; set; }
        void resetToParentGroup();
    }
}
