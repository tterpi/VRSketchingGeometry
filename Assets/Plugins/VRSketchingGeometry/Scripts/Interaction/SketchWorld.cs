using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.SketchObjectManagement
{
    public class SketchWorld : MonoBehaviour
    {
        private static SketchWorld activeSketchWorld;

        public static SketchWorld ActiveSketchWorld { get => activeSketchWorld; private set => activeSketchWorld = value; }

        //instantiate SketchObjects through the sketch world
        //or parent them to the the sketch world when instantiating in the tool

        /// <summary>
        /// Deleted SketchObjects and Groups go here and are deactivated.
        /// May be useful in the future to restore deleted objects.
        /// </summary>
        private GameObject deletedBin;

        // Start is called before the first frame update
        void Start()
        {
            deletedBin = new GameObject("Deleted Bin");
            deletedBin.transform.SetParent(this.transform);
        }

        // Update is called once per frame
        void Update()
        {

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
        public void AddObject(GameObject gameObject)
        {
            gameObject.transform.SetParent(this.transform);
        }


        /// <summary>
        /// Restores a previously deleted object.
        /// </summary>
        /// <param name="gameObject"></param>
        public void RestoreObject(GameObject gameObject) {
            if (gameObject.transform.IsChildOf(this.deletedBin.transform))
            {
                IGroupable groupableObject = gameObject.GetComponent<IGroupable>();
                if (groupableObject != null)
                {
                    groupableObject.resetToParentGroup();
                }
                else
                {
                    gameObject.transform.SetParent(this.transform);
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
        public void SaveSketchWorld()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Export sketch world as .obj file.
        /// </summary>
        /// <param name="path"></param>
        public void ExportSketchWorld(string path)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Load sketch world from serialized file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SketchWorld LoadSketchWorld(string path)
        {
            throw new System.NotImplementedException();
        }
    }

}

