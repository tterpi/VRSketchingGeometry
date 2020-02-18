using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KochanekBartelsSplines;

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
    //public int steps = 20;

    //public bool regenerate = true;
    // Start is called before the first frame update
    void GenerateInterpolatedPoints()
    {
        kochanekBartelsSpline.setControlPoints(ControlPoints);
        LineRenderer.positionCount = InterpolatedPoints.Count;
        LineRenderer.SetPositions(InterpolatedPoints.ToArray());
    }

    private void Start()
    {
        kochanekBartelsSpline = new KochanekBartelsSpline();
        InterpolatedPoints = kochanekBartelsSpline.InterpolatedPoints;
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
