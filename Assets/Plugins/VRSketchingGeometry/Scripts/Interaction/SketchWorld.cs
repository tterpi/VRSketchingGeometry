using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Serialization;
using VRSketchingGeometry.Export;

namespace VRSketchingGeometry.SketchObjectManagement
{
    public class SketchWorld : MonoBehaviour
    {
        private static SketchWorld activeSketchWorld;

        public static SketchWorld ActiveSketchWorld { get => activeSketchWorld; private set => activeSketchWorld = value; }

        public DefaultReferences defaults;

        /// <summary>
        /// Deleted SketchObjects and Groups go here and are deactivated.
        /// May be useful in the future to restore deleted objects.
        /// </summary>
        private GameObject deletedBin;

        private SketchObjectGroup RootGroup;

        // Start is called before the first frame update
        void Start()
        {
            deletedBin = new GameObject("Deleted Bin");
            deletedBin.transform.SetParent(this.transform);

            RootGroup = Instantiate(defaults.SketchObjectGroupPrefab).GetComponent<SketchObjectGroup>();
            RootGroup.gameObject.name = "RootSketchObjectGroup";
            RootGroup.transform.SetParent(this.transform);
        }

        /// <summary>
        /// Disables the game object and places it under the deleted bin.
        /// </summary>
        /// <param name="gameObject"></param>
        public void DeleteObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            gameObject.transform.SetParent(deletedBin.transform);
        }

        /// <summary>
        /// Adds the game object as a child of the sketch world.
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        public void AddObject(IGroupable groupableObject)
        {
            RootGroup.AddToGroup(groupableObject);
        }


        /// <summary>
        /// Restores a previously deleted object.
        /// </summary>
        /// <param name="gameObject"></param>
        public void RestoreObject(GameObject gameObject) {
            if (gameObject.transform.IsChildOf(this.deletedBin.transform))
            {
                IGroupable groupableObject = gameObject.GetComponent<IGroupable>();
                if (groupableObject != null && groupableObject.ParentGroup != null)
                {
                    groupableObject.resetToParentGroup();
                }
                else
                {
                    RootGroup.AddToGroup(groupableObject);
                }
                gameObject.SetActive(true);
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
            Serializer.SerializeToXmlFile<SketchObjectGroupData>(RootGroup.GetData(), path);
        }

        /// <summary>
        /// Export sketch world as .obj file.
        /// </summary>
        /// <param name="path"></param>
        public void ExportSketchWorld(string path)
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
            }
            RootGroup.ApplyData(groupData);
        }
    }

}

