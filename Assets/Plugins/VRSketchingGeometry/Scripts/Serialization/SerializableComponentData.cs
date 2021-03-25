using UnityEngine;

namespace VRSketchingGeometry.Serialization
{
    /// <summary>
    /// Base class for the data of components that implement <see cref="VRSketchingGeometry.Serialization.ISerializableComponent"/>.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class SerializableComponentData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        /// <summary>
        /// Applies the transform data in the data object to a transform object of a game object.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="transform"></param>
        public void ApplyDataToTransform(Transform transform) {
            transform.position = this.Position;
            transform.rotation = this.Rotation;
            transform.localScale = this.Scale;
        }

        /// <summary>
        /// Assign the transform values from a transform object to a data object.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="transform"></param>
        public void SetDataFromTransform(Transform transform) {
            this.Position = transform.position;
            this.Rotation = transform.rotation;
            this.Scale = transform.localScale;
        }
    }
}