using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Serialization;
using VRSketchingGeometry.Export;

namespace VRSketchingGeometry.SketchObjectManagement
{
    /// <summary>
    /// The root of a sketch.
    /// This is used to serialize, deserialize and export sketches.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class SketchWorld : MonoBehaviour
    {
        private static SketchWorld activeSketchWorld;

        public static SketchWorld ActiveSketchWorld { get => activeSketchWorld; set => activeSketchWorld = value; }

        public DefaultReferences defaults;

        /// <summary>
        /// Deleted SketchObjects and Groups go here and are deactivated.
        /// May be useful in the future to restore deleted objects.
        /// </summary>
        private GameObject deletedBin;

        private SketchObjectGroup RootGroup;

        void Awake()
        {
            deletedBin = new GameObject("Deleted Bin");
            deletedBin.transform.SetParent(this.transform);

            RootGroup = Instantiate(defaults.SketchObjectGroupPrefab).GetComponent<SketchObjectGroup>();
            RootGroup.gameObject.name = "RootSketchObjectGroup";
            RootGroup.transform.SetParent(this.transform);

            if (ActiveSketchWorld == null) {
                ActiveSketchWorld = this;
            }
        }

        /// <summary>
        /// Disables the game object and places it under the deleted bin.
        /// </summary>
        /// <param name="selectableObject"></param>
        internal void DeleteObject(SelectableObject selectableObject)
        {
            selectableObject.gameObject.SetActive(false);
            selectableObject.transform.SetParent(deletedBin.transform);
        }

        /// <summary>
        /// Is this object in the deleted bin of this sketch world?
        /// </summary>
        /// <param name="selectableObject"></param>
        /// <returns></returns>
        public bool IsObjectDeleted(SelectableObject selectableObject) {
            return selectableObject.transform.IsChildOf(this.deletedBin.transform);
        }

        /// <summary>
        /// Adds the game object as a child of the sketch world.
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        internal void AddObject(SelectableObject selectableObject)
        {
            RootGroup.AddToGroup(selectableObject);
        }


        /// <summary>
        /// Restores a previously deleted object.
        /// </summary>
        /// <param name="selectableObject"></param>
        public void RestoreObject(SelectableObject selectableObject) {
            if (selectableObject.transform.IsChildOf(this.deletedBin.transform))
            {
                if (selectableObject != null && selectableObject.ParentGroup != null)
                {
                    selectableObject.resetToParentGroup();
                }
                else
                {
                    RootGroup.AddToGroup(selectableObject);
                }
                selectableObject.gameObject.SetActive(true);
            }
            else {
                Debug.LogWarning("Object can not be restored because it was not deleted before.");
            }
        }

        /// <summary>
        /// Serializes the sketching game objects that are children of this sketch world. 
        /// </summary>
        public void SaveSketchWorld(string path)
        {
            ISerializableComponent serializableGroup = RootGroup as ISerializableComponent;
            Serializer.SerializeToXmlFile<SketchObjectGroupData>(serializableGroup.GetData() as SketchObjectGroupData, path);
        }

        /// <summary>
        /// Export sketch world as .obj file.
        /// </summary>
        /// <param name="path">Path of the exported .obj file including file name and file extension.</param>
        public void ExportSketchWorld(string path)
        {
            OBJExporter exporter = new OBJExporter();
            exporter.ExportGameObject(RootGroup.gameObject, path);
        }

        /// <summary>
        /// Exports as .obj to UnityEngine.Application.dataPath/ExportedSketches.
        /// </summary>
        /// <remarks>The folder is in the Assets folder when run in the editor and in the ***_Data folder in a build.</remarks>
        public void ExportSketchWorldToDefaultPath()
        {
            OBJExporter exporter = new OBJExporter();
            exporter.ExportGameObject(RootGroup.gameObject, OBJExporter.GetDefaultExportPath());
        }

        /// <summary>
        /// Load sketch world from serialized file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void LoadSketchWorld(string path)
        {
            Serializer.DeserializeFromXmlFile<SketchObjectGroupData>(out SketchObjectGroupData groupData, path);
            if (RootGroup.transform.childCount != 0) {
                Debug.LogError("Root group of sketch world is not empty! Please create empty sketch world to load file.");
                return;
            }
            (RootGroup as ISerializableComponent).ApplyData(groupData);
        }
    }

}

