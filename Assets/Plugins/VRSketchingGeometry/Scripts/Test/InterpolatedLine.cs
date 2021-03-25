using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;

public class InterpolatedLine : MonoBehaviour
{
    [SerializeField]
    private LineRenderer LineRenderer;

    //[SerializeField]
    private Vector3[] ControlPoints;

    KochanekBartelsSpline kochanekBartelsSpline;

    [SerializeField]
    private GameObject[] ControlPointObjects;

    private List<Vector3> InterpolatedPoints;

    [SerializeField]
    private GameObject extraControlPoint;

    //public int steps = 20;

    //public bool regenerate = true;
    // Start is called before the first frame update
    void GenerateInterpolatedPoints()
    {
        //kochanekBartelsSpline.setControlPoint(1,ControlPoints[2]);
        LineRenderer.positionCount = InterpolatedPoints.Count;
        LineRenderer.SetPositions(InterpolatedPoints.ToArray());
    }

    private void Start()
    {
        kochanekBartelsSpline = new KochanekBartelsSpline();

        ControlPoints = new Vector3[ControlPointObjects.Length];
        for (int i = 0; i < ControlPointObjects.Length; i++)
        {
            ControlPoints[i] = ControlPointObjects[i].transform.position;
        }

        InterpolatedPoints = kochanekBartelsSpline.InterpolatedPoints;
        kochanekBartelsSpline.SetControlPoints(ControlPoints);
        //kochanekBartelsSpline.insertControlPoint(0, extraControlPoint.transform.position);
        kochanekBartelsSpline.AddControlPoint(extraControlPoint.transform.position);
        kochanekBartelsSpline.DeleteControlPoint(9);
    }

    private void Update()
    {
        ControlPoints = new Vector3[ControlPointObjects.Length];
        for (int i = 0; i < ControlPointObjects.Length; i++) {
            ControlPoints[i] = ControlPointObjects[i].transform.position;
        }
        //if (regenerate) {
        //    regenerate = !regenerate;
            GenerateInterpolatedPoints();
        //}
    }
}
