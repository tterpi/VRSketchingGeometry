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
    public class SketchObjectGroupData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        [XmlArrayItem(typeof(SketchObjectData)), XmlArrayItem(typeof(LineSketchObjectData))]
        public List<SketchObjectData> SketchObjects;
        public List<SketchObjectGroupData> SketchObjectGroups;

        public SketchObjectGroupData() { }

        public SketchObjectGroupData(SketchObjectGroup group) {
            this.Position = group.transform.position;
            this.Rotation = group.transform.rotation;
            this.Scale = group.transform.localScale;

            SketchObjects = new List<SketchObjectData>();
            SketchObjectGroups = new List<SketchObjectGroupData>();

            foreach (Transform childTransform in group.transform) {
                LineSketchObject line = childTransform.GetComponent<LineSketchObject>();
                SketchObjectGroup childGroup = childTransform.GetComponent<SketchObjectGroup>();

                if (line != null)
                {
                    SketchObjects.Add(line.GetData());
                }
                else if (childGroup != null) {
                    SketchObjectGroups.Add(new SketchObjectGroupData(childGroup));
                }

            }
        }
    }
}
