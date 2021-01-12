using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Serialization
{

    public class Serializer
    {
        public static void SerializeToXmlFile<T>(T objectToSerialize, String path) {
            //string path = System.IO.Path.Combine(Application.dataPath, "test_sketch.xml");
            Debug.Log("Serializing object of type " + objectToSerialize.GetType() + " to file at:\n" + path);
            // Serialize the object to a file.
            var writer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            var wfile = new System.IO.StreamWriter(path);
            writer.Serialize(wfile, objectToSerialize);
            wfile.Close();
        }

        public static void DeserializeFromXmlFile<T>(out T targetObject, String path) {
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.IO.StreamReader file = new System.IO.StreamReader(
                path);
            T deserializedObject = (T) reader.Deserialize(file);
            file.Close();

            targetObject = deserializedObject;
        }

        public static string WriteTestXmlFile<T>(T objectToWrite) {
            string path = System.IO.Path.Combine(Application.dataPath, "TestSerialization.xml");
            SerializeToXmlFile<T>(objectToWrite, path);
            return path;
        }
    }
}