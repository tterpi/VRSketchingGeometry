using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Splines;

public class ExtrudeTest : MonoBehaviour
{
    [SerializeField]
    private GameObject[] ControlPointObjects;
    [SerializeField]
    private GameObject extraControlPoint;
    private Vector3[] ControlPoints;

    [SerializeField]
    private SplineMesh SplineMesh;

    // Start is called before the first frame update
    void Start()
    {
        ControlPoints = new Vector3[ControlPointObjects.Length];
        for (int i = 0; i < ControlPointObjects.Length; i++)
        {
            ControlPoints[i] = ControlPointObjects[i].transform.position;
        }

        SplineMesh = new SplineMesh(new KochanekBartelsSpline(),GetComponent<MeshFilter>());
        SplineMesh.setControlPoints(ControlPoints);
        //SplineMesh.deleteControlPoint(9);
        //SplineMesh.addControlPoint(extraControlPoint.transform.position);
        //SplineMesh.insertControlPoint(1, extraControlPoint.transform.position);
    }

    private void Update()
    {
        //SplineMesh.setControlPoint(4, ControlPointObjects[4].transform.position);
    }
}
