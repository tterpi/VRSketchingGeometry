using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRSketchingGeometry.Serialization
{
    /// <summary>
    /// Interface that marks Unity components that can be serialized 
    /// using <see cref="VRSketchingGeometry.Serialization.SerializableComponentData"/> objects.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    interface ISerializableComponent
    {
        SerializableComponentData GetData();
        void ApplyData(SerializableComponentData data);
    }
}
