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
    /// A control point for the Kochanek Bartels spline class.
    /// Contains the position of the control point and the parameters tension, bias and continuity
    /// that control the shape of the spline at the control point.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class KochanekBartelsControlPoint {

        public KochanekBartelsControlPoint() : this(Vector3.zero) { }

        public Vector3 Position { get; set; }

        public float Tension { get; set; }
        public float Bias { get; set; }
        public float Continuity { get; set; }

        /// <summary>
        /// Implicitly convert a Vector3 object to a <see cref="KochanekBartelsControlPoint"/> object with default values for tension, bias and continuity.
        /// </summary>
        /// <param name="position"></param>
        public static implicit operator KochanekBartelsControlPoint(Vector3 position) => new KochanekBartelsControlPoint(position);

        /// <summary>
        /// Calculate the incoming tangent.
        /// </summary>
        /// <param name="previousControlPoint"></param>
        /// <param name="nextControlPoint"></param>
        /// <returns></returns>
        public Vector3 CalculateSourceTangent(KochanekBartelsControlPoint previousControlPoint, KochanekBartelsControlPoint nextControlPoint) {
            var factor1 = (1 - Tension) * (1 - Continuity) * (1 + Bias) / 2;
            var factor2 = (1 - Tension) * (1 + Continuity) * (1 - Bias) / 2;

            return factor1 * (Position - previousControlPoint.Position) + factor2 * (nextControlPoint.Position - Position);
        }

        /// <summary>
        /// Calculate the outgoing tangent.
        /// </summary>
        /// <param name="previousControlPoint"></param>
        /// <param name="nextControlPoint"></param>
        /// <returns></returns>
        public Vector3 CalculateDestinationTangent(KochanekBartelsControlPoint previousControlPoint, KochanekBartelsControlPoint nextControlPoint)
        {
            var factor1 = (1 - Tension) * (1 + Continuity) * (1 + Bias) / 2;
            var factor2 = (1 - Tension) * (1 - Continuity) * (1 - Bias) / 2;

            return factor1 * (Position - previousControlPoint.Position) + factor2 * (nextControlPoint.Position - Position);
        }

        /// <summary>
        /// Constructor for the KochanekBartelsSpline.
        /// </summary>
        /// <param name="position">Position of the control point.</param>
        /// <param name="tension">Controls how sharp or soft the curve is at the control point.</param>
        /// <param name="bias">Determines the influence of the previous and next control point on the tangent.</param>
        /// <param name="continuity">Determines the similarity between the source and destination tangent.</param>
        public KochanekBartelsControlPoint(Vector3 position, float tension = 0.0f, float bias = 0.0f, float continuity = 0.0f) {
            Position = position;
            Tension = tension;
            Bias = bias;
            Continuity = continuity;
        }
    }
}

