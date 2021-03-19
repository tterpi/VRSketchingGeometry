# VRSketchingGeometry
Work in progress. Features may not be complete and the API may change.

This is a framework for developing 3D sketching applications in Unity.

## Features:
- smoothly interpolated lines
- patch surfaces
- organisation of sketch objects with groups
- undo and redo
- serialization of sketches
- OBJ export of sketches

## Installation 
Add the VRSketchingGeometry folder in the plugins folder of your Unity project.

## Workflow
1. Instantiate a sketch world prefab. Prefabs can be found in Assets/Plugins/VRSketchingGeometry/Prefabs. 
2. Create sketch objects and groups and add them to the sketch object world. Execute commands using a CommandInvoker object for undo and redo functionality. All scripts are in the VRSketchingGeometry namespace.
3. Serialize or export using methods of the sketch world script.

## Quick start
1. Instantiate SketchObject.prefab to create a new line.
2. Use methods of attached LineSketchObject component to add and remove control points of the line.

## Sample scene
The sample scene contains various messy test scripts and corresponding game object.

## Tests
There are unit tests using Unity Testing Framework. (https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/index.html)
They mostly cover the undoable commands and the generation and applying of data objects.
Coverage of the unit tests should be expanded.

## Notes
Originally based on code from: https://github.com/bittermanq/KochanekBartelsSplines
