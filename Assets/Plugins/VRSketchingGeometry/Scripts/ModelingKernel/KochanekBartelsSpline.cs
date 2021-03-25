//-----------------------------------------------------------------------
//
// Original repository: https://github.com/tterpi/VRSketchingGeometry
//
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Splines
{
    /// <summary>
    /// Contains information about what parts of the spline were modified
    /// </summary>
    public struct SplineModificationInfo{
        public int Index { get; private set; }
        public int RemoveCount { get; private set; }
        public int AddCount { get; private set; }

        public SplineModificationInfo(int index, int removeCount, int addCount) {
            Index = index;
            RemoveCount = removeCount;
            AddCount = addCount;
        }

        public override string ToString() {
            return "Index: " + Index + "; RemoveCount: " + RemoveCount + "; AddCount: " + AddCount;
        }
    }

    /// <summary>
    /// Hermite interpolated spline with Kochanek-Bartels tangent calculation
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class KochanekBartelsSpline : Spline
    {
        private List<KochanekBartelsControlPoint> ControlPoints { get; set; }
        private int Steps;

        /// <summary>
        /// Constructor for the KochanekBartelsSpline class.
        /// </summary>
        /// <param name="steps">Number of interpolation steps for each segment.</param>
        public KochanekBartelsSpline(int steps = 20) {
            ControlPoints = new List<KochanekBartelsControlPoint>();
            InterpolatedPoints = new List<Vector3>();
            Steps = steps;
        }

        /// <summary>
        /// Reset the existing control point at index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="controlPoints"></param>
        public SplineModificationInfo SetControlPoint(int index, KochanekBartelsControlPoint controlPoint) {
            ControlPoints[index] = controlPoint;

            int start = (index - 2) >= 0 ? (index-2) : 0 ;
            int end = (index + 1) <= (ControlPoints.Count -2) ? (index + 1) : (ControlPoints.Count - 2);

            for (int i = start; i <= end; i++) {
                try
                {
                    InterpolatedPoints.RemoveRange(i * Steps, Steps);
                    InterpolatedPoints.InsertRange(i * Steps, InterpolateSegment(i));
                }
                catch (ArgumentException exception)
                {
                    Debug.LogError("Can't set control point that doesnt exist yet, add instead!\n" + exception);
                }
            }

            return new SplineModificationInfo(start * Steps, (end - start + 1) * Steps, (end - start + 1) * Steps);
        }

        /// <summary>
        /// Add control point at the end of the curve.
        /// </summary>
        /// <param name="controlPoint"></param>
        public SplineModificationInfo AddControlPoint(KochanekBartelsControlPoint controlPoint) {
            return InsertControlPoint(ControlPoints.Count, controlPoint);
        }

        /// <summary>
        /// Insert a control point at index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="controlPoint"></param>
        public SplineModificationInfo InsertControlPoint(int index, KochanekBartelsControlPoint controlPoint) {

            ControlPoints.Insert(index, controlPoint);

            if (ControlPoints.Count < 3 && InterpolatedPoints.Count < 2 * Steps)
            {
                Debug.LogWarning("KochanekBartelsSpline: Control point was added but the line can only be interpolated when there are at least 3 control points.");
                return new SplineModificationInfo(0, 0, 0);
            }
            else if (ControlPoints.Count == 3 && InterpolatedPoints.Count == Steps)
            {
                this.SetControlPoints(ControlPoints.ToArray());
                return new SplineModificationInfo(0, Steps, 2 * Steps);
            }
            else if (ControlPoints.Count == 3) {
                this.SetControlPoints(ControlPoints.ToArray());
                return new SplineModificationInfo(0, 0, 2 * Steps);
            }

            //determine which segments have to be reinterpolated
            int start = (index - 2) >= 0 ? (index - 2) : 0;
            int end = (index + 1) <= (ControlPoints.Count - 2) ? (index + 1) : (ControlPoints.Count - 2);

            for (int i = start; i <= end; i++)
            {
                try
                {
                    if (index == ControlPoints.Count-1)
                    {
                        //if we are inserting the control point as the new last element
                        //dont try to remove the segment before the new control point because it doesnt exist yet
                        if (i != index - 1)
                        {
                            //dont remove at added index so a new segment is added
                            InterpolatedPoints.RemoveRange(i * Steps, Steps);
                        }
                    }
                    else {
                        if (i != index)
                        {
                            //dont remove at added index so a new segment is added
                            InterpolatedPoints.RemoveRange(i * Steps, Steps);
                        }
                    }
                    InterpolatedPoints.InsertRange(i * Steps, InterpolateSegment(i));

                }
                catch (ArgumentException exception)
                {
                    Debug.LogError("Can't insert control point at index that doesnt exist yet, add instead!\n" + exception);
                }
            }

            return new SplineModificationInfo(start * Steps, (end - start) * Steps, (end - start + 1) * Steps);
        }

        /// <summary>
        /// Delete the control point at index.
        /// If the index is not the first or last control point the curve will skip the deleted control point and connect the control point before and after the deleted one.
        /// </summary>
        /// <param name="index"></param>
        public override SplineModificationInfo DeleteControlPoint(int index) {

            //if ((ControlPoints.Count - 1) < 3) {
            //    Debug.LogError("Cannot remove more control points, minimum number is 3.");
            //    return new SplineModificationInfo(0,0,0);
            //}

            //Check if there is only one point left
            if (ControlPoints.Count == 1 && index == 0) {
                //nothing left to remove
            }
            else if (index == (ControlPoints.Count - 1))
            {
                InterpolatedPoints.RemoveRange((index -1) * Steps, Steps);
            }
            else {
                InterpolatedPoints.RemoveRange(index * Steps, Steps);
            }

            ControlPoints.RemoveAt(index);

            if (ControlPoints.Count == 2)
            {
                int startIndex = (index - 1) >= 0 ? (index - 1) : 0;
                return new SplineModificationInfo(startIndex * Steps, Steps, 0);
            }
            else if (ControlPoints.Count < 2) {
                return new SplineModificationInfo(0, index * Steps, 0);
            }

            //determine which segments have to be reinterpolated
            int start = (index - 2) >= 0 ? (index - 2) : 0;
            int end = (index +1) <= (ControlPoints.Count - 2) ? (index+1) : (ControlPoints.Count - 2);

            for (int i = start; i <= end; i++)
            {
                try
                {
                    InterpolatedPoints.RemoveRange(i * Steps, Steps);
                    InterpolatedPoints.InsertRange(i * Steps, InterpolateSegment(i));
                }
                catch (ArgumentException exception)
                {
                    Debug.LogError("Can't delete control point at index that doesnt exist yet!\n" + exception);
                }
            }

            return new SplineModificationInfo(start * Steps, (end - start + 2) * Steps, (end - start+1) * Steps);
        }

        /// <summary>
        /// Set all control points and recalculate.
        /// </summary>
        /// <param name="controlPoints"></param>
        public override void SetControlPoints(Vector3[] controlPoints) {
            ControlPoints.Clear();
            foreach (Vector3 controlPoint in controlPoints)
            {
                ControlPoints.Add(controlPoint);
            }
            InterpolateSpline();
        }

        public void SetControlPoints(KochanekBartelsControlPoint[] controlPoints) {
            ControlPoints.Clear();
            ControlPoints.AddRange(controlPoints);
            InterpolateSpline();
        }

        private List<Vector3> InterpolateSegment(int controlPointIndex) {
            if (controlPointIndex == ControlPoints.Count - 2)
            {
                return InterpolateSegment(GetControlPointsForSegment(controlPointIndex), Steps, true);
            }
            else {
                return InterpolateSegment(GetControlPointsForSegment(controlPointIndex), Steps);
            }
        }

        /// <summary>
        /// Calculate one segment.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="steps">Steps for this segement, between point2 and point3.</param>
        /// <returns></returns>
        private static List<Vector3> InterpolateSegment(KochanekBartelsControlPoint point1, KochanekBartelsControlPoint point2, KochanekBartelsControlPoint point3, KochanekBartelsControlPoint point4, int steps, bool isLastSegment = false)
        {
            var interpolatedPoints = new List<Vector3>();

            Vector3 point2DestinationTangent = point2.CalculateDestinationTangent(point1, point3);
            Vector3 point3SourceTangent = point3.CalculateSourceTangent(point2, point4);

            for (var i = 0; i < steps; i++)
            {
                float s;
                if (isLastSegment)
                {
                    s = i / (float)(steps - 1);
                }
                else {
                    s = i / (float)(steps);
                }

                var newPoint = CalculateInterpolatedPoint(point2.Position, point3.Position, point2DestinationTangent, point3SourceTangent, s);

                interpolatedPoints.Add(newPoint);
            }

            return interpolatedPoints;
        }

        /// <summary>
        /// Calculate a single interpolated point between two control points.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="tangent1"></param>
        /// <param name="tangent2"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static Vector3 CalculateInterpolatedPoint(Vector3 point1, Vector3 point2, Vector3 tangent1, Vector3 tangent2, float parameter) {

            float h1 = (float)(2 * Math.Pow(parameter, 3) - 3 * Math.Pow(parameter, 2) + 1);
            float h2 = (float)((-2) * Math.Pow(parameter, 3) + 3 * Math.Pow(parameter, 2));
            float h3 = (float)(Math.Pow(parameter, 3) - 2 * Math.Pow(parameter, 2) + parameter);
            float h4 = (float)(Math.Pow(parameter, 3) - Math.Pow(parameter, 2));

            return h1 * point1 + h2 * point2 + h3 * tangent1 + h4 * tangent2;
        }

        private static List<Vector3> InterpolateSegment(List<KochanekBartelsControlPoint> controlPoints, int steps, bool isLastSegment = false) {
            if (controlPoints != null && controlPoints.Count == 4)
            {
                if (isLastSegment)
                {
                    return InterpolateSegment(controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3], steps, true);
                }
                else {
                    return InterpolateSegment(controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3], steps);
                }
            }
            else {
                Debug.LogError("List must have 4 control points");
                return null;
            }
        }

        /// <summary>
        /// Recalculate the complete spline.
        /// </summary>
        /// <param name="steps">Steps for each segment</param>
        private void InterpolateSpline()
        {
            InterpolatedPoints.Clear();
            if (ControlPoints.Count < 3)
            {
                Debug.LogWarning("KochanekBartelsSpline: Not enough control points! Minimum is 3.");
                return;
            }
            for (int i = 0; i < ControlPoints.Count-1; i++)
            {
                List<Vector3> points = InterpolateSegment(i);
                if (points != null && points.Count > 0) {
                    InterpolatedPoints.AddRange(points);
                }
            }
        }

        private List<KochanekBartelsControlPoint> GetControlPointsForSegment(int index) {
            int i = index;

            if (i< 0 || i >= ControlPoints.Count - 1) {
                Debug.LogError("Index out of range! Last segment is at number of control points minus one.");
                return null;
            }else if(ControlPoints.Count < 3)
            {
                Debug.LogError("There have to be at least 3 control points.");
                return null;
            }

            List<KochanekBartelsControlPoint> controlPointsOfSegment = new List<KochanekBartelsControlPoint>(4);

            //first control point
            if (i == 0)
            {
                controlPointsOfSegment.Add(ControlPoints[i]);
                controlPointsOfSegment.Add(ControlPoints[i]);
                controlPointsOfSegment.Add(ControlPoints[i + 1]);
                controlPointsOfSegment.Add(ControlPoints[i + 2]);
            }
            //middle control point
            else if (i + 2 < ControlPoints.Count)
            {
                controlPointsOfSegment.Add(ControlPoints[i - 1]);
                controlPointsOfSegment.Add(ControlPoints[i]);
                controlPointsOfSegment.Add(ControlPoints[i + 1]);
                controlPointsOfSegment.Add(ControlPoints[i + 2]);
            }
            //last control point
            else if (i + 1 < ControlPoints.Count)
            {
                controlPointsOfSegment.Add(ControlPoints[i - 1]);
                controlPointsOfSegment.Add(ControlPoints[i]);
                controlPointsOfSegment.Add(ControlPoints[i + 1]);
                controlPointsOfSegment.Add(ControlPoints[i + 1]);
            }
            return controlPointsOfSegment;
        }

        public override int GetNumberOfControlPoints() {
            return ControlPoints.Count;
        }

        public override List<Vector3> GetControlPoints() {
            List<Vector3> vectorControlPoints = ControlPoints.Select(controlPoint => controlPoint.Position).ToList();
            return vectorControlPoints;
        }

        //Adapters for the interface
        public override SplineModificationInfo AddControlPoint(Vector3 controlPoint)
        {
            return this.AddControlPoint(controlPoint);
        }

        public override SplineModificationInfo SetControlPoint(int index, Vector3 controlPoint)
        {
            return this.SetControlPoint(index, controlPoint);
        }

        public override SplineModificationInfo InsertControlPoint(int index, Vector3 controlPoint)
        {
            return this.InsertControlPoint(index, controlPoint);
        }


    }
}