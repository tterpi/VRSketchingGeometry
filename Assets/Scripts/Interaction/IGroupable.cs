using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.SketchObjectManagement
{
    public interface IGroupable
    {
        GameObject ParentGroup { get; set; }
        void resetToParentGroup();
    }
}
