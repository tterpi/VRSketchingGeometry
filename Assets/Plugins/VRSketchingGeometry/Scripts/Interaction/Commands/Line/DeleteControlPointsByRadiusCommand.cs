using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.Commands.Line
{
    /// <summary>
    /// Delete control points within a sphere volume defined by a point and radius.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class DeleteControlPointsByRadiusCommand : ICommand
    {
        private LineSketchObject OriginalLineSketchObject;
        /// <summary>
        /// New sketch objects created during deletion of control points.
        /// </summary>
        private List<LineSketchObject> NewLines;
        /// <summary>
        /// Control points of OriginalLineSketchObject before deletion.
        /// </summary>
        private List<Vector3> OriginalControlPoints;
        /// <summary>
        /// Control points of OriginalLineSketchObject after deletion.
        /// </summary>
        private List<Vector3> NewControlPoints;
        /// <summary>
        /// Point around which is deleted.
        /// </summary>
        private Vector3 Point;
        /// <summary>
        /// Radius around Point.
        /// </summary>
        private float Radius;

        /// <summary>
        /// Command for deleting control points within a radius around a point of a line sketch object. 
        /// </summary>
        /// <param name="lineSketchObject"></param>
        /// <param name="point">Point around which the control points will be deleted.</param>
        /// <param name="radius">Radius around point in which the control points are deleted.</param>
        public DeleteControlPointsByRadiusCommand(LineSketchObject lineSketchObject, Vector3 point, float radius)
        {
            this.OriginalLineSketchObject = lineSketchObject;
            this.Point = point;
            this.Radius = radius;
        }

        public bool Execute()
        {
            this.OriginalControlPoints = OriginalLineSketchObject.GetControlPoints();
            bool didDelete = OriginalLineSketchObject.DeleteControlPoints(Point, Radius, out NewLines);
            if (OriginalLineSketchObject.gameObject.activeInHierarchy)
            {
                NewControlPoints = OriginalLineSketchObject.GetControlPoints();
            }
            else {
                NewControlPoints = null;
            }
            return didDelete;
        }

        public void Redo()
        {
            if (NewControlPoints == null)
            {
                SketchWorld.ActiveSketchWorld.DeleteObject(OriginalLineSketchObject);
            }
            else {
                OriginalLineSketchObject.SetControlPointsLocalSpace(NewControlPoints);
            }

            foreach (LineSketchObject line in NewLines)
            {
                SketchWorld.ActiveSketchWorld.RestoreObject(line);
            }
        }

        public void Undo()
        {
            if (!OriginalLineSketchObject.gameObject.activeInHierarchy) {
                SketchWorld.ActiveSketchWorld.RestoreObject(OriginalLineSketchObject);
            }
            OriginalLineSketchObject.SetControlPointsLocalSpace(OriginalControlPoints);

            foreach (LineSketchObject line in NewLines) {
                SketchWorld.ActiveSketchWorld.DeleteObject(line);
            }

        }
    }
}
