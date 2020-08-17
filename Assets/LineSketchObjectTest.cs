using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSketchObjectTest : MonoBehaviour
{
    public GameObject LineSketchObjectPrefab;
    private LineSketchObject lineSketchObject;

    private bool ranOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        lineSketchObject = Instantiate(LineSketchObjectPrefab).GetComponent<LineSketchObject>();
    }

    IEnumerator changeDiameter() {
            yield return new WaitForSeconds(5);
            lineSketchObject.setLineDiameter(.1f);
    }

    private void lineSketchObjectTest() {
        lineSketchObject.addControlPoint(Vector3.zero);
        lineSketchObject.addControlPoint(Vector3.one);
        lineSketchObject.addControlPoint(new Vector3(2, 2, 0));
        lineSketchObject.addControlPoint(new Vector3(2, 1, 0));

        lineSketchObject.setLineDiameter(.7f);

        StartCoroutine(changeDiameter());
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
