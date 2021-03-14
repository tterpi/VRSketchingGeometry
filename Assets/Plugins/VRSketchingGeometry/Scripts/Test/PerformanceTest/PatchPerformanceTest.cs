using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Commands.Patch;
using VRSketchingGeometry.Serialization;
using UnityEngine.SceneManagement;
using Unity.PerformanceTesting;

namespace Tests
{
    public class PatchPerformanceTest
    {
        private PatchSketchObject PatchSketchObject;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.PatchSketchObject = GameObject.FindObjectOfType<PatchSketchObject>();
            yield return null;
        }

        public List<Vector3> GenerateControlPoints(int width, int height) {
            List<Vector3> controlPoints = new List<Vector3>();
            for (int i = 0; i < height; i++)
            {
                for (int y = 0; y < width; y++)
                {
                    controlPoints.Add(new Vector3(y, i, y%2));
                }
            }
            return controlPoints;
        }

        [Test, Performance]
        public void PatchPerformanceTestSimplePasses2()
        {
            List<Vector3> segment1 = new List<Vector3> { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0), new Vector3(3, 0, 0) };
            List<Vector3> segment2 = new List<Vector3> { new Vector3(0, 0, 1), new Vector3(1, 1, 1), new Vector3(2, 0, 1), new Vector3(3, 0, 1) };
            List<Vector3> segment3 = new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 0, 2), new Vector3(2, 1, 2), new Vector3(3, 0, 2) };
            List<Vector3> segment4 = new List<Vector3> { new Vector3(0, 1, 3), new Vector3(1, 0, 3), new Vector3(2, 0, 3), new Vector3(3, 0, 3) };

            Measure.Method(() =>
            {
                this.PatchSketchObject.AddPatchSegment(segment4);
            })
            .MeasurementCount(20)
            .SetUp(() => {
                this.PatchSketchObject.SetControlPoints(new List<Vector3>(), 0,0);
                this.PatchSketchObject.Width = 4;
                this.PatchSketchObject.AddPatchSegment(segment1);
                this.PatchSketchObject.AddPatchSegment(segment2);
                this.PatchSketchObject.AddPatchSegment(segment3);
            })
            .Run();
        }

        [Test, Performance]
        public void AddSegment_Performance([Values(3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 40, 50)]int length)
        {
            List<Vector3> controlPoints = GenerateControlPoints(4, length);
            List<Vector3> lastControlPoints = controlPoints.GetRange(controlPoints.Count - 4, 4);
            controlPoints.RemoveRange(controlPoints.Count - 4, 4);

            Measure.Method(() =>
            {
                this.PatchSketchObject.AddPatchSegment(lastControlPoints);
            })
            .SetUp(() => {
                this.PatchSketchObject.SetControlPoints(controlPoints, 4, length-1);
            })
            .Run();
        }

        [Test, Performance]
        public void SetControlPoints_Performance([Values(3,4,5,6,7,8,9,10,15,20,25,30,40,50)]int length) {
            List<Vector3> controlPoints = GenerateControlPoints(4, length);
            Measure.Method(() =>
            {
                this.PatchSketchObject.SetControlPoints(controlPoints, 4, length);

            })
            .Run();
        }

        [Test, Performance]
        public void SetControlPointsBig_Performance([Values(100,200,300)]int length)
        {
            Measure.Method(() =>
            {
                this.PatchSketchObject.SetControlPoints(GenerateControlPoints(4, length), 4, length);

            }).Run();
        }

        [Test, Performance]
        public void GeneratePatchVertices_Unoptimized([Values(10,50, 100)]int length) {
            List<Vector3> controlPoints = GenerateControlPoints(4, length);
            Measure.Method(() =>
            {
                PatchMesh.GenerateVerticesOfPatch_Original(controlPoints, 4, length, 4,4);
            }).Run();
        }

        [Test, Performance]
        public void GeneratePatchVerticesSmall_Unoptimized([NUnit.Framework.Range(3, 10)]int length)
        {
            List<Vector3> controlPoints = GenerateControlPoints(4, length);
            Measure.Method(() =>
            {
                PatchMesh.GenerateVerticesOfPatch_Original(controlPoints, 4, length, 4, 4);
            }).Run();
        }

        [Test, Performance]
        public void GeneratePatchVertices_Optimized([Values(10,50, 100)]int length)
        {
            List<Vector3> controlPoints = GenerateControlPoints(4, length);
            Measure.Method(() =>
            {
                PatchMesh.GenerateVerticesOfPatch_Optimized(controlPoints, 4, length, 4, 4);
            }).Run();
        }

        [Test, Performance]
        public void GeneratePatchVerticesSmall_Optimized([NUnit.Framework.Range(3, 10)]int length)
        {
            List<Vector3> controlPoints = GenerateControlPoints(4, length);
            Measure.Method(() =>
            {
                PatchMesh.GenerateVerticesOfPatch_Optimized(controlPoints, 4, length, 4, 4);
            }).Run();
        }

        [Test, Performance]
        public void GeneratePatchVertices_Parallel([Values(10,50,100)]int length)
        {
            List<Vector3> controlPoints = GenerateControlPoints(4, length);
            Measure.Method(() =>
            {
                PatchMesh.GenerateVerticesOfPatch_Parallel(controlPoints, 4, length, 4, 4);
            }).Run();
        }

        [Test, Performance]
        public void GeneratePatchVerticesSmall_Parallel([NUnit.Framework.Range(3,10)]int length)
        {
            List<Vector3> controlPoints = GenerateControlPoints(4, length);
            Measure.Method(() =>
            {
                PatchMesh.GenerateVerticesOfPatch_Parallel(controlPoints, 4, length, 4, 4);
            }).Run();
        }
    }
}
