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

        public List<Vector3> GenerateControlPointsYDirection(int length)
        {
            List<Vector3> controlPoints = new List<Vector3>();
            for (int i = 0; i < length; i++)
            {
                controlPoints.Add(new Vector3(0, i, 0));
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
                this.LineSketchObject.SetControlPointsLocalSpace(controlPoints);
            })
            .Run();
        }

        [Test, Performance]
        public void SketchObject_AddControlPoint_Performance([Values(3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 40, 50)]int length)
        {
            List<Vector3> controlPoints = GenerateControlPoints(length-1);
            Measure.Method(() =>
            {
                this.LineSketchObject.AddControlPoint(new Vector3(length, 0, 0));
            })
            .SetUp(()=> {
                this.LineSketchObject.SetControlPointsLocalSpace(controlPoints);
            })
            .Run();
        }

        [Test, Performance]
        public void SplineMesh_AddControlPoint_Performance([NUnit.Framework.Range(9, 99, 10)]int length)
        {
            SplineMesh splineMesh = null;
            Measure.Method(() =>
            {
                splineMesh.AddControlPoint(new Vector3(length + 1, 0, 0));
            })
            .SetUp(() => {
                splineMesh = new SplineMesh(new KochanekBartelsSpline());
                splineMesh.SetControlPoints(GenerateControlPoints(length).ToArray());
            })
            .Run();
        }

        [Test, Performance]
        public void SketchObject_SetControlPoints_InterpolationSteps_Performance([Values(5, 6, 7, 8, 9, 10, 15, 20, 25)]int steps)
        {
            List<Vector3> controlPoints = GenerateControlPoints(10);
            Measure.Method(() =>
            {
                this.LineSketchObject.SetControlPointsLocalSpace(controlPoints);
            })
            .SetUp(() => {
                this.LineSketchObject.SetInterpolationSteps(steps);
            })
            .Run();
        }


        [UnityTest, Performance]
        public IEnumerator Framerate_LineSketchObjects([Values(5,10,20)]int steps, [Values(100, 500, 1000, 2000, 4000, 6000, 8000, 10000)]int count) {
            List<Vector3> controlPoints = GenerateControlPointsYDirection(3);
            this.LineSketchObject.SetInterpolationSteps(steps);
            this.LineSketchObject.SetControlPointsLocalSpace(controlPoints);
            for (int i = 1; i < count; i++)
            {
                LineSketchObject currentLine = GameObject.Instantiate(this.LineSketchObject).GetComponent<LineSketchObject>();
                currentLine.SetInterpolationSteps(steps);
                currentLine.SetControlPointsLocalSpace(controlPoints);
                currentLine.transform.position = new Vector3(i%50, 0, i/50);
            }
            yield return Measure.Frames().Run();
        }
    }
}
