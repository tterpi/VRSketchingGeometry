using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement
{

    public class LinearInterpolationLineSketchObject : LineSketchObject
    {
        // Start is called before the first frame update
        protected override void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

            SplineMesh = new SplineMesh(new LinearInterpolationSpline(), Vector3.one * lineDiameter);

            meshCollider.sharedMesh = meshFilter.sharedMesh;
            setUpOriginalMaterialAndMeshRenderer();
        }

        public override void setLineDiameter(float diameter)
        {
            this.lineDiameter = diameter;

            if (SplineMesh == null) return;

            meshFilter.mesh = SplineMesh.SetCrossSectionScale(Vector3.one * diameter);

            sphereObject.transform.localScale = Vector3.one * diameter / sphereObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;

            chooseDisplayMethod();
        }

        public override void SetLineCrossSection(List<Vector3> crossSection, List<Vector3> crossSectionNormals, float diameter)
        {
            this.lineDiameter = diameter;

            if (SplineMesh == null)
            {
                return;
            }

            meshFilter.mesh = SplineMesh.SetCrossSection(crossSection, crossSectionNormals, diameter * Vector3.one);

            sphereObject.transform.localScale = Vector3.one * diameter / sphereObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;

            chooseDisplayMethod();
        }

        /// <summary>
        /// Determines how to display the spline depending on the number of control points that are present.
        /// </summary>
        protected override void chooseDisplayMethod()
        {
            sphereObject.SetActive(false);
            if (SplineMesh.getNumberOfControlPoints() == 0)
            {
                //display nothing
                meshFilter.mesh = new Mesh();
                //update collider
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
            else if (SplineMesh.getNumberOfControlPoints() == 1)
            {
                //display sphere if there is only one control point
                sphereObject.SetActive(true);
                sphereObject.transform.localPosition = SplineMesh.getControlPoints()[0];
                //update collider
                meshCollider.sharedMesh = null;
            }
            else if (SplineMesh.getNumberOfControlPoints() == 2)
            {
                //display linearly interpolated segment if there are two control points
                List<Vector3> controlPoints = SplineMesh.getControlPoints();
                //set the two control points
                meshFilter.mesh = SplineMesh.setControlPoints(controlPoints.ToArray());
                //update collider
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
            else
            {
                //display smoothly interpolated segments by not overwriting the previously generated mesh
                //update collider
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }

        }

        public override SerializableComponentData GetData()
        {
            LineSketchObjectData data = base.GetData() as LineSketchObjectData;
            data.Interpolation = LineSketchObjectData.InterpolationType.Linear;
            return data;
        }
    }
}