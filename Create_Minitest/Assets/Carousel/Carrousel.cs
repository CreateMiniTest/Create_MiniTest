using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Carrousel : MonoBehaviour
{

    public int numberOfImages = 10;
    public int radius = 20;
    public float SpriteOrienataion = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        buildImages();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buildImages()
    {
        this.transform.localPosition = new Vector3(this.transform.position.x, this.transform.position.y, radius);

        while (this.transform.childCount > 0)
        {
            SafeDestroy(this.transform.GetChild(0).gameObject);
        }

        GameObject[] sprites = new GameObject[numberOfImages];

        for (int i = 0; i < numberOfImages; i++)
        {
            var newSprite = (GameObject)Instantiate(Resources.Load("Car_Image"));
            newSprite.transform.parent = this.transform;
            sprites[i] = newSprite;
            var angle = (360 / numberOfImages) * (Mathf.PI / 180);
            float x = radius * Mathf.Sin(i * angle);
            float y = 0;
            float z = radius * Mathf.Cos(i * angle);
            sprites[i].transform.localPosition = new Vector3(x,y,-z);
        }
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
