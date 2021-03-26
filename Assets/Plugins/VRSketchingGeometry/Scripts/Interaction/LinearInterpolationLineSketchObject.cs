using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement
{
    /// <summary>
    /// A line sketch object with no smooth interpolation between control point.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class LinearInterpolationLineSketchObject : LineSketchObject, ISerializableComponent
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

        public override void SetLineDiameter(float diameter)
        {
            this.lineDiameter = diameter;

            if (SplineMesh == null) return;

            meshFilter.mesh = SplineMesh.SetCrossSectionScale(Vector3.one * diameter);

            sphereObject.transform.localScale = Vector3.one * diameter / sphereObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;

            ChooseDisplayMethod();
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

            ChooseDisplayMethod();
        }

        /// <remarks>
        /// This method is not supported in this class. This means that this inheritance is probaly not ideal.
        /// </remarks>
        /// <param name="steps"></param>
        public override void SetInterpolationSteps(int steps) {
            Debug.LogWarning("Interpolation steps are not supported by linear interpolation line sketch object!");
        }

        /// <summary>
        /// Determines how to display the spline depending on the number of control points that are present.
        /// </summary>
        protected override void ChooseDisplayMethod()
        {
            sphereObject.SetActive(false);
            if (SplineMesh.GetNumberOfControlPoints() == 0)
            {
                //display nothing
                meshFilter.mesh = new Mesh();
                //update collider
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
            else if (SplineMesh.GetNumberOfControlPoints() == 1)
            {
                //display sphere if there is only one control point
                sphereObject.SetActive(true);
                sphereObject.transform.localPosition = SplineMesh.GetControlPoints()[0];
                //update collider
                meshCollider.sharedMesh = null;
            }
            else if (SplineMesh.GetNumberOfControlPoints() == 2)
            {
                //display linearly interpolated segment if there are two control points
                List<Vector3> controlPoints = SplineMesh.GetControlPoints();
                //set the two control points
                meshFilter.mesh = SplineMesh.SetControlPoints(controlPoints.ToArray());
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

        SerializableComponentData ISerializableComponent.GetData()
        {
            LineSketchObjectData data = base.GetData();
            data.Interpolation = LineSketchObjectData.InterpolationType.Linear;
            return data;
        }
    }
}