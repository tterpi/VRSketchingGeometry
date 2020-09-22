using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SketchObjectManagement {
    public abstract class SketchObject : MonoBehaviour, IGroupable
    {
        private GameObject parentGroup;

        public GameObject ParentGroup { get => parentGroup; set => parentGroup = value; }

        public void resetToParentGroup()
        {
            this.transform.SetParent(ParentGroup?.transform);
        }

        public abstract void highlight();
        public abstract void revertHighlight();
    }
}

