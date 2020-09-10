using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SketchObjectManagement;

public class SketchObjectSelection : MonoBehaviour
{
    private static SketchObjectSelection activeSketchObjectSelection;

    private List<GameObject> sketchObjectsOfSelection;

    public static SketchObjectSelection ActiveSketchObjectSelection
    {
        get { return activeSketchObjectSelection; }
        set
        {
            activeSketchObjectSelection = value;
        }
    }

    public void addToSelection(SketchObject sketchObject) {
        addToSelection(sketchObject.gameObject);
    }

    public void addToSelection(SketchObjectGroup sketchObjectGroup) {
        //should check if any of the sketch objects of the group are already in the selection first
        //is child of
        foreach (Transform child in sketchObjectGroup.transform) {
            if (sketchObjectsOfSelection.Contains(child.gameObject))
            {
                //remove from selection if part of group, it will be added again as part of the group
                sketchObjectsOfSelection.Remove(child.gameObject);
            }
            else {
                foreach (GameObject goInSelection in sketchObjectsOfSelection) {
                    if (child.transform.IsChildOf(goInSelection.transform)) {
                        Debug.LogWarning("Selection already contains game objects from group.");
                    }
                }
            }

        }
        addToSelection(sketchObjectGroup.gameObject);
    }

    private void addToSelection(GameObject gameObject) {
        if (!sketchObjectsOfSelection.Contains(gameObject))
        {
            sketchObjectsOfSelection.Add(gameObject);
        }
        else
        {
            Debug.LogWarning("SketchObjectSelection already contains object");
        }
    }

    public void removeFromSelection(SketchObject sketchObject)
    {
        removeFromSelection(sketchObject.gameObject);
    }

    private void removeFromSelection(GameObject gameObject) {
        if (sketchObjectsOfSelection.Contains(gameObject))
        {
            sketchObjectsOfSelection.Remove(gameObject);
        }
    }

    public void activate() {
        if (ActiveSketchObjectSelection != this) {
            ActiveSketchObjectSelection.deactivate();
            ActiveSketchObjectSelection = this;
        }
        //todo highlight objects of selection
    }

    public void deactivate() {
        if (ActiveSketchObjectSelection == this) {
            ActiveSketchObjectSelection = null;
        }
        //todo revert highlighting of selection
        //gameObject.BroadcastMessage
    }

}
