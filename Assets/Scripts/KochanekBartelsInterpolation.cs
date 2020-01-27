using System;
using System.Collections.Generic;
using UnityEngine;

namespace KochanekBartelsSplines.Interpolation
{
    public class InterpolatedPointsCalculator
    {
        public static List<Vector3> GetInterpolatedPoints(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float tension, float continuity, float bias, int steps)
        {
            var interpolatedPoints = new List<Vector3>();

            var factor1 = (1 - tension) * (1 + continuity) * (1 + bias) / 2;
            var factor2 = (1 - tension) * (1 - continuity) * (1 - bias) / 2;

            var tangentDestinationI = factor1 * (point2 - point1) + factor2 * (point3 - point2);

            var factor3 = (1 - tension) * (1 - continuity) * (1 + bias) / 2;
            var factor4 = (1 - tension) * (1 + continuity) * (1 - bias) / 2;

            var tangentSourceIPlusOne = factor3 * (point3 - point2) + factor4 * (point4 - point3);

            for (var i = 0; i < steps-1; i++)
            {
                var s = i / (float)(steps-1);

                float h1 = (float)(2 * Math.Pow(s, 3) - 3 * Math.Pow(s, 2) + 1);
                float h2 = (float) ((-2) * Math.Pow(s, 3) + 3 * Math.Pow(s, 2));
                float h3 = (float)(Math.Pow(s, 3) - 2 * Math.Pow(s, 2) + s);
                float h4 = (float)(Math.Pow(s, 3) - Math.Pow(s, 2));

                var newPoint = h1 * point2 + h2 * point3 + h3 * tangentDestinationI + h4 * tangentSourceIPlusOne;

                interpolatedPoints.Add(newPoint);
            }

            interpolatedPoints.Add(point3);

            return interpolatedPoints;
        }
    }
}