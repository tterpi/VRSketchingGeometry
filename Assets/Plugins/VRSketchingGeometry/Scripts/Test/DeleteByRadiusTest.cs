using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Export;
using System.Xml;
using System.Xml.Serialization;
using VRSketchingGeometry;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Serialization;

public class DeleteByRadiusTest : MonoBehaviour
{
    public DefaultReferences defaults;
    public GameObject selectionPrefab;
    public GameObject LineSketchObjectPrefab;
    private LineSketchObject lineSketchObject;
    private LineSketchObject lineSketchObject2;
    public GameObject controlPointParent;
    public GameObject deletePoint;
    public float deleteRadius;

    private bool ranOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        lineSketchObject = Instantiate(LineSketchObjectPrefab).GetComponent<LineSketchObject>();
        lineSketchObject.SetLineDiameter(.5f);

        lineSketchObject2 = Instantiate(defaults.LinearInterpolationLineSketchObjectPrefab).GetComponent<LineSketchObject>();
        lineSketchObject2.SetLineDiameter(.5f);
    }

    IEnumerator changeDiameter() {
        yield return new WaitForSeconds(5);
        lineSketchObject.DeleteControlPoints(deletePoint.transform.position, deleteRadius, out List<LineSketchObject> newLines);
        OBJExporter exporter = new OBJExporter();
        string exportPath = OBJExporter.GetDefaultExportPath();
        //exporter.ExportGameObject(lineSketchObject.gameObject, exportPath);
        //Debug.Log(JsonUtility.ToJson(lineSketchObject));
        //XMLSerializeTest();
        //XMLSerializeTest2();
        //exporter.ExportGameObject(controlPointParent, exportPath);
        lineSketchObject.SetInterpolationSteps(4);
        lineSketchObject.RefineMesh();
        lineSketchObject2.RefineMesh();

        //Debug.Log(exportPath);
        //lineSketchObject.setLineDiameter(.1f);
        //yield return new WaitForSeconds(2);
        //lineSketchObject.deleteControlPoint();
        //lineSketchObject.deleteControlPoint();
    }

    private void XMLSerializeTest() {
        //LineSketchObject lso = new LineSketchObject();

        string path = System.IO.Path.Combine(Application.dataPath, "test_sketch.xml");
        Debug.Log(path);
        // Serialize the object to a file.
        // First write something so that there is something to read ...  
        var writer = new System.Xml.Serialization.XmlSerializer(typeof(Vector3[]));
        var wfile = new System.IO.StreamWriter(path);
        writer.Serialize(wfile, lineSketchObject);
        wfile.Close();

        // Now we can read the serialized book ...  
        System.Xml.Serialization.XmlSerializer reader =
            new System.Xml.Serialization.XmlSerializer(typeof(Vector3[]));
        System.IO.StreamReader file = new System.IO.StreamReader(
            path);
        Vector3[] overview = (Vector3[]) reader.Deserialize(file);
        file.Close();
    }

    private void XMLSerializeTest2() {
        string path = Serializer.WriteTestXmlFile<VRSketchingGeometry.Serialization.SerializableComponentData>
            ((lineSketchObject as ISerializableComponent).GetData());
        Serializer.DeserializeFromXmlFile(out LineSketchObjectData readData, System.IO.Path.Combine(Application.dataPath, "TestSerialization.xml"));
        LineSketchObject deserLine = Instantiate(LineSketchObjectPrefab).GetComponent<LineSketchObject>();
        readData.SketchMaterial.AlbedoColor = Color.red;
        (deserLine as ISerializableComponent).ApplyData(readData);

        deserLine.transform.position += new Vector3(0,2,0);

        Debug.Log(readData.ControlPoints.Count);
    }

    IEnumerator deactivateSelection(SketchObjectSelection selection) {
        yield return new WaitForSeconds(3);
        selection.Deactivate();
    }

    private void lineSketchObjectTest() {

        foreach (Transform controlPoint in controlPointParent.transform) {
            lineSketchObject.AddControlPoint(controlPoint.position);
            lineSketchObject2.AddControlPoint(controlPoint.position);
        }

        lineSketchObject.SetLineCrossSection(CircularCrossSection.GenerateVertices(16), CircularCrossSection.GenerateVertices(16, 1f), .5f);
        //lineSketchObject.setLineDiameter(.7f);
        StartCoroutine(changeDiameter());

        //StartCoroutine(deactivateSelection(selection));
    }

    private void SetAddComparison() {
        List<Vector3> controlPoints = new List<Vector3>();
        lineSketchObject.SetControlPointsLocalSpace(new List<Vector3>());
        foreach (Transform controlPoint in controlPointParent.transform)
        {
            lineSketchObject.AddControlPoint(controlPoint.position);
            controlPoints.Add(controlPoint.position);
        }

        lineSketchObject2.SetControlPointsLocalSpace(controlPoints);
    }

    // Update is called once per frame
    void Update()
    {
        //SetAddComparison();
        if (!ranOnce)
        {
            lineSketchObjectTest();
            ranOnce = true;
        }
    }
}
