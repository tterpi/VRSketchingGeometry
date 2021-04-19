//-----------------------------------------------------------------------
//
// Original repository: https://github.com/tterpi/VRSketchingGeometry
//
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Splines {
    /// <summary>
    /// Spline that directly represents the provided control points, does not perform any interpolation.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class LinearInterpolationSpline: Spline
    {
        private List<Vector3> ControlPoints;

        public LinearInterpolationSpline() {
            ControlPoints = new List<Vector3>();
            InterpolatedPoints = ControlPoints;
        }

        public override void SetControlPoints(Vector3[] controlPoints) {
            ControlPoints.Clear();
            ControlPoints.AddRange(controlPoints);
        }

        public override SplineModificationInfo SetControlPoint(int index, Vector3 controlPoint) {
            ControlPoints[index] = controlPoint;
            return new SplineModificationInfo(index, 1, 1);
        }

        public override SplineModificationInfo DeleteControlPoint(int index) {
            if (InterpolatedPoints.Count == 0) {
                return new SplineModificationInfo(0, 0, 0);
            }
            ControlPoints.RemoveAt(index);
            if (InterpolatedPoints.Count == 0) {
                return new SplineModificationInfo(0, 0, 0);
            }
            else if (InterpolatedPoints.Count == 1)
            {
                return new SplineModificationInfo(0, 2, 0);
            }
            return new SplineModificationInfo(index, 1, 0);
        }

        public override SplineModificationInfo AddControlPoint(Vector3 controlPoint) {
            ControlPoints.Add(controlPoint);
            if (InterpolatedPoints.Count < 2) {
                return new SplineModificationInfo(0, 0, 0);
            } else if(InterpolatedPoints.Count == 2){
                return new SplineModificationInfo(0, 0, 2);
            }
            return new SplineModificationInfo(ControlPoints.Count - 1, 0, 1);
        }

        public override SplineModificationInfo InsertControlPoint(int index, Vector3 controlPoint) {
            ControlPoints.Insert(index, controlPoint);
            if (InterpolatedPoints.Count < 2)
            {
                return new SplineModificationInfo(0, 0, 0);
            }
            else if (InterpolatedPoints.Count == 2)
            {
                return new SplineModificationInfo(0, 0, 2);
            }

            return new SplineModificationInfo(index, 0, 1);
        }

        public override int GetNumberOfControlPoints()
        {
            return ControlPoints.Count;
        }

        public override List<Vector3> GetControlPoints()
        {
            return ControlPoints;
        }
    }
}

