using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Serialization
{
    /// <summary>
    /// This class provides methods for serializing and deserializing objects to and from XML files.
    /// </summary>
    /// <remark>Original author: tterpi</remark>
    public class Serializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>The type t should only contain fields of types that can be automatically serialized by the
        /// .NET XmlSerializer class. This is the case for the data classes in the <see cref="VRSketchingGeometry.Serialization"/> namespace.
        /// </remarks>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="path">File path of the xml file.</param>
        public static void SerializeToXmlFile<T>(T objectToSerialize, String path) {
            //string path = System.IO.Path.Combine(Application.dataPath, "test_sketch.xml");
            Debug.Log("Serializing object of type " + objectToSerialize.GetType() + " to file at:\n" + path);
            // Serialize the object to a file.
            var writer = new System.Xml.Serialization.XmlSerializer(typeof(T), "https://github.com/tterpi/VRSketchingGeometry");
            var wfile = new System.IO.StreamWriter(path);
            writer.Serialize(wfile, objectToSerialize);
            wfile.Close();
        }

        /// <summary>
        /// This will deserialize an object from a XML file. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetObject"></param>
        /// <param name="path"></param>
        public static void DeserializeFromXmlFile<T>(out T targetObject, String path) {
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(T), "https://github.com/tterpi/VRSketchingGeometry");
            System.IO.StreamReader file = new System.IO.StreamReader(
                path);
            T deserializedObject = (T) reader.Deserialize(file);
            file.Close();

            targetObject = deserializedObject;
        }

        /// <summary>
        /// Writes an object as xml to a default path.
        /// </summary>
        /// <remarks>Default path is <c>UnityEngine.Application.dataPath</c></remarks>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="objectToWrite"></param>
        /// <returns></returns>
        public static string WriteTestXmlFile<T>(T objectToWrite) {
            string path = System.IO.Path.Combine(Application.dataPath, "TestSerialization.xml");
            SerializeToXmlFile<T>(objectToWrite, path);
            return path;
        }
    }
}