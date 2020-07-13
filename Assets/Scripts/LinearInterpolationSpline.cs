using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Splines {
    public class LinearInterpolationSpline: Spline
    {
        private List<Vector3> ControlPoints;

        public LinearInterpolationSpline() {
            ControlPoints = new List<Vector3>();
            InterpolatedPoints = ControlPoints;
        }

        public override void setControlPoints(Vector3[] controlPoints) {
            ControlPoints.Clear();
            ControlPoints.AddRange(controlPoints);
        }

        public override SplineModificationInfo setControlPoint(int index, Vector3 controlPoint) {
            ControlPoints[index] = controlPoint;
            return new SplineModificationInfo(index, 1, 1);
        }

        public override SplineModificationInfo deleteControlPoint(int index) {
            ControlPoints.RemoveAt(index);
            return new SplineModificationInfo(index, 1, 0);
        }

        public override SplineModificationInfo addControlPoint(Vector3 controlPoint) {
            ControlPoints.Add(controlPoint);
            return new SplineModificationInfo(ControlPoints.Count - 1, 0, 1);
        }

        public override SplineModificationInfo insertControlPoint(int index, Vector3 controlPoint) {
            ControlPoints.Insert(index, controlPoint);
            return new SplineModificationInfo(index, 0, 1);
        }

        public override int getNumberOfControlPoints()
        {
            return ControlPoints.Count;
        }
    }
}

