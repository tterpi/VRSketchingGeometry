using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.SketchObjectManagement
{
    /// <summary>
    /// This interface combines IGroupable and IHighlightable.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public interface ISelectable : IGroupable, IHighlightable
    {
    }
}