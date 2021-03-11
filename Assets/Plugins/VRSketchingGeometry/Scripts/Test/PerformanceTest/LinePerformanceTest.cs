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
        public void SketchObject_SetControlPoints_Performance([Values(3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 40, 50)]int length) {
            List<Vector3> controlPoints = GenerateControlPoints(length);
            Measure.Method(() =>
            {
                this.LineSketchObject.SetControlPoints(controlPoints);
            })
            .WarmupCount(10)
            .MeasurementCount(50)
            .Run();
        }

        [Test, Performance]
        public void SketchObject_AddControlPoint_Performance([Values(3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 40, 50)]int length)
        {
            List<Vector3> controlPoints = GenerateControlPoints(length-1);
            Measure.Method(() =>
            {
                this.LineSketchObject.addControlPoint(new Vector3(length, 0, 0));
            })
            .WarmupCount(10)
            .MeasurementCount(50)
            .SetUp(()=> {
                this.LineSketchObject.SetControlPoints(controlPoints);
            })
            .Run();
        }

        [Test, Performance]
        public void SplineMesh_AddControlPoint_Performance([NUnit.Framework.Range(9, 99, 10)]int length)
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
