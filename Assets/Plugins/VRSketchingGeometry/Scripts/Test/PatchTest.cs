using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

public class PatchTest : MonoBehaviour
{

    public GameObject controlPointParent;
    public GameObject patchPrefab;
    public PatchSketchObject patchSketchObject;
    private int index = 0;

    private IEnumerator AddSegmentTest() {

        bool quit = false;

        List<Vector3> controlPoints = new List<Vector3>();

        for (int i = 0; i < 4; i++)
        {
            Transform controlPoint = null;
            if (index < controlPointParent.transform.childCount) {
                controlPoint = controlPointParent.transform.GetChild(index);
            }
            index++;
            if (controlPoint != null)
            {
                controlPoints.Add(controlPoint.position);
            }
            else {
                quit = true;
            }
        }

        patchSketchObject.AddPatchSegment(controlPoints);
        yield return new WaitForSeconds(.5f);
        if (!quit)
        {
            StartCoroutine(AddSegmentTest());
        }
        else {
            patchSketchObject.RemovePatchSegment();
        }
    }

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
        patchSketchObject = Instantiate(patchPrefab).GetComponent<PatchSketchObject>();
        patchSketchObject.Width = 4;
        patchSketchObject.ResolutionHeight = 8;
        patchSketchObject.ResolutionWidth = 8;
        //StartCoroutine(AddSegmentTest());
    }

    // Update is called once per frame
    void Update()
    {
        setAllControlPoints();
    }
}
