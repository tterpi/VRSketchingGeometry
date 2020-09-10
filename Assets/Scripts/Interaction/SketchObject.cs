using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SketchObject : MonoBehaviour
{
    public abstract void highlight();
    public abstract void revertHighlight();
}
