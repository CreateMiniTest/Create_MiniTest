using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Carrousel : MonoBehaviour
{

    private int numberOfImages = 10;
    // Start is called before the first frame update
    void Start()
    {
        buildImages(numberOfImages);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buildImages(int amount)
    {
        while (this.transform.childCount > 2)
        {
            SafeDestroy(this.transform.GetChild(0).gameObject);
        }
        GameObject proxy = new GameObject("Proxy");
        proxy.AddComponent<SpriteRenderer>();
        //proxy.GetComponent<SpriteRenderer>().sprite

        LoadSprite("Carousel/Test_Image");
        
        Vector3 pos;
        var newSprite = Instantiate(proxy);
    }

    public static Sprite LoadSprite(string path)
    {
        Sprite srt;
        if (Application.isEditor)
            srt = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        else
            srt = Resources.Load<Sprite>(path);

        return srt;
    }

    public static T SafeDestroy<T>(T obj) where T : Object
    {
        if (Application.isEditor)
            Object.DestroyImmediate(obj);
        else
            Object.Destroy(obj);

        return null;
    }
    public static T SafeDestroyGameObject<T>(T component) where T : Component
    {
        if (component != null)
            SafeDestroy(component.gameObject);
        return null;
    }
}
