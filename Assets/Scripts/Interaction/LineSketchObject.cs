//-----------------------------------------------------------------------
//
// Original repository: https://github.com/tterpi/VRSketchingGeometry
//
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Splines;

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

    private MeshRenderer meshRenderer;

    private Material originalMaterial;

    [SerializeField]
    private Material highlightMaterial;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        SplineMesh = new SplineMesh(new KochanekBartelsSpline());
        LinearSplineMesh = new SplineMesh(new LinearInterpolationSpline());

        meshCollider.sharedMesh = meshFilter.sharedMesh;
        setUpOriginalMaterialAndMeshRenderer();
    }

    /// <summary>
    /// Adds a control point to the end of the spline.
    /// </summary>
    /// <param name="point"></param>
    public void addControlPoint(Vector3 point) {
        //Transform the new control point from world to local space of sketch object
        Vector3 transformedPoint = transform.InverseTransformPoint(point);
        meshFilter.mesh = SplineMesh.addControlPoint(transformedPoint);
        chooseDisplayMethod();
        
    }

    /// <summary>
    /// Adds a control point to the spline if it is far enough away from the previous control point.
    /// </summary>
    /// <param name="point"></param>
    public void addControlPointContinuous(Vector3 point) {
        //Check that new control point is far enough away from previous control point
        if (
            SplineMesh.getNumberOfControlPoints() == 0 || 
            (transform.InverseTransformPoint(point) - SplineMesh.getControlPoints()[SplineMesh.getNumberOfControlPoints() - 1]).magnitude > 1f
           ) 
        {
            addControlPoint(point);
        }
    }

    public virtual void setLineDiameter(float diameter) {
        meshFilter.mesh = SplineMesh.setCrossSectionScale(Vector3.one * diameter) ?? LinearSplineMesh.setCrossSectionScale(Vector3.one * diameter);
        

        sphereObject.transform.localScale = Vector3.one * diameter / sphereObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;

        chooseDisplayMethod();
    }

    /// <summary>
    /// Deletes the last control point of the spline.
    /// </summary>
    public void deleteControlPoint() {
        //delete the last control point of the spline
        meshFilter.mesh = SplineMesh.deleteControlPoint(SplineMesh.getNumberOfControlPoints() - 1);
        chooseDisplayMethod();
    }

    public int getNumberOfControlPoints() {
        return SplineMesh.getNumberOfControlPoints();
    }

    /// <summary>
    /// Determines how to display the spline depending on the number of control points that are present.
    /// </summary>
    protected virtual void chooseDisplayMethod() {
        sphereObject.SetActive(false);
        if (SplineMesh.getNumberOfControlPoints() == 0) {
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

    protected void setUpOriginalMaterialAndMeshRenderer() {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.sharedMaterial;
    }

    public override void highlight()
    {
        meshRenderer.sharedMaterial = highlightMaterial;
    }

    public override void revertHighlight()
    {
        meshRenderer.sharedMaterial = originalMaterial;
    }
}
