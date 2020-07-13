using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Splines {
    public abstract class Spline {
        public List<Vector3> InterpolatedPoints { get; protected set; }

        public abstract void setControlPoints(Vector3[] controlPoints);

        public abstract SplineModificationInfo setControlPoint(int index, Vector3 controlPoint);

        public abstract SplineModificationInfo deleteControlPoint(int index);

        public abstract SplineModificationInfo addControlPoint(Vector3 controlPoint);

        public abstract SplineModificationInfo insertControlPoint(int index, Vector3 controlPoint);

        public abstract int getNumberOfControlPoints();
    }
}

