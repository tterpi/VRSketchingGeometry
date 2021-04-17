using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Serialization;
using VRSketchingGeometry.Meshing;

public class LineSketchObjectTest : MonoBehaviour
{
    public GameObject selectionPrefab;
    public GameObject LineSketchObjectPrefab;
    private LineSketchObject lineSketchObject;
    private LineSketchObject lineSketchObject2;
    private PatchSketchObject patchSketchObject;
    private RibbonSketchObject ribbonSketchObject;

    public DefaultReferences defaults;

    public SketchWorld SketchWorld;
    public SketchWorld SketchWorld2;

    public Material ropeMaterial;
    public Material twoSidedMaterial;

    public GameObject ControlPointParent;

    private bool ranOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        lineSketchObject = Instantiate(LineSketchObjectPrefab).GetComponent<LineSketchObject>();
        lineSketchObject.SetLineDiameter(.5f);
        //lineSketchObject2 = Instantiate(LineSketchObjectPrefab).GetComponent<LineSketchObject>();
        lineSketchObject2 = Instantiate(defaults.LinearInterpolationLineSketchObjectPrefab).GetComponent<LineSketchObject>();
        patchSketchObject = Instantiate(defaults.PatchSketchObjectPrefab).GetComponent<PatchSketchObject>();
        ribbonSketchObject = Instantiate(defaults.RibbonSketchObjectPrefab).GetComponent<RibbonSketchObject>();
    }

    IEnumerator changeDiameter() {
        yield return new WaitForSeconds(5);
        lineSketchObject.SetLineDiameter(.1f);
        yield return new WaitForSeconds(2);
        lineSketchObject.DeleteControlPoint();
        lineSketchObject.DeleteControlPoint();

    }

    IEnumerator deactivateSelection(SketchObjectSelection selection) {
        yield return new WaitForSeconds(3);
        selection.Deactivate();
    }

    private void lineSketchObjectTest() {
        lineSketchObject.AddControlPoint(new Vector3(-2, 1, 0));
        lineSketchObject.AddControlPoint(Vector3.one);
        lineSketchObject.AddControlPoint(new Vector3(2, 2, 0));
        lineSketchObject.AddControlPoint(new Vector3(2, 1, 0));

        //lineSketchObject.setLineDiameter(.7f);
        StartCoroutine(changeDiameter());

        lineSketchObject2.AddControlPoint(new Vector3(1,0,0));
        lineSketchObject2.AddControlPoint(new Vector3(2, 1, 1));
        lineSketchObject2.AddControlPoint(new Vector3(3, 2, 0));
        lineSketchObject2.minimumControlPointDistance = 2f;
        lineSketchObject2.AddControlPointContinuous(new Vector3(3, 1, 0));

        //GameObject selectionGO = new GameObject("sketchObjectSelection", typeof(SketchObjectSelection));
        GameObject selectionGO = Instantiate(selectionPrefab);
        GameObject groupGO = new GameObject("sketchObjectGroup", typeof(SketchObjectGroup));
        SketchObjectSelection selection = selectionGO.GetComponent<SketchObjectSelection>();
        selection.AddToSelection(lineSketchObject);
        selection.AddToSelection(lineSketchObject2);
        selection.Activate();
        StartCoroutine(deactivateSelection(selection));
    }

    private void groupSerializationTest()
    {
        lineSketchObject.AddControlPoint(new Vector3(-2, 1, 0));
        lineSketchObject.AddControlPoint(Vector3.one);
        lineSketchObject.AddControlPoint(new Vector3(2, 2, 0));
        lineSketchObject.AddControlPoint(new Vector3(2, 1, 0));

        //lineSketchObject.setLineDiameter(.7f);
        //StartCoroutine(changeDiameter());

        lineSketchObject2.AddControlPoint(new Vector3(1, 0, 0));
        lineSketchObject2.AddControlPoint(new Vector3(2, 1, 1));
        lineSketchObject2.AddControlPoint(new Vector3(3, 2, 0));
        //lineSketchObject2.minimumControlPointDistance = 2f;
        //lineSketchObject2.addControlPointContinuous(new Vector3(3, 1, 0));
        GameObject groupGO = new GameObject("sketchObjectGroup", typeof(SketchObjectGroup));
        SketchObjectGroup group = groupGO.GetComponent<SketchObjectGroup>();
        group.AddToGroup(lineSketchObject);
        group.AddToGroup(lineSketchObject2);

        SketchObjectGroupData groupData = (group as ISerializableComponent).GetData() as SketchObjectGroupData;
        string xmlFilePath = Serializer.WriteTestXmlFile<SketchObjectGroupData>(groupData);
        Serializer.DeserializeFromXmlFile<SketchObjectGroupData>(out SketchObjectGroupData readGrouptData, xmlFilePath);
        Debug.Log(readGrouptData.SketchObjects[0].GetType());

        SketchObjectGroup deserGroup = Instantiate(defaults.SketchObjectGroupPrefab).GetComponent<SketchObjectGroup>();
        (deserGroup as ISerializableComponent).ApplyData(readGrouptData);

        deserGroup.transform.position += new Vector3(3, 0, 0);

        //GameObject selectionGO = new GameObject("sketchObjectSelection", typeof(SketchObjectSelection));
        //GameObject selectionGO = Instantiate(selectionPrefab);
        //SketchObjectSelection selection = selectionGO.GetComponent<SketchObjectSelection>();
        //selection.addToSelection(lineSketchObject);
        //selection.addToSelection(lineSketchObject2);
        //selection.activate();
        //StartCoroutine(deactivateSelection(selection));
    }

    private void SketchWorldSerializationTest()
    {
        lineSketchObject.AddControlPoint(new Vector3(-2, 1, 0));
        lineSketchObject.AddControlPoint(Vector3.one);
        lineSketchObject.AddControlPoint(new Vector3(2, 2, 0));
        lineSketchObject.AddControlPoint(new Vector3(2, 1, 0));
        //lineSketchObject.gameObject.GetComponent<MeshRenderer>().material = twoSidedMaterial;
        lineSketchObject.gameObject.GetComponent<MeshRenderer>().material = ropeMaterial;
        lineSketchObject.SetLineCrossSection(CircularCrossSection.GenerateVertices(4), CircularCrossSection.GenerateVertices(4,1f), .4f);

        //lineSketchObject.setLineDiameter(.7f);
        //StartCoroutine(changeDiameter());

        lineSketchObject2.AddControlPoint(new Vector3(1, 0, 0));
        lineSketchObject2.AddControlPoint(new Vector3(2, 1, 1));
        lineSketchObject2.AddControlPoint(new Vector3(3, 2, 0));
        lineSketchObject2.AddControlPoint(new Vector3(4, 4, 4));
        lineSketchObject2.DeleteControlPoint();
        lineSketchObject2.DeleteControlPoint();
        lineSketchObject2.DeleteControlPoint();
        lineSketchObject2.DeleteControlPoint();
        lineSketchObject2.DeleteControlPoint();
        lineSketchObject2.AddControlPoint(new Vector3(1, 0, 0));
        lineSketchObject2.AddControlPoint(new Vector3(2, 1, 1));
        lineSketchObject2.AddControlPoint(new Vector3(3, 2, 0));
        lineSketchObject2.AddControlPoint(new Vector3(4, 4, 4));
        lineSketchObject2.GetComponent<MeshRenderer>().material.color = Color.blue;
        lineSketchObject2.gameObject.GetComponent<MeshRenderer>().material = ropeMaterial;

        patchSketchObject.transform.position += new Vector3(3,0,0);
        patchSketchObject.Width = 3;
        patchSketchObject.AddPatchSegment(new List<Vector3> { new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(2,0,0) });
        patchSketchObject.AddPatchSegment(new List<Vector3> { new Vector3(0, 0, 1), new Vector3(1, 2, 1), new Vector3(2, 0, 1) });
        patchSketchObject.AddPatchSegment(new List<Vector3> { new Vector3(0, 0, 2), new Vector3(1, 0, 2), new Vector3(2, 0, 2) });

        (List<Vector3> points, List<Quaternion> rotations) = RibbonTest.GetPointTransformation(ControlPointParent);
        ribbonSketchObject.SetControlPoints(points, rotations);
        SketchWorld.AddObject(ribbonSketchObject);

        //lineSketchObject2.minimumControlPointDistance = 2f;
        //lineSketchObject2.addControlPointContinuous(new Vector3(3, 1, 0));
        GameObject groupGO = new GameObject("sketchObjectGroup", typeof(SketchObjectGroup));
        SketchObjectGroup group = groupGO.GetComponent<SketchObjectGroup>();
        group.defaults = this.defaults;

        SketchWorld.AddObject(lineSketchObject);
        group.AddToGroup(lineSketchObject2);
        group.AddToGroup(patchSketchObject);
        group.transform.position += new Vector3(2.568f, 5.555f, 1.123f);
        SketchWorld.AddObject(group);

        string worldXmlPath = System.IO.Path.Combine(Application.dataPath, "SketchWorldTest.xml");
        SketchWorld.SaveSketchWorld(worldXmlPath);

        SketchWorld2.LoadSketchWorld(worldXmlPath);

        SerializeBrushCollection();

        //SketchObjectGroupData groupData = group.GetData();
        //string xmlFilePath = Serializer.WriteTestXmlFile<SketchObjectGroupData>(groupData);
        //Serializer.DeserializeFromXmlFile<SketchObjectGroupData>(out SketchObjectGroupData readGrouptData, xmlFilePath);
        //Debug.Log(readGrouptData.SketchObjects[0].GetType());

        //SketchObjectGroup deserGroup = Instantiate(defaults.SketchObjectGroupPrefab).GetComponent<SketchObjectGroup>();
        //deserGroup.ApplyData(readGrouptData);

        //deserGroup.transform.position += new Vector3(3, 0, 0);
    }

    private void SerializeBrushCollection() {
        string brushXmlPath = System.IO.Path.Combine(Application.dataPath, "BrushCollectionTest.xml");

        BrushCollection brushes = new BrushCollection();
        brushes.Brushes.Add(lineSketchObject.GetBrush());
        brushes.Brushes.Add(patchSketchObject.GetBrush());
        brushes.Brushes.Add(ribbonSketchObject.GetBrush());
        Serializer.SerializeToXmlFile<BrushCollection>(brushes, brushXmlPath);

        BrushCollection loadedBrushes;
        Serializer.DeserializeFromXmlFile<BrushCollection>(out loadedBrushes, brushXmlPath);
        Debug.Log(loadedBrushes.Brushes.Count);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ranOnce) {
            ranOnce = true;
            //lineSketchObjectTest();
            //groupSerializationTest();
            SketchWorldSerializationTest();
        }
    }
}
