using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement {
    /// <summary>
    /// Represents a group of SketchObjects.
    /// This mostly uses the built in behaviour of GameObjects but limits the interface to SketchObjects and other SketchObjectGroups.
    /// SketchObjectGroups can contain SketchObjects and other SketchObjectGroups
    /// </summary>
    public class SketchObjectGroup : MonoBehaviour, IGroupable, IHighlightable, ISerializableObject
    {
        private GameObject parentGroup;

        public DefaultValues defaults;

        public GameObject ParentGroup { get => parentGroup; set => parentGroup = value; }

        public void addToGroup(SketchObject sketchObject) {
            sketchObject.transform.SetParent(this.transform);
            sketchObject.ParentGroup = this.gameObject;
        }

        public void addToGroup(SketchObjectGroup sketchObjectGroup) {
            sketchObjectGroup.transform.SetParent(this.transform);
            sketchObjectGroup.parentGroup = this.gameObject;
        }

        public void addToGroup(SketchObjectSelection sketchObjectSelection) {
            foreach (Transform selected in sketchObjectSelection.transform) {
                SketchObject so = selected.gameObject.GetComponent<SketchObject>();
                SketchObjectGroup sog = selected.gameObject.GetComponent<SketchObjectGroup>();

                if (so != null)
                {
                    addToGroup(so);
                }
                else if (sog != null) {
                    addToGroup(sog);
                }
            }
        }


        public void removeFromGroup(SketchObject sketchObject) {
            removeFromGroup(sketchObject.gameObject);
        }

        public void removeFromGroup(SketchObjectGroup sketchObjectGroup)
        {
            removeFromGroup(sketchObjectGroup.gameObject);
        }

        private void removeFromGroup(GameObject gameObject) {
            if (SketchWorld.ActiveSketchWorld != null)
            {
                gameObject.transform.SetParent(SketchWorld.ActiveSketchWorld.transform);
            }
            else {
                gameObject.transform.SetParent(null);
            }
        }

        public void resetToParentGroup() {
            this.transform.SetParent(ParentGroup?.transform);
        }

        public void highlight()
        {
            this.gameObject.BroadcastMessage(nameof(IHighlightable.highlight));
        }

        public void revertHighlight()
        {
            this.gameObject.BroadcastMessage(nameof(IHighlightable.revertHighlight));
        }

        public SketchObjectGroupData GetData()
        {
            SketchObjectGroupData data = new SketchObjectGroupData();
            data.Position = this.transform.position;
            data.Rotation = this.transform.rotation;
            data.Scale = this.transform.localScale;

            data.SketchObjects = new List<SketchObjectData>();
            data.SketchObjectGroups = new List<SketchObjectGroupData>();

            foreach (Transform childTransform in this.transform)
            {
                SketchObject sketchObject = childTransform.GetComponent<SketchObject>();
                SketchObjectGroup childGroup = childTransform.GetComponent<SketchObjectGroup>();

                if (sketchObject != null && sketchObject is ISerializableObject serializableObject)
                {
                    SerializableObjectData objectData = serializableObject.GetData();
                    if (objectData is SketchObjectData sketchObjectData)
                    {
                        data.SketchObjects.Add(sketchObjectData);
                    }
                }
                else if (childGroup != null)
                {
                    data.SketchObjectGroups.Add(childGroup.GetData());
                }
            }

            return data;
        }

        public void ApplyData(SketchObjectGroupData data)
        {
            if (data == null) return;

            this.transform.position = data.Position;
            this.transform.rotation = data.Rotation;
            this.transform.localScale = data.Scale;

            foreach (SketchObjectData sketchObjectData in data.SketchObjects) {
                if (sketchObjectData is LineSketchObjectData) {
                    LineSketchObject sketchObject = Instantiate(defaults.LineSketchObjectPrefab).GetComponent<LineSketchObject>();
                    sketchObject.ApplyData(sketchObjectData);
                    addToGroup(sketchObject);
                }
                //Todo: Handle other types of SketchObjects
            }

            foreach (SketchObjectGroupData groupData in data.SketchObjectGroups) {
                SketchObjectGroup newGroup = Instantiate(defaults.SketchObjectGroupPrefab).GetComponent<SketchObjectGroup>();
                this.addToGroup(newGroup);
                newGroup.ApplyData(groupData);
            }
        }

        SerializableObjectData ISerializableObject.GetData()
        {
            return this.GetData();
        }

        public void ApplyData(SerializableObjectData data)
        {
            if (data is SketchObjectGroupData groupData) {
                this.ApplyData(groupData);
            }
        }
    }
}
