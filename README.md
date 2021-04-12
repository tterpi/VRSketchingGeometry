# VRSketchingGeometry
Work in progress. Features may not be complete and the API may change.

This is a framework for developing 3D sketching applications in Unity.

## Features:
- smoothly interpolated lines
- patch surfaces
- ribbon shaped lines
- organisation of sketch objects with groups
- undo and redo
- serialization of sketches
- OBJ export of sketches

## Installation 
Add the Assets/Plugins/VRSketchingGeometry folder into the plugins folder of your Unity project.  
If you downloaded a release import the Unity package using the editor into your project.  

The materials and shaders provided are for the Built-In Render Pipeline as used by a normal 3D project template. If you want to use a Scriptable Render Pipeline you will have to upgrade the materials and replace the `Custom/TwoSidedSurfaceShader`.

## [API documentation](https://tterpi.github.io/VRSketchingGeometry/)
Read the [developer guide](https://tterpi.github.io/VRSketchingGeometry/articles/intro.html) and [API documentation](https://tterpi.github.io/VRSketchingGeometry/api/index.html) at the github pages site.

## Quick start
The following example script shows how to create new line sketch object and add few control points to it using a command invoker. At the end one command is undone.  
You will have to reference the file DefaultReferences.asset found in `Assets/Plugins/VRSketchingGeometry` in the public field `defaults`.  
See [the example script](https://github.com/tterpi/VRSketchingGeometry/blob/master/Assets/Scripts/VRSketchingExample.cs) for a more comprehensive demonstration.

    using UnityEngine;
    using VRSketchingGeometry.SketchObjectManagement;
    using VRSketchingGeometry;
    using VRSketchingGeometry.Commands;
    using VRSketchingGeometry.Commands.Line;

    public class CreateLineSketchObject : MonoBehaviour
    {
        public DefaultReferences Defaults;
        private LineSketchObject LineSketchObject;
        private SketchWorld SketchWorld;
        private CommandInvoker Invoker;

        void Start()
        {
            SketchWorld = Instantiate(Defaults.SketchWorldPrefab).GetComponent<SketchWorld>();
            LineSketchObject = Instantiate(Defaults.LineSketchObjectPrefab).GetComponent<LineSketchObject>();
            Invoker = new CommandInvoker();
            Invoker.ExecuteCommand(new AddObjectToSketchWorldRootCommand(LineSketchObject, SketchWorld));
            Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 2, 3)));
            Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 4, 2)));
            Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 5, 3)));
            Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 5, 2)));
            Invoker.Undo();
        }
    }

## Workflow
1. Instantiate a sketch world prefab. Easy access to prefabs is provided through the DefaultReferences asset at `Assets/Plugins/VRSketchingGeometry/DefaultReferences.asset`. 
2. Create sketch objects and groups from prefabs and add them to the sketch object world. Execute commands using a CommandInvoker object for undo and redo functionality. All scripts are in the VRSketchingGeometry namespace.
4. Serialize or export using methods of the sketch world script.
5. Load serialized sketch world from the serialized xml file for further editing.

An [example script](https://github.com/tterpi/VRSketchingGeometry/blob/master/Assets/Scripts/VRSketchingExample.cs) was created to show this process in practice.

## Sample scene
The sample scene contains various messy test scripts and corresponding game object.

## Tests
There are unit tests using Unity Testing Framework. (https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/index.html)
They mostly cover the undoable commands and the generation and applying of data objects.
Coverage of the unit tests should be expanded.

## Notes
Originally based on code from: https://github.com/bittermanq/KochanekBartelsSplines
