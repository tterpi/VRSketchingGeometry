//-----------------------------------------------------------------------
//
// Original repository: https://github.com/tterpi/VRSketchingGeometry
//
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Splines {
    /// <summary>
    /// Interface for a spline implementation
    /// </summary>
    public abstract class Spline {
        public List<Vector3> InterpolatedPoints { get; protected set; }

        public abstract void setControlPoints(Vector3[] controlPoints);

        public abstract SplineModificationInfo setControlPoint(int index, Vector3 controlPoint);

        public abstract SplineModificationInfo deleteControlPoint(int index);

        public abstract SplineModificationInfo addControlPoint(Vector3 controlPoint);

        public abstract SplineModificationInfo insertControlPoint(int index, Vector3 controlPoint);

        public abstract int getNumberOfControlPoints();

        public abstract List<Vector3> getControlPoints();
    }
}

