using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.Commands.Line;

public class CommandsTest : MonoBehaviour
{
    public GameObject selectionPrefab;
    public GameObject LineSketchObjectPrefab;
    private LineSketchObject lineSketchObject;
    private LineSketchObject lineSketchObject2;
    public SketchWorld sketchWorld;
    private CommandInvoker invoker;

    private bool ranOnce = false;

    // Start is called before the first frame update
    void Start()
    {

        invoker = new CommandInvoker();
        lineSketchObject = Instantiate(LineSketchObjectPrefab).GetComponent<LineSketchObject>();
        invoker.ExecuteCommand(new AddObjectToSketchWorldRootCommand(lineSketchObject, sketchWorld));

        lineSketchObject2 = Instantiate(LineSketchObjectPrefab).GetComponent<LineSketchObject>();
    }

    IEnumerator changeDiameter()
    {
        yield return new WaitForSeconds(5);
        lineSketchObject.SetLineDiameter(.1f);
    }

    IEnumerator deactivateSelection(SketchObjectSelection selection)
    {
        yield return new WaitForSeconds(3);
        selection.Deactivate();
    }

    private void lineSketchObjectTest()
    {
        lineSketchObject.AddControlPoint(new Vector3(-2, 1, 0));
        lineSketchObject.AddControlPoint(Vector3.one);
        lineSketchObject.AddControlPoint(new Vector3(2, 2, 0));
        lineSketchObject.AddControlPoint(new Vector3(2, 1, 0));

        lineSketchObject.SetLineDiameter(.7f);

        //StartCoroutine(changeDiameter());

        lineSketchObject2.AddControlPoint(new Vector3(1, 0, 0));
        lineSketchObject2.AddControlPoint(new Vector3(2, 1, 1));
        lineSketchObject2.AddControlPoint(new Vector3(3, 2, 0));
        lineSketchObject2.AddControlPoint(new Vector3(3, 1, 0));

        //GameObject selectionGO = new GameObject("sketchObjectSelection", typeof(SketchObjectSelection));
        GameObject selectionGO = Instantiate(selectionPrefab);
        GameObject groupGO = new GameObject("sketchObjectGroup", typeof(SketchObjectGroup));
        SketchObjectSelection selection = selectionGO.GetComponent<SketchObjectSelection>();
        selection.AddToSelection(lineSketchObject);
        selection.AddToSelection(lineSketchObject2);
        selection.Activate();
        StartCoroutine(deactivateSelection(selection));
    }

    private void commandsTest() {

        //CommandInvoker invoker = new CommandInvoker();
        invoker.ExecuteCommand(new AddControlPointCommand(lineSketchObject, new Vector3(-2, 1, 0)));
        invoker.ExecuteCommand(new AddControlPointCommand(lineSketchObject, new Vector3(1, 1, 1)));
        invoker.ExecuteCommand(new AddControlPointCommand(lineSketchObject, new Vector3(2, 2, 0)));
        invoker.ExecuteCommand(new AddControlPointCommand(lineSketchObject, new Vector3(2, 3, 0)));
        invoker.ExecuteCommand(new DeleteControlPointCommand(lineSketchObject));

        invoker.Undo();
        invoker.Undo();
        invoker.Undo();
        invoker.Undo();
        invoker.Undo();
        invoker.Undo();

        invoker.Redo();
        invoker.Redo();
        invoker.Redo();
        invoker.Redo();
        invoker.Redo();
        invoker.Redo();

        invoker.ExecuteCommand(new DeleteObjectCommand(lineSketchObject, sketchWorld));

        invoker.Undo();
        invoker.Redo();
        //invoker.Undo();


        //invoker.ExecuteCommand(new AddControlPointCommand(lineSketchObject, new Vector3(2, 3, 0)));
        //invoker.Redo();

        //invoker.Undo();
        //invoker.Undo();
        //invoker.Undo();
        //invoker.Redo();
        //invoker.Redo();
        //invoker.Redo();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ranOnce)
        {
            ranOnce = true;
            //lineSketchObjectTest();
            commandsTest();
        }
    }
}
