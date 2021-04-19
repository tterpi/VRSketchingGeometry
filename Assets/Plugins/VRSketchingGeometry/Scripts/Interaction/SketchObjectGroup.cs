using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement {
    /// <summary>
    /// Represents a group of objects that implement IGroupable.
    /// SketchObjectGroups can contain SketchObjects and other SketchObjectGroups.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class SketchObjectGroup : SelectableObject, ISerializableComponent
    {
        private SketchObjectGroup parentGroup;

        public DefaultReferences defaults;

        public override SketchObjectGroup ParentGroup { get => parentGroup; set => parentGroup = value; }

        private bool Highlighted = false;

        /// <summary>
        /// Add a groupable object to the group.
        /// </summary>
        /// <param name="groupableComponent"></param>
        internal void AddToGroup(IGroupable groupableComponent) {
            groupableComponent.ParentGroup = this;
            groupableComponent.resetToParentGroup();
        }

        /// <summary>
        /// Add all groupable objects of the selection to the group.
        /// </summary>
        /// <param name="sketchObjectSelection"></param>
        public void AddToGroup(SketchObjectSelection sketchObjectSelection) {
            IGroupable[] groupables = sketchObjectSelection.GetComponentsInChildren<IGroupable>();
            foreach (IGroupable groupable in groupables) {
                this.AddToGroup(groupable);
            }
        }

        /// <summary>
        /// Remove a groupable object from any group.
        /// This will put the object as first level child of the active sketch world if present.
        /// Otherwise it is put on the root level of the scene.
        /// </summary>
        /// <param name="groupedObject"></param>
        internal static void RemoveFromGroup(IGroupable groupedObject) {
            if (SketchWorld.ActiveSketchWorld != null && groupedObject != null)
            {
                if (groupedObject is SelectableObject selectableObject) {
                    SketchWorld.ActiveSketchWorld.AddObject(selectableObject);
                }
            }
            else
            {
                groupedObject.ParentGroup = null;
                groupedObject.resetToParentGroup();
            }
        }

        public override void resetToParentGroup() {
            this.transform.SetParent(ParentGroup?.transform);
        }

        public override void highlight()
        {
            if (!Highlighted) {
                Highlighted = true;
                this.gameObject.BroadcastMessage(nameof(IHighlightable.highlight));
            }
        }

        public override void revertHighlight()
        {
            if (Highlighted) {
                Highlighted = false;
                this.gameObject.BroadcastMessage(nameof(IHighlightable.revertHighlight));
            }
        }

        private SketchObjectGroupData GetSketchObjectGroupData()
        {
            SketchObjectGroupData data = new SketchObjectGroupData();
            data.SetDataFromTransform(this.transform);

            data.SketchObjects = new List<SketchObjectData>();
            data.SketchObjectGroups = new List<SketchObjectGroupData>();

            foreach (Transform childTransform in this.transform)
            {
                SketchObject sketchObject = childTransform.GetComponent<SketchObject>();
                SketchObjectGroup childGroup = childTransform.GetComponent<SketchObjectGroup>();

                if (sketchObject != null && sketchObject is ISerializableComponent serializableObject)
                {
                    SerializableComponentData objectData = serializableObject.GetData();
                    if (objectData is SketchObjectData sketchObjectData)
                    {
                        data.SketchObjects.Add(sketchObjectData);
                    }
                }
                else if (childGroup != null)
                {
                    data.SketchObjectGroups.Add(childGroup.GetSketchObjectGroupData());
                }
            }

            return data;
        }

        SerializableComponentData ISerializableComponent.GetData() {
            return this.GetSketchObjectGroupData();
        }

        private void ApplyData(SketchObjectGroupData data)
        {
            if (data == null) return;

            data.ApplyDataToTransform(this.transform);

            foreach (SketchObjectData sketchObjectData in data.SketchObjects) {
                    ISerializableComponent serializableSketchObject = sketchObjectData.Deserialize(defaults);
                    this.AddToGroup(serializableSketchObject);
            }

            foreach (SketchObjectGroupData groupData in data.SketchObjectGroups) {
                ISerializableComponent newGroup = groupData.Deserialize(defaults);
                this.AddToGroup(newGroup);
            }
        }

        /// <summary>
        /// Recreate the children of this group according to the data object.
        /// This will recursively recreate child groups.
        /// </summary>
        /// <param name="data"></param>
        void ISerializableComponent.ApplyData(SerializableComponentData data)
        {
            if (data is SketchObjectGroupData groupData)
            {
                this.ApplyData(groupData);
            }
            else {
                Debug.LogError("Invalid data object type for SketchObjectGroup.");
            }
        }
    }
}
