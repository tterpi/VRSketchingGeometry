using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Xml.Serialization;

namespace VRSketchingGeometry.Serialization
{
    class SketchObjectGroupData
    {
        public Matrix4x4 Transform;
        [XmlArrayItem(typeof(SketchObjectData)), XmlArrayItem(typeof(LineSketchObjectData))]
        public List<SketchObjectData> SketchObjects;
        public List<SketchObjectGroupData> SketchObjectGroups;
    }
}
