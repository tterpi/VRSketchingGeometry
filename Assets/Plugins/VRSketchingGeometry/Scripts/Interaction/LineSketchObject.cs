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
using VRSketchingGeometry.Serialization;
using System.Linq;

namespace VRSketchingGeometry.SketchObjectManagement
{
    /// <summary>
    /// Provides methods to interact with a line game object in the scene.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class LineSketchObject : SketchObject, ISerializableComponent, IBrushable
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
        protected SplineMesh LinearSplineMesh;

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
        private int InterpolationSteps = 20;

        protected override void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

            SplineMesh = new SplineMesh(new KochanekBartelsSpline(InterpolationSteps), Vector3.one * lineDiameter);
            LinearSplineMesh = new SplineMesh(new LinearInterpolationSpline(), Vector3.one * lineDiameter);

            meshCollider.sharedMesh = meshFilter.sharedMesh;
            setUpOriginalMaterialAndMeshRenderer();
        }

        /// <summary>
        /// Adds a control point to the end of the spline.
        /// </summary>
        /// <param name="point"></param>
        public void AddControlPoint(Vector3 point)
        {
            //Transform the new control point from world to local space of sketch object
            Vector3 transformedPoint = transform.InverseTransformPoint(point);
            meshFilter.mesh = SplineMesh.addControlPoint(transformedPoint);
            ChooseDisplayMethod();

        }

        /// <summary>
        /// Adds a control point to the spline if it is far enough away from the previous control point.
        /// The distance is controlled by minimumControlPointDistance.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>True if the control point was added.</returns>
        public bool AddControlPointContinuous(Vector3 point)
        {
            //Check that new control point is far enough away from previous control point
            if (
                SplineMesh.getNumberOfControlPoints() == 0 ||
                (transform.InverseTransformPoint(point) - SplineMesh.getControlPoints()[SplineMesh.getNumberOfControlPoints() - 1]).magnitude > minimumControlPointDistance
               )
            {
                AddControlPoint(point);
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Set all control points. Control points are expected to be in local space.
        /// </summary>
        /// <param name="controlPoints"></param>
        public void SetControlPointsLocalSpace(List<Vector3> controlPoints) {
            meshFilter.mesh = this.SplineMesh.setControlPoints(controlPoints.ToArray());
            ChooseDisplayMethod();
        }

        /// <summary>
        /// Set all control points. Control points are expected to be in world space.
        /// </summary>
        /// <param name="controlPoints"></param>
        public void SetControlPoints(List<Vector3> controlPoints) {
            List<Vector3> transformedControlPoints = controlPoints.Select((point) => this.transform.InverseTransformPoint(point)).ToList();
            SetControlPointsLocalSpace(transformedControlPoints);
        }

        public virtual void SetLineDiameter(float diameter)
        {
            this.lineDiameter = diameter;

            if (SplineMesh == null || LinearSplineMesh == null) {
                return;
            }

            Mesh smoothMesh = SplineMesh.SetCrossSectionScale(Vector3.one * diameter);
            Mesh linearMesh = LinearSplineMesh.SetCrossSectionScale(Vector3.one * diameter);

            meshFilter.mesh = smoothMesh ?? linearMesh;

            sphereObject.transform.localScale = Vector3.one * diameter / sphereObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;

            ChooseDisplayMethod();
        }

        public virtual void SetLineCrossSection(List<Vector3> crossSection, List<Vector3> crossSectionNormals, float diameter) {
            this.lineDiameter = diameter;

            if (SplineMesh == null || LinearSplineMesh == null)
            {
                return;
            }

            Mesh smoothMesh = SplineMesh.SetCrossSection(crossSection, crossSectionNormals, diameter * Vector3.one);
            Mesh linearMesh = LinearSplineMesh.SetCrossSection(crossSection, crossSectionNormals, diameter * Vector3.one);

            meshFilter.mesh = smoothMesh ?? linearMesh;

            sphereObject.transform.localScale = Vector3.one * diameter / sphereObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;

            ChooseDisplayMethod();
        }

        /// <summary>
        /// Set the number of interpolation steps between two control points.
        /// A higher number makes the line smoother.
        /// </summary>
        /// <param name="steps"></param>
        public void SetInterpolationSteps(int steps) {
            this.InterpolationSteps = steps;
            List<Vector3> controlPoints = this.GetControlPoints();
            this.SplineMesh.GetCrossSectionShape(out List<Vector3> CurrentCrossSectionShape, out List<Vector3> CurrentCrossSectionNormals);
            SplineMesh = new SplineMesh(new KochanekBartelsSpline(steps), this.lineDiameter * Vector3.one);
            this.SetLineCrossSection(CurrentCrossSectionShape, CurrentCrossSectionNormals, this.lineDiameter);
            if (controlPoints.Count != 0) {
                this.SetControlPointsLocalSpace(controlPoints);
            }
        }

        /// <summary>
        /// Deletes the last control point of the spline.
        /// </summary>
        public void DeleteControlPoint()
        {
            //delete the last control point of the spline
            meshFilter.mesh = SplineMesh.deleteControlPoint(SplineMesh.getNumberOfControlPoints() - 1);
            ChooseDisplayMethod();
        }

        /// <summary>
        /// Delete the control points of this line that are within the radius around point.
        /// </summary>
        /// <param name="point">Point in world space.</param>
        /// <param name="radius">Radius in world space.</param>
        /// <param name="newLineSketchObjects">List of new line sketch objects that were created for the deletion.</param>
        /// <returns>Returns true if at least one control point was deleted, false otherwise.</returns>
        public bool DeleteControlPoints(Vector3 point, float radius, out List<LineSketchObject> newLineSketchObjects) {
            List<List<Vector3>> contiguousSections = new List<List<Vector3>>();
            List<Vector3> contiguousSection = new List<Vector3>();

            //find contiguous sections of control points that are not in the radius
            foreach (Vector3 controlPoint in GetControlPoints()) {
                if (!IsInRadius(controlPoint, this.transform.InverseTransformPoint(point), radius / this.transform.lossyScale.x))
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

            newLineSketchObjects = new List<LineSketchObject>();

            //create lines from the sections
            if (contiguousSections.Count > 0)
            {
                if (contiguousSections.Count == 1 && contiguousSections[0].Count == this.getNumberOfControlPoints()) {
                    //if this is the case, no control points were deleted and the line stays unchanged
                    return false;
                }

                //this line becomes the first section
                this.SetControlPointsLocalSpace(contiguousSections[0]);
                contiguousSections.RemoveAt(0);

                //create new lines for each additional section
                foreach (List<Vector3> section in contiguousSections)
                {
                    LineSketchObject newLine = Instantiate(this, this.transform.parent);
                    newLine.SetControlPointsLocalSpace(section);
                    //newLine.setLineDiameter(this.lineDiameter);
                    this.SplineMesh.GetCrossSectionShape(out List<Vector3> crossSectionVertices, out List<Vector3> crossSectionNormals);

                    newLine.SetLineCrossSection(crossSectionVertices, crossSectionNormals, this.lineDiameter);
                    newLineSketchObjects.Add(newLine);
                }
            }
            else {
                if (SketchWorld.ActiveSketchWorld)
                {
                    SketchWorld.ActiveSketchWorld.DeleteObject(this);
                }
                else {
                    Destroy(this.gameObject);
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if gameObject is a LineSketchObject, then deletes control points of this LineSketchObject within radius
        /// </summary>
        /// <param name="gameObject">LineSketchObject of which control points should be deleted.</param>
        /// <param name="point">Point in world space.</param>
        /// <param name="radius">Radius in world space.</param>
        public static void DeleteControlPoints(GameObject gameObject, Vector3 point, float radius) {
            LineSketchObject line = gameObject.GetComponent<LineSketchObject>();

            //When there is only one control point, a sphere that is a child of the LineSketchObject is shown
            //This code checks if this child sphere of a LineSketchObject was collided with
            if (line == null && gameObject.GetComponent<SphereCollider>() && gameObject.transform.parent.GetComponent<LineSketchObject>())
            {
                line = gameObject.transform.parent.GetComponent<LineSketchObject>();
            }

            line?.DeleteControlPoints(point, radius, out List<LineSketchObject> newLines);
        }

        private static bool IsInRadius(Vector3 a, Vector3 b, float radius) {
            return (a - b).sqrMagnitude <= radius * radius;
        }

        public int getNumberOfControlPoints()
        {
            return SplineMesh.getNumberOfControlPoints();
        }

        /// <summary>
        /// Get the control points in local space.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetControlPoints() {
            return SplineMesh.getControlPoints();
        }

        /// <summary>
        /// Determines how to display the spline depending on the number of control points that are present.
        /// </summary>
        protected virtual void ChooseDisplayMethod()
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

        public virtual void RefineMesh() {
            meshFilter.mesh = this.SplineMesh.RefineMesh();
            ChooseDisplayMethod();
        }

        private void SetMaterial(Material material) {
            this.meshRenderer.sharedMaterial = material;
            this.sphereObject.GetComponent<MeshRenderer>().sharedMaterial = material;
            originalMaterial = this.meshRenderer.sharedMaterial;
        }

        public Brush GetBrush() {
            LineBrush brush = new LineBrush();
            brush.SketchMaterial = new SketchMaterialData(meshRenderer.sharedMaterial);
            brush.CrossSectionScale = this.lineDiameter;
            brush.InterpolationSteps = this.InterpolationSteps;
            SplineMesh.GetCrossSectionShape(out brush.CrossSectionVertices, out brush.CrossSectionNormals);
            return brush;
        }

        public void SetBrush(Brush brush) {
            this.SetMaterial(Defaults.GetMaterialFromDictionary(brush.SketchMaterial));
            if (brush is LineBrush lineBrush)
            {
                this.SetInterpolationSteps(lineBrush.InterpolationSteps);
                this.SetLineCrossSection(lineBrush.CrossSectionVertices, lineBrush.CrossSectionNormals, lineBrush.CrossSectionScale);
            }
        }

        public virtual SerializableComponentData GetData() {
            LineSketchObjectData data = new LineSketchObjectData
            {
                Interpolation = LineSketchObjectData.InterpolationType.Cubic,
                ControlPoints = GetControlPoints(),
                CrossSectionScale = this.lineDiameter,
                InterpolationSteps = this.InterpolationSteps
            };

            data.SetDataFromTransform(this.transform);

            SplineMesh.GetCrossSectionShape(out data.CrossSectionVertices,out data.CrossSectionNormals);

            data.SketchMaterial = new SketchMaterialData(this.meshRenderer.sharedMaterial);

            return data;
        }

        public void ApplyData(LineSketchObjectData data) {

            this.transform.position = Vector3.zero;
            this.transform.rotation = Quaternion.identity;
            this.SetInterpolationSteps(data.InterpolationSteps);
            this.SetLineCrossSection(data.CrossSectionVertices, data.CrossSectionNormals, data.CrossSectionScale);
            this.SetControlPointsLocalSpace(data.ControlPoints);
            data.ApplyDataToTransform(this.transform);

            this.SetMaterial(Defaults.GetMaterialFromDictionary(data.SketchMaterial));
        }

        public void ApplyData(SerializableComponentData data)
        {
            if (data is LineSketchObjectData lineSketchData)
            {
                this.ApplyData(lineSketchData);
            }
            else {
                Debug.LogError("Trying to deserialize object as LineSketchObject that is not a LineSketchObjectData object.");
            }
        }
    }
}