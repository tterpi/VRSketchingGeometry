using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SketchObjectManagement {
    public abstract class SketchObject : MonoBehaviour
    {
        public GameObject parentGroup;
        public abstract void highlight();
        public abstract void revertHighlight();
    }
}

