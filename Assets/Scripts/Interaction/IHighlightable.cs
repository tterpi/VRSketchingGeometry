using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SketchObjectManagement
{
    public interface IHighlightable
    {
        void highlight();
        void revertHighlight();
    }
}
