# Extension guide

In order to implement a new type of sketch object you will have to create a new class that derives from [SketchObject](xref:VRSketchingGeometry.SketchObjectManagement.SketchObject). The base SketchObject class implements grouping and highlighting which you can override if needed.

To make your sketch object serializable you will also have to implement the interface `ISerializableComponent` in the `VRSketchingGeometry.Serialization` namespace.
You also have to create a new data class that derives from [SketchObjectData](xref:VRSketchingGeometry.Serialization.SketchObjectData).
The new data class has to be added to the [XmlArrayItem](https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlarrayitemattribute?view=net-5.0) attributes of the SketchObjects List in [SketchObjectGroupData](xref:VRSketchingGeometry.Serialization.SketchObjectGroupData). This is needed so the data classes derived from SketchObjectData are deserialized correctly.  

To support brushes you will have to implement the [IBrushable](xref:VRSketchingGeometry.SketchObjectManagement.IBrushable) interface and create your own type of [Brush](xref:VRSketchingGeometry.Serialization.Brush) class if needed.  

You can also derive from existing SketchObjects. In this case you will have to adjust the serialization. Either you create a new type of data object 
or you add an Enum to the existing data object to differentiate between the SketchObject types.
