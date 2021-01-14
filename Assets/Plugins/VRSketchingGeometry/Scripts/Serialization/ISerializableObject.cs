using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRSketchingGeometry.Serialization
{
    interface ISerializableObject
    {
        SerializableObjectData GetData();
        void ApplyData(SerializableObjectData data);
    }
}
