using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Meshing;
using UnityEngine.SceneManagement;
using Unity.PerformanceTesting;

namespace Tests
{
    public class RibbonPerformanceTest
    {
        private RibbonSketchObject RibbonSketchObject;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.RibbonSketchObject = GameObject.FindObjectOfType<RibbonSketchObject>();
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
        public void SetControlPoints_Performance([Values(3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 40, 50)]int length) {
            List<Vector3> points = GenerateControlPoints(length);
            List<Quaternion> rotations = GenerateQuaternions(length);
            Measure.Method(() =>
            {
                this.RibbonSketchObject.SetControlPoints(points, rotations);
            })
            .Run();
        }

        [Test, Performance]
        public void SketchObject_AddControlPoint_Performance([Values(3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 40, 50)]int length)
        {
            List<Vector3> points = GenerateControlPoints(length-1);
            List<Quaternion> rotations = GenerateQuaternions(length-1);
            Measure.Method(() =>
            {
                this.RibbonSketchObject.AddControlPoint(new Vector3(length + 1, 0, 0), Quaternion.identity);
            })
            .SetUp(()=> {
                this.RibbonSketchObject.SetControlPoints(points, rotations);
            })
            .Run();
        }

        [Test, Performance]
        public void RibbonMesh_AddControlPoint_Performance([NUnit.Framework.Range(9, 99, 10)]int length)
        {
            RibbonMesh ribbonMesh = null;
            Measure.Method(() =>
            {
                ribbonMesh.AddPoint(new Vector3(length + 1, 0, 0), Quaternion.identity);
            })
            .SetUp(() => {
                ribbonMesh = new RibbonMesh(Vector3.one);
                ribbonMesh.GetMesh(GenerateControlPoints(length), GenerateQuaternions(length));
            })
            .Run();
        }
    }
}
