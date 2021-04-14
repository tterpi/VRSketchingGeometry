using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

namespace VRSketchingGeometry.Serialization {
    public class BrushCollection
    {
        [XmlElement(typeof(Brush)), XmlElement(typeof(RibbonBrush)), XmlElement(typeof(LineBrush))]
        public List<Brush> Brushes = new List<Brush>();
    }
}

