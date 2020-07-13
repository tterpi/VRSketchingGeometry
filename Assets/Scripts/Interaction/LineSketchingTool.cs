using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore.Examples.ObjectManipulation;

public class LineSketchingTool : MonoBehaviour
{
    // select a line

    // call functions of line

    public LineSketchObject lineSketch;

    public Camera mainCamera = null;

    public GameObject pointMarker;

    public void Start() {
        //mainCamera = Camera.allCameras[0];
        if (!mainCamera) return;
        //pointMarker.transform.position = mainCamera.transform.position + mainCamera.transform.forward * .2f;
        pointMarker.transform.SetParent(mainCamera.transform);
        pointMarker.transform.localPosition = Vector3.forward * .5f;
    }

    private void getCurrentSelectedSketchObject() {
        lineSketch = ManipulationSystem.Instance.SelectedObject.GetComponentInChildren<LineSketchObject>();
    }

    public void addControlPoint() {
        getCurrentSelectedSketchObject();
        lineSketch.addControlPoint(pointMarker.transform.position);
    }

    public void deleteControlPoint() {
        getCurrentSelectedSketchObject();
        lineSketch.deleteControlPoint();
    }

}
