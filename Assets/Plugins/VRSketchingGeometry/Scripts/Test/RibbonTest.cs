using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Meshing;

public class RibbonTest : MonoBehaviour
{
    public GameObject pointerObject;

    public GameObject ControlPointParent;

    private RibbonMesh ribbonMesh;

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

    // Start is called before the first frame update
    void Start()
    {
        ribbonMesh = new RibbonMesh(Vector3.one * .5f);
        StartCoroutine(addPointCoroutine());
        //setPointsToRibbon();
        //addPointsToRibbon();

    }

    // Update is called once per frame
    void Update()
    {
        //generateRibbon();
    }
}
