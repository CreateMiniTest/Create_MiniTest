using UnityEngine;

public class ImageOrientation : MonoBehaviour
{
    public Transform Target { get; }

    public float Orienataion;

    public ImageOrientation(Transform target)
    {
        Target = target;
    }

    private void OnGUI()
    {
        UpdateOrientation();
    }

    public void UpdateOrientation()
    {
        Orienataion = transform.parent.gameObject.GetComponent<Carrousel>().SpriteOrienataion;

        Vector3 fVector3 = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        Vector3 cVector3 = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);

        var newVector3 = (((fVector3 * Orienataion) + cVector3) / 2);
        transform.LookAt(newVector3);
    }
}
