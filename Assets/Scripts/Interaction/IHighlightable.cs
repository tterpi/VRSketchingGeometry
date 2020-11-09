using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.SketchObjectManagement
{
    public interface IHighlightable
    {
        void highlight();
        void revertHighlight();
    }
}
