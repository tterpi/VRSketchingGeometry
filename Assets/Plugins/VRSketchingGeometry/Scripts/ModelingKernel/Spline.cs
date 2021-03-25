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
    /// Interface for a spline implementation
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public abstract class Spline {
        public List<Vector3> InterpolatedPoints { get; protected set; }

        /// <summary>
        /// Set all control points.
        /// </summary>
        /// <param name="controlPoints"></param>
        public abstract void SetControlPoints(Vector3[] controlPoints);

        /// <summary>
        /// Replace control point at index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="controlPoint"></param>
        /// <returns></returns>
        public abstract SplineModificationInfo SetControlPoint(int index, Vector3 controlPoint);

        /// <summary>
        /// Delete a control point at index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract SplineModificationInfo DeleteControlPoint(int index);

        /// <summary>
        /// Add control point at the end of the spline.
        /// </summary>
        /// <param name="controlPoint"></param>
        /// <returns></returns>
        public abstract SplineModificationInfo AddControlPoint(Vector3 controlPoint);

        /// <summary>
        /// Insert control point at index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="controlPoint"></param>
        /// <returns></returns>
        public abstract SplineModificationInfo InsertControlPoint(int index, Vector3 controlPoint);

        public abstract int GetNumberOfControlPoints();

        public abstract List<Vector3> GetControlPoints();
    }
}

