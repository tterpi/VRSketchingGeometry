using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Meshing
{
    public interface ITubeMesh
    {
        Mesh GenerateMesh(List<Vector3> points);
        Mesh ReplacePoints(List<Vector3> points, int index, int addCount, int removeCount);
        Mesh SetCrossSection(List<Vector3> points, CrossSection crossSection);
        CrossSection GetCrossSection();
    }
}
