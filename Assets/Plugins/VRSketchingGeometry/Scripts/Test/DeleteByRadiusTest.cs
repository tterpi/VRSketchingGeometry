using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

public class DeleteByRadiusTest : MonoBehaviour
{
    public GameObject selectionPrefab;
    public GameObject LineSketchObjectPrefab;
    private LineSketchObject lineSketchObject;
    public GameObject controlPointParent;
    public GameObject deletePoint;
    public float deleteRadius;

    private bool ranOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        lineSketchObject = Instantiate(LineSketchObjectPrefab).GetComponent<LineSketchObject>();
        lineSketchObject.setLineDiameter(.5f);
    }

    IEnumerator changeDiameter() {
        yield return new WaitForSeconds(5);
        lineSketchObject.DeleteControlPoints(deletePoint.transform.position, deleteRadius);
        //lineSketchObject.setLineDiameter(.1f);
        //yield return new WaitForSeconds(2);
        //lineSketchObject.deleteControlPoint();
        //lineSketchObject.deleteControlPoint();

    }

    IEnumerator deactivateSelection(SketchObjectSelection selection) {
        yield return new WaitForSeconds(3);
        selection.deactivate();
    }

    private void lineSketchObjectTest() {

        foreach (Transform controlPoint in controlPointParent.transform) {
            lineSketchObject.addControlPoint(controlPoint.position);
        }

        //lineSketchObject.setLineDiameter(.7f);
        StartCoroutine(changeDiameter());

        //StartCoroutine(deactivateSelection(selection));
    }

    // Update is called once per frame
    void Update()
    {
        if (!ranOnce) {
            lineSketchObjectTest();
            ranOnce = true;
        }
    }
}
