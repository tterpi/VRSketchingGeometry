using UnityEngine;

namespace VRSketchingGeometry.Serialization
{
    /// <summary>
    /// Base class for components that are a <see cref="VRSketchingGeometry.SketchObjectManagement.SketchObject"/>.
    /// </summary>
    /// <remarks>
    /// Original author: tterpi
    /// Used to differentiate between group and sketch object data.
    /// </remarks>
    public abstract class SketchObjectData: SerializableComponentData
    {
    }
}