using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

public class DeleteToolTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("Collided !");

        LineSketchObject line = other.gameObject.GetComponent<LineSketchObject>();

        if(line == null && other is SphereCollider && other.transform.parent.GetComponent<LineSketchObject>()){
            line = other.transform.parent.GetComponent<LineSketchObject>();
        }

        line?.DeleteControlPoints(transform.position, transform.lossyScale.x / 2);
    }
}
