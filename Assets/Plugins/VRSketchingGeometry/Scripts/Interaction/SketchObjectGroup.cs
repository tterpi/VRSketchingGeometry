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
    public class SketchObjectGroup : MonoBehaviour, IGroupable, IHighlightable, ISerializableComponent
    {
        private GameObject parentGroup;

        public DefaultReferences defaults;

        public GameObject ParentGroup { get => parentGroup; set => parentGroup = value; }

        public void AddToGroup(IGroupable groupableComponent) {
            groupableComponent.ParentGroup = this.gameObject;
            groupableComponent.resetToParentGroup();
        }

        public void AddToGroup(SketchObjectSelection sketchObjectSelection) {
            IGroupable[] groupables = sketchObjectSelection.GetComponentsInChildren<IGroupable>();
            foreach (IGroupable groupable in groupables) {
                this.AddToGroup(groupable);
            }
        }

        public static void RemoveFromGroup(IGroupable groupedObject) {
            if (SketchWorld.ActiveSketchWorld != null && groupedObject != null)
            {
                SketchWorld.ActiveSketchWorld.AddObject(groupedObject);
            }
            else
            {
                groupedObject.ParentGroup = null;
                groupedObject.resetToParentGroup();
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
                if (sketchObjectData is LineSketchObjectData lineSketchObjectData)
                {
                    if (lineSketchObjectData.Interpolation == LineSketchObjectData.InterpolationType.Cubic)
                    {
                        LineSketchObject sketchObject = Instantiate(defaults.LineSketchObjectPrefab).GetComponent<LineSketchObject>();
                        sketchObject.ApplyData(sketchObjectData);
                        AddToGroup(sketchObject);
                    }
                    else if (lineSketchObjectData.Interpolation == LineSketchObjectData.InterpolationType.Linear)
                    {
                        LinearInterpolationLineSketchObject sketchObject =
                            Instantiate(defaults.LinearInterpolationLineSketchObjectPrefab)
                            .GetComponent<LinearInterpolationLineSketchObject>();

                        sketchObject.ApplyData(sketchObjectData);
                        AddToGroup(sketchObject);
                    }
                }
                else if (sketchObjectData is PatchSketchObjectData patchData)
                {
                    PatchSketchObject patchSketchObject = Instantiate(defaults.PatchSketchObjectPrefab).GetComponent<PatchSketchObject>();
                    patchSketchObject.ApplyData(patchData);
                    AddToGroup(patchSketchObject);
                }
                else if (sketchObjectData is RibbonSketchObjectData ribbonData) {
                    RibbonSketchObject ribbonSketchObject = Instantiate(defaults.RibbonSketchObjectPrefab).GetComponent<RibbonSketchObject>();
                    ribbonSketchObject.ApplyData(ribbonData);
                    AddToGroup(ribbonSketchObject);
                }
            }

            foreach (SketchObjectGroupData groupData in data.SketchObjectGroups) {
                SketchObjectGroup newGroup = Instantiate(defaults.SketchObjectGroupPrefab).GetComponent<SketchObjectGroup>();
                this.AddToGroup(newGroup);
                newGroup.ApplyData(groupData);
            }
        }

        SerializableComponentData ISerializableComponent.GetData()
        {
            return this.GetData();
        }

        public void ApplyData(SerializableComponentData data)
        {
            if (data is SketchObjectGroupData groupData) {
                this.ApplyData(groupData);
            }
        }
    }
}
