using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

[ExecuteInEditMode]
public class PatchSketchObjectSetup : MonoBehaviour
{

    public GameObject controlPointParent;
    public GameObject patchPrefab;
    public PatchSketchObject patchSketchObject;
    public int PatchWidth;
    public int PatchResolution;

    public void setAllControlPoints() {
        List<Vector3> controlPoints = new List<Vector3>();
        foreach (Transform child in controlPointParent.transform) {
            controlPoints.Add(child.position);
        }
        this.patchSketchObject.SetControlPoints(controlPoints, 4);
    }

    // Start is called before the first frame update
    
    void Start()
    {
        //patchSketchObject = Instantiate(patchPrefab).GetComponent<PatchSketchObject>();
        patchSketchObject.Width = PatchWidth;
        patchSketchObject.ResolutionHeight = PatchResolution;
        patchSketchObject.ResolutionWidth = PatchResolution;
        //StartCoroutine(AddSegmentTest());
    }

    // Update is called once per frame
    void Update()
    {
        setAllControlPoints();
    }
}
