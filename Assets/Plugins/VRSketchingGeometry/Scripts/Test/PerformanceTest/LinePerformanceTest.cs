using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Splines;
using UnityEngine.SceneManagement;
using Unity.PerformanceTesting;

namespace Tests
{
    public class LinePerformanceTest
    {
        private LineSketchObject LineSketchObject;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.LineSketchObject = GameObject.FindObjectOfType<LineSketchObject>();
            yield return null;
        }

        public List<Vector3> GenerateControlPoints(int length) {
            List<Vector3> controlPoints = new List<Vector3>();
            for (int i = 0; i < length; i++)
            {
               controlPoints.Add(new Vector3(i, i % 2, 0));
            }
            return controlPoints;
        }

        public List<Quaternion> GenerateQuaternions(int length) {
            List<Quaternion> quaternions = new List<Quaternion>();
            for (int i = 0; i < length; i++)
            {
                quaternions.Add(Quaternion.identity);
            }
            return quaternions;
        }

        [Test, Performance]
        public void SetControlPoints_Performance([NUnit.Framework.Range(10,100,10)]int length) {
            Measure.Method(() =>
            {
                this.LineSketchObject.SetControlPoints(GenerateControlPoints(length));
            }).Run();
        }

        [Test, Performance]
        public void SketchObject_AddControlPoint_Performance([NUnit.Framework.Range(9, 99, 10)]int length)
        {
            Measure.Method(() =>
            {
                this.LineSketchObject.addControlPoint(new Vector3(length + 1, 0, 0));
            })
            .SetUp(()=> {
                this.LineSketchObject.SetControlPoints(GenerateControlPoints(length));
            })
            .Run();
        }

        [Test, Performance]
        public void RibbonMesh_AddControlPoint_Performance([NUnit.Framework.Range(9, 99, 10)]int length)
        {
            SplineMesh splineMesh = null;
            Measure.Method(() =>
            {
                splineMesh.addControlPoint(new Vector3(length + 1, 0, 0));
            })
            .SetUp(() => {
                splineMesh = new SplineMesh(new KochanekBartelsSpline());
                splineMesh.setControlPoints(GenerateControlPoints(length).ToArray());
            })
            .Run();
        }
    }
}
