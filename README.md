# VRSketchingGeometry
Work in progress. Features may not be complete and the API may change.

This is a framework for developing 3D sketching application in Unity.

##Features:
- smoothly interpolated lines
- patch surfaces
- organisation of sketch objects with groups
- undo and redo
- serialization of sketches
- OBJ export of sketches

## Workflow
1. Instantiate a sketch object world.
2. Create sketch objects and groups and add them to the sketch object world. If you want undo and redo functionality you have to execute commands.
3. Serialize or export using methods of the sketch object world script.

## Quick start
1. Instantiate SketchObject.prefab to create a new line.
2. Use methods of attached LineSketchObject component to add and remove control points of the line.

## Sample scene
The sample scene contains various messy test scripts and corresponding game object.

## Notes
Originally based on code from: https://github.com/bittermanq/KochanekBartelsSplines
