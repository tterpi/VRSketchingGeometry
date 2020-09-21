using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SketchObjectManagement
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
        public void deleteObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            gameObject.transform.SetParent(deletedBin.transform);
        }

        /// <summary>
        /// Adds the game object as a child of the sketch world.
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        public void addObject(GameObject gameObject)
        {
            gameObject.transform.SetParent(this.transform);
        }

        /// <summary>
        /// Serializes the sketching game objects that are children of this sketch world. 
        /// </summary>
        public void saveSketchWorld()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Export sketch world as .obj file.
        /// </summary>
        /// <param name="path"></param>
        public void exportSketchWorld(string path)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Load sketch world from serialized file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SketchWorld loadSketchWorld(string path)
        {
            throw new System.NotImplementedException();
        }
    }

}

