using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.SketchObjectManagement;

public class RibbonTest : MonoBehaviour
{
    public GameObject pointerObject;

    public GameObject ControlPointParent;

    private RibbonMesh ribbonMesh;

    public GameObject RibbonPrefab;
    private RibbonSketchObject RibbonSketchObject;

    private void generateRibbon() {
        List<Vector3> points = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();
        foreach (Transform controlPoint in ControlPointParent.transform)
        {
            points.Add(controlPoint.position);
            rotations.Add(controlPoint.rotation);
        }

        GetComponent<MeshFilter>().mesh = RibbonMesh.GetRibbonMesh(points, rotations, .4f);
    }

    private void addPointsToRibbon() {

        List<Vector3> points = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();
        foreach (Transform controlPoint in ControlPointParent.transform)
        {
            GetComponent<MeshFilter>().mesh = ribbonMesh.AddPoints(new List<Vector3> { controlPoint.position }, new List<Quaternion> { controlPoint.rotation });
            points.Add(controlPoint.position);
            rotations.Add(controlPoint.rotation);
        }

        //GetComponent<MeshFilter>().mesh = RibbonMesh.GetRibbonMesh(points, rotations, .4f);
    }

    private void setPointsToRibbon()
    {

        List<Vector3> points = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();
        foreach (Transform controlPoint in ControlPointParent.transform)
        {
            //GetComponent<MeshFilter>().mesh = ribbonMesh.AddPoints(new List<Vector3> { controlPoint.position }, new List<Quaternion> { controlPoint.rotation });
            points.Add(controlPoint.position);
            rotations.Add(controlPoint.rotation);
        }

        //GetComponent<MeshFilter>().mesh = RibbonMesh.GetRibbonMesh(points, rotations, .4f);
        GetComponent<MeshFilter>().mesh=  ribbonMesh.GetMesh(points, rotations);
    }

    IEnumerator addPointCoroutine() {
        GetComponent<MeshFilter>().mesh = ribbonMesh.AddPoint(pointerObject.transform.position, pointerObject.transform.rotation);
        yield return new WaitForSeconds(.05f);
        StartCoroutine(addPointCoroutine());
    }

    public void addPointRibbonSketchObject()
    {
        RibbonSketchObject.AddControlPointContinuous(pointerObject.transform.position, pointerObject.transform.rotation);
        RibbonSketchObject.SetCrossSection(new List<Vector3> { new Vector3(.5f, 0, 0), new Vector3(0, .5f, 0),new Vector3(0, .5f, 0), new Vector3(-.5f, 0, 0) }, Vector3.one);
    }

    public static (List<Vector3>, List<Quaternion>) GetPointTransformation(GameObject parent) {
        List<Vector3> points = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();
        foreach (Transform controlPoint in parent.transform)
        {
            points.Add(controlPoint.position);
            rotations.Add(controlPoint.rotation);
        }

        return (points, rotations);
    }

    private void RibbonSketchObjectTest() {
        (List<Vector3> thePoints, List<Quaternion> theRotations) = GetPointTransformation(ControlPointParent);
        RibbonSketchObject.SetControlPoints(thePoints, theRotations);
        RibbonSketchObject.DeleteControlPoint();
    }

    // Start is called before the first frame update
    void Start()
    {
        RibbonSketchObject = Instantiate(RibbonPrefab).GetComponent<RibbonSketchObject>();
        //RibbonSketchObjectTest();
        //ribbonMesh = new RibbonMesh(Vector3.one * .5f);
        //StartCoroutine(addPointCoroutine());
        //setPointsToRibbon();
        //addPointsToRibbon();

    }

    // Update is called once per frame
    void Update()
    {
        //generateRibbon();
        addPointRibbonSketchObject();
    }
}
