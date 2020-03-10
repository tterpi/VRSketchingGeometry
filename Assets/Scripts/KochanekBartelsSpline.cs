using System;
using System.Collections.Generic;
using UnityEngine;

namespace Splines
{
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

    public class KochanekBartelsSpline
    {
        private List<KochanekBartelsControlPoint> ControlPoints { get; set; }
        public List<Vector3> InterpolatedPoints { get; private set; }
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
        public SplineModificationInfo setControlPoint(int index, KochanekBartelsControlPoint controlPoint) {
            ControlPoints[index] = controlPoint;

            int start = (index - 2) >= 0 ? (index-2) : 0 ;
            int end = (index + 1) <= (ControlPoints.Count -2) ? (index + 1) : (ControlPoints.Count - 2);

            for (int i = start; i <= end; i++) {
                try
                {
                    InterpolatedPoints.RemoveRange(i * Steps, Steps);
                    InterpolatedPoints.InsertRange(i * Steps, InterpolateSegment(getControlPointsForSegment(i), Steps));
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
        public SplineModificationInfo addControlPoint(KochanekBartelsControlPoint controlPoint) {
            return insertControlPoint(ControlPoints.Count, controlPoint);
        }

        /// <summary>
        /// Insert a control point at index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="controlPoint"></param>
        public SplineModificationInfo insertControlPoint(int index, KochanekBartelsControlPoint controlPoint) {
            ControlPoints.Insert(index, controlPoint);

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
                    InterpolatedPoints.InsertRange(i * Steps, InterpolateSegment(getControlPointsForSegment(i), Steps));

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
        public SplineModificationInfo deleteControlPoint(int index) {

            if ((ControlPoints.Count - 1) < 3) {
                Debug.LogError("Cannot remove more control points, minimum number is 3.");
                return new SplineModificationInfo(0,0,0);
            }

            if (index == (ControlPoints.Count - 1))
            {
                InterpolatedPoints.RemoveRange((index -1) * Steps, Steps);
            }
            else {
                InterpolatedPoints.RemoveRange(index * Steps, Steps);
            }

            ControlPoints.RemoveAt(index);

            //determine which segments have to be reinterpolated
            int start = (index - 2) >= 0 ? (index - 2) : 0;
            int end = (index +1) <= (ControlPoints.Count - 2) ? (index+1) : (ControlPoints.Count - 2);

            for (int i = start; i <= end; i++)
            {
                try
                {
                    InterpolatedPoints.RemoveRange(i * Steps, Steps);
                    InterpolatedPoints.InsertRange(i * Steps, InterpolateSegment(getControlPointsForSegment(i), Steps));
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
        public void setControlPoints(Vector3[] controlPoints) {
            ControlPoints.Clear();
            foreach (Vector3 controlPoint in controlPoints)
            {
                ControlPoints.Add(controlPoint);
            }
            InterpolateSpline();
        }

        public void setControlPoints(KochanekBartelsControlPoint[] controlPoints) {
            ControlPoints.Clear();
            ControlPoints.AddRange(controlPoints);
            InterpolateSpline();
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
        private static List<Vector3> InterpolateSegment(KochanekBartelsControlPoint point1, KochanekBartelsControlPoint point2, KochanekBartelsControlPoint point3, KochanekBartelsControlPoint point4, int steps)
        {
            var interpolatedPoints = new List<Vector3>();

            Vector3 point2DestinationTangent = point2.CalculateDestinationTangent(point1, point3);
            Vector3 point3SourceTangent = point3.CalculateSourceTangent(point2, point4);

            for (var i = 0; i < steps; i++)
            {
                var s = i / (float)(steps);

                float h1 = (float)(2 * Math.Pow(s, 3) - 3 * Math.Pow(s, 2) + 1);
                float h2 = (float)((-2) * Math.Pow(s, 3) + 3 * Math.Pow(s, 2));
                float h3 = (float)(Math.Pow(s, 3) - 2 * Math.Pow(s, 2) + s);
                float h4 = (float)(Math.Pow(s, 3) - Math.Pow(s, 2));

                var newPoint = h1 * point2.Position + h2 * point3.Position + h3 * point2DestinationTangent + h4 * point3SourceTangent;

                interpolatedPoints.Add(newPoint);
            }

            //interpolatedPoints.Add(point3.Position);

            return interpolatedPoints;
        }

        private static List<Vector3> InterpolateSegment(List<KochanekBartelsControlPoint> controlPoints, int steps) {
            if (controlPoints != null && controlPoints.Count == 4)
            {
                return InterpolateSegment(controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3], steps);
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
                Debug.LogError("Not enough control points! Minimum is 3.");
            }
            for (int i = 0; i < ControlPoints.Count-1; i++)
            {
                List<Vector3> points = InterpolateSegment(getControlPointsForSegment(i), Steps);
                if (points != null && points.Count > 0) {
                    InterpolatedPoints.AddRange(points);
                }
            }
        }

        private List<KochanekBartelsControlPoint> getControlPointsForSegment(int index) {
            int i = index;

            if (i >= ControlPoints.Count - 1) {
                Debug.LogError("Index out of range! Last segment is at number of control points minus one.");
                return null;
            }

            //first control point
            if (i == 0)
            {
                KochanekBartelsControlPoint point1 = ControlPoints[i];
                KochanekBartelsControlPoint point2 = ControlPoints[i];
                KochanekBartelsControlPoint point3 = ControlPoints[i + 1];
                KochanekBartelsControlPoint point4 = ControlPoints[i + 2];

                return new List<KochanekBartelsControlPoint>(new KochanekBartelsControlPoint[] { point1, point2, point3, point4 });
            }
            //middle control point
            else if (i + 2 < ControlPoints.Count)
            {
                KochanekBartelsControlPoint point1 = ControlPoints[i - 1];
                KochanekBartelsControlPoint point2 = ControlPoints[i];
                KochanekBartelsControlPoint point3 = ControlPoints[i + 1];
                KochanekBartelsControlPoint point4 = ControlPoints[i + 2];

                return new List<KochanekBartelsControlPoint>(new KochanekBartelsControlPoint[] { point1, point2, point3, point4 });
            }
            //last control point
            else if (i + 1 < ControlPoints.Count)
            {
                KochanekBartelsControlPoint point1 = ControlPoints[i - 1];
                KochanekBartelsControlPoint point2 = ControlPoints[i];
                KochanekBartelsControlPoint point3 = ControlPoints[i + 1];
                KochanekBartelsControlPoint point4 = ControlPoints[i + 1];

                return new List<KochanekBartelsControlPoint>(new KochanekBartelsControlPoint[] { point1, point2, point3, point4 });

            }
            else {
                return null;
            }
        }
    }
}