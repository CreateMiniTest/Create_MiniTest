using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageOrientation : MonoBehaviour
{

    public Transform target;
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = new Vector3(Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z);
        transform.LookAt(targetPos);

        //transform.LookAt(Camera.main.transform, Vector3.left);
    }
}
