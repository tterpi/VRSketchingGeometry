using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.Commands.Line;
using VRSketchingGeometry.Commands.Ribbon;
using VRSketchingGeometry.Commands.Patch;
using VRSketchingGeometry.Commands.Group;
using VRSketchingGeometry.Commands.Selection;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Export;

public class VRSketchingExample : MonoBehaviour
{
    private CommandInvoker Invoker;
    public DefaultReferences Defaults;

    public SketchWorld SketchWorld;
    public SketchWorld DeserializedSketchWorld;
    public LineSketchObject LineSketchObject;
    public RibbonSketchObject RibbonSketchObject;
    public PatchSketchObject PatchSketchObject;
    public SketchObjectGroup SketchObjectGroup;
    public SketchObjectSelection SketchObjectSelection;
    private string SavePath;

    // Start is called before the first frame update
    void Start()
    {
        //Create a SketchWorld, many commands require a SketchWorld to be present
        SketchWorld = Instantiate(Defaults.SketchWorldPrefab).GetComponent<SketchWorld>();

        //Create a LineSketchObject
        LineSketchObject = Instantiate(Defaults.LineSketchObjectPrefab).GetComponent<LineSketchObject>();
        Invoker = new CommandInvoker();
        Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 2, 3)));
        Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 4, 2)));
        Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 5, 3)));
        Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 5, 2)));
        Invoker.Undo();
        Invoker.Redo();

        //Create a RibbonSketchObject
        RibbonSketchObject = Instantiate(Defaults.RibbonSketchObjectPrefab).GetComponent<RibbonSketchObject>();
        Invoker.ExecuteCommand(new AddPointAndRotationCommand(RibbonSketchObject, new Vector3(1, 1, 1), Quaternion.identity));
        Invoker.ExecuteCommand(new AddPointAndRotationCommand(RibbonSketchObject, new Vector3(1, 2, 2), Quaternion.Euler(25, 45, 0)));
        Invoker.ExecuteCommand(new AddPointAndRotationCommand(RibbonSketchObject, new Vector3(1, 2, 3), Quaternion.Euler(-25, 45, 0)));

        //Create a PatchSketchObject
        PatchSketchObject = Instantiate(Defaults.PatchSketchObjectPrefab).GetComponent<PatchSketchObject>();
        PatchSketchObject.Width = 3;
        Invoker.ExecuteCommand(new AddSegmentCommand(PatchSketchObject, new List<Vector3> { new Vector3(0, 0, 1), new Vector3(0, 1, 2), new Vector3(0, 0, 3) }));
        Invoker.ExecuteCommand(new AddSegmentCommand(PatchSketchObject, new List<Vector3> { new Vector3(1, 1, 1), new Vector3(1, 0, 2), new Vector3(1, 1, 3) }));
        Invoker.ExecuteCommand(new AddSegmentCommand(PatchSketchObject, new List<Vector3> { new Vector3(2, 0, 1), new Vector3(2, 1, 2), new Vector3(2, 0, 3) }));

        //Add the LineSketchObject to the SketchWorld
        Invoker.ExecuteCommand(new AddObjectToSketchWorldRootCommand(LineSketchObject, SketchWorld));
        //Create a SketchObjectGroup and add objects to it
        SketchObjectGroup = Instantiate(Defaults.SketchObjectGroupPrefab).GetComponent<SketchObjectGroup>();
        Invoker.ExecuteCommand(new AddToGroupCommand(SketchObjectGroup, RibbonSketchObject));
        Invoker.ExecuteCommand(new AddToGroupCommand(SketchObjectGroup, PatchSketchObject));
        //Add the SketchObjectGroup to the SketchWorld
        Invoker.ExecuteCommand(new AddObjectToSketchWorldRootCommand(SketchObjectGroup, SketchWorld));

        //Serialize the SketchWorld to a XML file
        SavePath = System.IO.Path.Combine(Application.dataPath, "YourSketch.xml");
        SketchWorld.SaveSketchWorld(SavePath);

        //Create another SketchWorld and load the serialized SketchWorld
        DeserializedSketchWorld = Instantiate(Defaults.SketchWorldPrefab).GetComponent<SketchWorld>();
        DeserializedSketchWorld.LoadSketchWorld(SavePath);
        DeserializedSketchWorld.transform.position += new Vector3(5, 0, 0);

        //Export the SketchWorld as an OBJ file
        SketchWorld.ExportSketchWorldToDefaultPath();

        //Select the SketchObjectGroup
        SketchObjectSelection = Instantiate(Defaults.SketchObjectSelectionPrefab).GetComponent<SketchObjectSelection>();
        Invoker.ExecuteCommand(new AddToSelectionAndHighlightCommand(SketchObjectSelection, SketchObjectGroup));
        Invoker.ExecuteCommand(new ActivateSelectionCommand(SketchObjectSelection));
    }
}
