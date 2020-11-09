//-----------------------------------------------------------------------
//
// Original repository: https://github.com/tterpi/VRSketchingGeometry
//
//-----------------------------------------------------------------------

//namespace Sketching
//{
//    using GoogleARCore;
//    using UnityEngine;
//    using UnityEngine.EventSystems;
//    using System.Collections.Generic;

//    /// <summary>
//    /// Controls the creation and deletion of line sketch objects via touch gestures.
//	/// Example implementation of a sketching tool based on the ARCore SDK and touch input.
//    /// </summary>
//    public class LineSketchingTool : MonoBehaviour
//    {
//        /// <summary>
//        /// The first-person camera being used to render the passthrough camera image (i.e. AR
//        /// background).
//        /// </summary>
//        public Camera FirstPersonCamera;

//        /// <summary>
//        /// Prefab that is instatiated to create a new line
//        /// </summary>
//        public GameObject SketchObjectPrefab;

//        /// <summary>
//        /// The anchor that all sketch objects are attached to
//        /// </summary>
//        private Anchor worldAnchor;

//        /// <summary>
//        /// The line sketch object that is currently being created.
//        /// </summary>
//        private LineSketchObject currentLineSketchObject;
//        /// <summary>
//        /// The previously created lines
//        /// </summary>
//        private Stack<LineSketchObject> LineSketchObjects = new Stack<LineSketchObject>();

//        /// <summary>
//        /// Used to check if the touch interaction should be performed
//        /// </summary>
//        private bool canStartTouchManipulation = false;

//        /// <summary>
//        /// Shows were new control points are added
//        /// </summary>
//        public GameObject pointMarker;

//        /// <summary>
//        /// True if a new touch was started, false if a new sketch object was created during this touch
//        /// </summary>
//        private bool startNewSketchObject = false;

//        public void Start()
//        {
//            //set up marker in the center of the screen
//            pointMarker.transform.SetParent(FirstPersonCamera.transform);
//            pointMarker.transform.localPosition = Vector3.forward * .3f;
//        }

//        public void Update()
//        {
//            //handle the touch input
//            if (Input.touchCount > 0) {
//                Touch currentTouch = Input.GetTouch(0);
//                if (currentTouch.phase == TouchPhase.Began) {
//                        canStartTouchManipulation = CanStartTouchManipulation();
//                }

//                if (canStartTouchManipulation) {
//                    if (currentTouch.phase == TouchPhase.Began)
//                    {
//                        startNewSketchObject = true;
//                    }
//                    else if (currentTouch.phase == TouchPhase.Stationary || (currentTouch.phase == TouchPhase.Moved && startNewSketchObject == false && currentLineSketchObject.getNumberOfControlPoints()>0))
//                    {

//                        if (startNewSketchObject) {
//                            //create a new sketch object
//                            CreateNewLineSketchObject();
//                            startNewSketchObject = false;
//                        }
//                        else if (currentLineSketchObject)
//                        {
//                            //Add new control point according to current device position
//                            currentLineSketchObject.addControlPointContinuous(FirstPersonCamera.transform.position + FirstPersonCamera.transform.forward * .3f);
//                        }
//                    }
//                    else if (currentTouch.phase == TouchPhase.Ended) {
//                        //if an empty sketch object was created, delete it
//                        if (startNewSketchObject == false && currentLineSketchObject.getNumberOfControlPoints() < 1)
//                        {
//                            Destroy(currentLineSketchObject.gameObject);
//                            currentLineSketchObject = null;
//                        }
                        
//                        //if a swipe occured and no new sketch object was created, delete the last sketch object
//                        if ((currentTouch.position - currentTouch.rawPosition).magnitude > Screen.width * 0.05
//                            && ((startNewSketchObject == false && currentLineSketchObject == null) || startNewSketchObject == true))
//                        {
//                            DeleteLastLineSketchObject();
//                        }
//                        else {
//                            AddCurrentLineSketchObjectToStack();
//                        }

//                        canStartTouchManipulation = false;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Checks if a touch interaction can be started
//        /// </summary>
//        /// <returns></returns>
//        private bool CanStartTouchManipulation()
//        {
//            // Should not handle input if the player is pointing on UI or if the AR session is not tracking the environment.
//            if (Session.Status != SessionStatus.Tracking || EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
//            {
//                Debug.Log("Not starting tap gesture");
//                return false;
//            }
//            return true;
//        }

//        /// <summary>
//        /// Instatiates a new LineSketchObject and parants it to the world anchor
//        /// </summary>
//        private void CreateNewLineSketchObject()
//        {
//            //see if an anchor exists
//            if (!worldAnchor) {
//                worldAnchor = Session.CreateAnchor(Frame.Pose);
//            }
//            // Instantiate sketch object as child of anchor
//            var gameObject = Instantiate(SketchObjectPrefab, worldAnchor.gameObject.transform);
//            currentLineSketchObject = gameObject.GetComponent<LineSketchObject>();
//        }

//        /// <summary>
//        /// Saves a LineSketchObject to the stack so it can be deleted later
//        /// </summary>
//        private void AddCurrentLineSketchObjectToStack()
//        {
//            //add the current line sketch object to the stack
//            if (currentLineSketchObject != null && currentLineSketchObject.gameObject != null && !LineSketchObjects.Contains(currentLineSketchObject)) {
//                LineSketchObjects.Push(currentLineSketchObject);
//            }

//        }

//        /// <summary>
//        /// Delete the last line sketch object from the stack.
//        /// </summary>
//        public void DeleteLastLineSketchObject() {
//            if (LineSketchObjects.Count != 0) {
//                Destroy(LineSketchObjects.Pop().gameObject);
//            }
//        }
//    }
//}
