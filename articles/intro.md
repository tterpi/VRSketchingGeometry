# Developer guide

This guide is intended to give an overview about the most important classes and their usage.

## [Sketch World](xref:VRSketchingGeometry.SketchObjectManagement.SketchWorld)
The sketch world is the root of a sketch. It provides methods for serializing, deserializing and exporting sketches.
Groups and Sketch-Objects can be added to the sketch world.

## [Sketch Object](xref:VRSketchingGeometry.SketchObjectManagement.SketchObject)
The sketch object class is the base class for all types of sketch objects.

## [Line Sketch Object](xref:VRSketchingGeometry.SketchObjectManagement.LineSketchObject)
The line sketch objects allows to create continuous lines represented by a tube shaped mesh.
It is created by specifying control points (Vector3) which are smoothly interpolated.

## [Ribbon Sketch Object](xref:VRSketchingGeometry.SketchObjectManagement.RibbonSketchObject)
The ribbon sketch object allows to create flat ribbon shaped lines.
For each point of the mesh you have to provide a position (Vector3) and rotation (Quaternion).
It does not interpolate control points but connects them directly.
There have to be at least two points for a mesh to be created.

## [Patch Sketch Object](xref:VRSketchingGeometry.SketchObjectManagement.PatchSketchObject)
This allows you to create a surface with four corners.
A patch mesh is only created once you have provided at least three by three control points.
The width of the patch has to be specified and then you can add segments of control points (list of Vector3). The width specifies a number of control points, not the spacial size of the patch.
The control points are smoothly interpolated.

## [Sketch Object Group](xref:VRSketchingGeometry.SketchObjectManagement.SketchObjectGroup)
The group object allows you to organize a sketch. You can add sketch objects and other groups to a group.

## [Sketch Object Selection](xref:VRSketchingGeometry.SketchObjectManagement.SketchObjectSelection)
The selection allows you to highlight and transform multiple sketch objects and groups together.

## [Command Invoker](xref:VRSketchingGeometry.Commands.CommandInvoker)
The command invoker is the central element of the undo/redo feature. It keeps track of all executed commands and provides methods for executing, undoing and redoing commands. To perform a command pass a command object to the execute method. It is then added to the stack of executed commands. This command can now be undone using the undo method of the command invoker. It can be redone using the redo method. 

## [IHighlightable](xref:VRSketchingGeometry.SketchObjectManagement.IHighlightable)
Components that implement this interface can be highlighted using a specified highlight shader.
The highlighting can be reverted.

## [DefaultReferences](xref:VRSketchingGeometry.DefaultReferences)
The DefaultReferences asset contains references to commonly used objects such as prefabs and materials.
It defines the default path for loading textures when deserializing a sketch.
