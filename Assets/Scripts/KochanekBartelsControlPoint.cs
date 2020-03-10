using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Splines {
    public class KochanekBartelsControlPoint {
        public Vector3 Position { get; set; }

        public float Tension { get; set; }
        public float Bias { get; set; }
        public float Continuity { get; set; }

        public static implicit operator KochanekBartelsControlPoint(Vector3 position) => new KochanekBartelsControlPoint(position);

        public Vector3 CalculateSourceTangent(KochanekBartelsControlPoint previousControlPoint, KochanekBartelsControlPoint nextControlPoint) {
            var factor1 = (1 - Tension) * (1 - Continuity) * (1 + Bias) / 2;
            var factor2 = (1 - Tension) * (1 + Continuity) * (1 - Bias) / 2;

            return factor1 * (Position - previousControlPoint.Position) + factor2 * (nextControlPoint.Position - Position);
        }

        public Vector3 CalculateDestinationTangent(KochanekBartelsControlPoint previousControlPoint, KochanekBartelsControlPoint nextControlPoint)
        {
            var factor1 = (1 - Tension) * (1 + Continuity) * (1 + Bias) / 2;
            var factor2 = (1 - Tension) * (1 - Continuity) * (1 - Bias) / 2;

            return factor1 * (Position - previousControlPoint.Position) + factor2 * (nextControlPoint.Position - Position);
        }

        public KochanekBartelsControlPoint(Vector3 position, float tension, float bias, float continuity) {
            Position = position;
            Tension = tension;
            Bias = bias;
            Continuity = continuity;
        }

        public KochanekBartelsControlPoint(Vector3 position) {
            Position = position;
            Tension = 0;
            Bias = 0;
            Continuity = 0;
        }
    }
}

