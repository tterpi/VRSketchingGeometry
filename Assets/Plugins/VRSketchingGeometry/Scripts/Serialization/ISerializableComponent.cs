using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRSketchingGeometry.Serialization
{
    interface ISerializableComponent
    {
        SerializableComponentData GetData();
        void ApplyData(SerializableComponentData data);
    }
}
