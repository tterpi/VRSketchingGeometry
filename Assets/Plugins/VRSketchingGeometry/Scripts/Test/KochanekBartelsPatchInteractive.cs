using VRSketchingGeometry.Meshing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochanekBartelsPatchInteractive : MonoBehaviour
{
    public int width = 4;
    public int height = 4;
    public int resolutionWidth = 4;
    public int resolutionHeight = 4;

    public List<GameObject> controlPointObjects;

    /// <summary>
    /// Generates the patch mesh and assigns it to the MeshFilter of this GameObject.
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <param name="width">Number of control points in x direction</param>
    /// <param name="height">Number of control points in y direction</param>
    public void UpdatePatchMesh(List<Vector3> controlPoints, int width, int height)
    {
        Mesh patchMesh = PatchMesh.GeneratePatchMesh(controlPoints, width, height, this.resolutionWidth, this.resolutionHeight);
        Mesh oldMesh = this.GetComponent<MeshFilter>().sharedMesh;
        Destroy(oldMesh);
        this.GetComponent<MeshFilter>().mesh = patchMesh;
    }

    private IEnumerator updatePatchMeshContinuously()
    {
        UpdatePatchMeshWithControlPointGameObjects();
        yield return new WaitForSeconds(.5f);
        StartCoroutine(nameof(updatePatchMeshContinuously));
    }

    private void UpdatePatchMeshWithControlPointGameObjects()
    {
        List<Vector3> controlPoints = new List<Vector3>();

        foreach (GameObject go in controlPointObjects)
        {
            controlPoints.Add(go.transform.position);
        }

        UpdatePatchMesh(controlPoints, width, height);
    }

    public void Start()
    {
        StartCoroutine(updatePatchMeshContinuously());
    }
}
