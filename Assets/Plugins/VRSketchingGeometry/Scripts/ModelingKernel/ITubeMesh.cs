using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Meshing
{
    public interface ITubeMesh
    {
        Vector3 CrossSectionScale { get; }
        Mesh GenerateMesh(List<Vector3> points);
        Mesh ReplacePoints(List<Vector3> points, int index, int addCount, int removeCount);
    }
}
