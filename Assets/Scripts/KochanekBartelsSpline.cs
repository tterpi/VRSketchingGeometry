using System;
using System.Collections.Generic;
using UnityEngine;

namespace KochanekBartelsSplines
{
    public class KochanekBartelsSpline
    {
        private List<KochanekBartelsControlPoint> ControlPoints { get; set; }
        public List<Vector3> InterpolatedPoints { get; private set; }
        private int Steps;
        //private List<Vector3> InterpolatedTangents;

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
        public void setControlPoint(int index, Vector3 controlPoints) {
            throw new NotImplementedException();
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
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                InterpolateSegmentAtIndex(i);
                ////first control point
                //if (i == 0)
                //{
                //    KochanekBartelsControlPoint point1 = ControlPoints[i];
                //    KochanekBartelsControlPoint point2 = ControlPoints[i];
                //    KochanekBartelsControlPoint point3 = ControlPoints[i + 1];
                //    KochanekBartelsControlPoint point4 = ControlPoints[i + 2];

                //    InterpolatedPoints.AddRange(KochanekBartelsSpline.InterpolateSegment(point1, point2, point3, point4, Steps));
                //}
                ////middle control point
                //else if (i + 2 < ControlPoints.Count)
                //{
                //    KochanekBartelsControlPoint point1 = ControlPoints[i - 1];
                //    KochanekBartelsControlPoint point2 = ControlPoints[i];
                //    KochanekBartelsControlPoint point3 = ControlPoints[i + 1];
                //    KochanekBartelsControlPoint point4 = ControlPoints[i + 2];

                //    InterpolatedPoints.AddRange(KochanekBartelsSpline.InterpolateSegment(point1, point2, point3, point4, Steps));
                //}
                ////last control point
                //else if (i + 1 < ControlPoints.Count)
                //{
                //    KochanekBartelsControlPoint point1 = ControlPoints[i - 1];
                //    KochanekBartelsControlPoint point2 = ControlPoints[i];
                //    KochanekBartelsControlPoint point3 = ControlPoints[i + 1];
                //    KochanekBartelsControlPoint point4 = ControlPoints[i + 1];

                //    InterpolatedPoints.AddRange(KochanekBartelsSpline.InterpolateSegment(point1, point2, point3, point4, Steps));
                //    InterpolatedPoints.Add(point3.Position);
                //}
            }
        }

        private void InterpolateSegmentAtIndex(int index) {
            int i = index;

            if (i > ControlPoints.Count - 1) {
                Debug.LogError("Index out of range! Last segment is at number of control points minus one.");
            }

            //first control point
            if (i == 0)
            {
                KochanekBartelsControlPoint point1 = ControlPoints[i];
                KochanekBartelsControlPoint point2 = ControlPoints[i];
                KochanekBartelsControlPoint point3 = ControlPoints[i + 1];
                KochanekBartelsControlPoint point4 = ControlPoints[i + 2];

                try
                {
                    InterpolatedPoints.RemoveRange(i * Steps, Steps);
                    InterpolatedPoints.InsertRange(i * Steps, KochanekBartelsSpline.InterpolateSegment(point1, point2, point3, point4, Steps));
                }
                catch (ArgumentException exception)
                {
                    InterpolatedPoints.AddRange(KochanekBartelsSpline.InterpolateSegment(point1, point2, point3, point4, Steps));
                }
            }
            //middle control point
            else if (i + 2 < ControlPoints.Count)
            {
                KochanekBartelsControlPoint point1 = ControlPoints[i - 1];
                KochanekBartelsControlPoint point2 = ControlPoints[i];
                KochanekBartelsControlPoint point3 = ControlPoints[i + 1];
                KochanekBartelsControlPoint point4 = ControlPoints[i + 2];

                try
                {
                    InterpolatedPoints.RemoveRange(i * Steps, Steps);
                    InterpolatedPoints.InsertRange(i * Steps, KochanekBartelsSpline.InterpolateSegment(point1, point2, point3, point4, Steps));
                }
                catch (ArgumentException exception)
                {
                    InterpolatedPoints.AddRange(KochanekBartelsSpline.InterpolateSegment(point1, point2, point3, point4, Steps));
                }
            }
            //last control point
            else if (i + 1 < ControlPoints.Count)
            {
                KochanekBartelsControlPoint point1 = ControlPoints[i - 1];
                KochanekBartelsControlPoint point2 = ControlPoints[i];
                KochanekBartelsControlPoint point3 = ControlPoints[i + 1];
                KochanekBartelsControlPoint point4 = ControlPoints[i + 1];

                try
                {
                    InterpolatedPoints.RemoveRange(i * Steps, Steps);
                    InterpolatedPoints.InsertRange(i * Steps, KochanekBartelsSpline.InterpolateSegment(point1, point2, point3, point4, Steps));
                    InterpolatedPoints[(i + 1) * Steps] = point3.Position;
                }
                catch (ArgumentException exception) {
                    InterpolatedPoints.AddRange(KochanekBartelsSpline.InterpolateSegment(point1, point2, point3, point4, Steps));
                    InterpolatedPoints.Add(point3.Position);
                }
                
            }

        }
    }
}