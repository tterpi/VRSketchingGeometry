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
        public void SetControlPoints5_Performance([Values(5,10,15)]int length) {
            Measure.Method(() =>
            {
                this.PatchSketchObject.SetControlPoints(GenerateControlPoints(4, length), 4, length);

            }).Run();
        }

        [Test, Performance]
        public void SetControlPointsBig100_Performance([Values(100,200,300)]int length)
        {
            Measure.Method(() =>
            {
                this.PatchSketchObject.SetControlPoints(GenerateControlPoints(4, length), 4, length);

            }).Run();
        }

        [Test, Performance]
        public void GeneratePatchVertices_Unoptimized() {
            Measure.Method(() =>
            {
                PatchMesh.GenerateVerticesOfPatch_Original(GenerateControlPoints(30, 30), 30, 30, 4,4);
            }).Run();
        }

        [Test, Performance]
        public void GeneratePatchVertices_Optimized()
        {
            Measure.Method(() =>
            {
                PatchMesh.GenerateVerticesOfPatch_Optimized(GenerateControlPoints(30, 30), 30, 30, 4, 4);
            }).Run();
        }

        [Test, Performance]
        public void GeneratePatchVertices_Parallel()
        {
            Measure.Method(() =>
            {
                PatchMesh.GenerateVerticesOfPatch_Parallel(GenerateControlPoints(30, 30), 30, 30, 4, 4);
            }).Run();
        }
    }
}
