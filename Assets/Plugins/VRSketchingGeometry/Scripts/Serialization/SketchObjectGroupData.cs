using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Xml.Serialization;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Serialization
{
    public class SketchObjectGroupData : SerializableComponentData
    {
        [XmlArrayItem(typeof(SketchObjectData)), XmlArrayItem(typeof(LineSketchObjectData)), XmlArrayItem(typeof(PatchSketchObjectData)), 
            XmlArrayItem(typeof(RibbonSketchObjectData))]
        public List<SketchObjectData> SketchObjects;
        public List<SketchObjectGroupData> SketchObjectGroups;

        public SketchObjectGroupData() { }
    }
}
