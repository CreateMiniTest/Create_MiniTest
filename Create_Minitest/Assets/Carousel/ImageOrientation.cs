using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageOrientation : MonoBehaviour
{

    public Transform target;

    public float Orienataion;
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        UpdateOrientation();
    }

    public void UpdateOrientation()
    {
        Orienataion = this.transform.parent.gameObject.GetComponent<Carrousel>().SpriteOrienataion;

        Vector3 fVector3 = new Vector3(this.transform.position.x, this.transform.position.y, Camera.main.transform.position.z);
        Vector3 cVector3 = new Vector3(Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z);

        var newVector3 = (((fVector3 * Orienataion) + cVector3) / 2);
        transform.LookAt(newVector3);
    }
}
