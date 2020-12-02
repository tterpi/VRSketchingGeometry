//-----------------------------------------------------------------------
//
// Original repository: https://github.com/tterpi/VRSketchingGeometry
//
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;
using VRSketchingGeometry.Meshing;

namespace VRSketchingGeometry.SketchObjectManagement
{
    /// <summary>
    /// Provides methods to interact with a line game object in the scene.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class LineSketchObject : SketchObject
    {
        /// <summary>
        /// The instance of the smoothly interpolated Catmul-Rom spline mesh
        /// </summary>
        protected SplineMesh SplineMesh;

        /// <summary>
        /// Mesh filter for the mesh of the spline
        /// </summary>
        protected MeshFilter meshFilter;

        /// <summary>
        /// Collider for the mesh of the spline
        /// </summary>
        protected MeshCollider meshCollider;

        /// <summary>
        /// Linierly interpolated spline for displaying a segment only two control points
        /// </summary>
        private SplineMesh LinearSplineMesh;

        /// <summary>
        /// Object to be displayed for a line of a single control point
        /// </summary>
        [SerializeField]
#pragma warning disable CS0649
        protected GameObject sphereObject;
#pragma warning restore CS0649

        /// <summary>
        /// The minimal distance a new control point has to have to the last control point.
        /// This is used by addControlPointContinuous.
        /// </summary>
        public float minimumControlPointDistance = 1f;

        protected float lineDiameter = .2f;

        // Start is called before the first frame update
        protected virtual void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

            SplineMesh = new SplineMesh(new KochanekBartelsSpline(), Vector3.one * lineDiameter);
            LinearSplineMesh = new SplineMesh(new LinearInterpolationSpline(), Vector3.one * lineDiameter);

            meshCollider.sharedMesh = meshFilter.sharedMesh;
            setUpOriginalMaterialAndMeshRenderer();
        }

        /// <summary>
        /// Adds a control point to the end of the spline.
        /// </summary>
        /// <param name="point"></param>
        public void addControlPoint(Vector3 point)
        {
            //Transform the new control point from world to local space of sketch object
            Vector3 transformedPoint = transform.InverseTransformPoint(point);
            meshFilter.mesh = SplineMesh.addControlPoint(transformedPoint);
            chooseDisplayMethod();

        }

        /// <summary>
        /// Adds a control point to the spline if it is far enough away from the previous control point.
        /// The distance is controlled by minimumControlPointDistance.
        /// </summary>
        /// <param name="point"></param>
        public void addControlPointContinuous(Vector3 point)
        {
            //Check that new control point is far enough away from previous control point
            if (
                SplineMesh.getNumberOfControlPoints() == 0 ||
                (transform.InverseTransformPoint(point) - SplineMesh.getControlPoints()[SplineMesh.getNumberOfControlPoints() - 1]).magnitude > minimumControlPointDistance
               )
            {
                addControlPoint(point);
            }
        }

        public void SetControlPoints(List<Vector3> controlPoints) {
            meshFilter.mesh = this.SplineMesh.setControlPoints(controlPoints.ToArray());
            chooseDisplayMethod();
        }

        public virtual void setLineDiameter(float diameter)
        {
            this.lineDiameter = diameter;

            if (SplineMesh == null || LinearSplineMesh == null) {
                return;
            }

            Mesh smoothMesh = SplineMesh.setCrossSectionScale(Vector3.one * diameter);
            Mesh linearMesh = LinearSplineMesh.setCrossSectionScale(Vector3.one * diameter);

            meshFilter.mesh = smoothMesh ?? linearMesh;

            sphereObject.transform.localScale = Vector3.one * diameter / sphereObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;

            chooseDisplayMethod();
        }

        /// <summary>
        /// Deletes the last control point of the spline.
        /// </summary>
        public void deleteControlPoint()
        {
            //delete the last control point of the spline
            meshFilter.mesh = SplineMesh.deleteControlPoint(SplineMesh.getNumberOfControlPoints() - 1);
            chooseDisplayMethod();
        }

        /// <summary>
        /// Delete the control points of this line that are within the radius around point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        public void DeleteControlPoints(Vector3 point, float radius) {
            List<List<Vector3>> contiguousSections = new List<List<Vector3>>();
            List<Vector3> contiguousSection = new List<Vector3>();

            //find contiguous sections of control points that are not in the radius
            foreach (Vector3 controlPoint in getControlPoints()) {
                if (!IsInRadius(controlPoint, this.transform.InverseTransformPoint(point), radius * this.transform.lossyScale.x))
                {
                    contiguousSection.Add(controlPoint);
                }
                else {
                    if (contiguousSection.Count > 0) {
                        contiguousSections.Add(contiguousSection);
                        contiguousSection = new List<Vector3>();
                    }
                }
            }
            if (contiguousSection.Count > 0)
            {
                contiguousSections.Add(contiguousSection);
            }

            //create lines from the sections
            if (contiguousSections.Count > 0)
            {
                //this line becomes the first section
                this.SetControlPoints(contiguousSections[0]);
                contiguousSections.RemoveAt(0);

                //create new lines for each additional section
                foreach (List<Vector3> section in contiguousSections)
                {
                    LineSketchObject newLine = Instantiate(this, this.transform.parent);
                    newLine.SetControlPoints(section);
                    newLine.setLineDiameter(this.lineDiameter);
                }
            }
            else {
                SketchWorld.ActiveSketchWorld.DeleteObject(this.gameObject);
            }

        }

        private bool IsInRadius(Vector3 a, Vector3 b, float radius) {
            return (a - b).sqrMagnitude <= radius * radius;
        }

        public int getNumberOfControlPoints()
        {
            return SplineMesh.getNumberOfControlPoints();
        }

        public List<Vector3> getControlPoints() {
            return SplineMesh.getControlPoints();
        }

        /// <summary>
        /// Determines how to display the spline depending on the number of control points that are present.
        /// </summary>
        protected virtual void chooseDisplayMethod()
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
                meshCollider.sharedMesh = sphereObject.GetComponent<MeshFilter>().sharedMesh;
            }
            else if (SplineMesh.getNumberOfControlPoints() == 2)
            {
                //display linearly interpolated segment if there are two control points
                List<Vector3> controlPoints = SplineMesh.getControlPoints();
                //set the two control points
                meshFilter.mesh = LinearSplineMesh.setControlPoints(controlPoints.ToArray());
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
    }
}