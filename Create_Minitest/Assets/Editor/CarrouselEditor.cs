using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Carrousel))]
public class CarrouselEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Carrousel myTarget = (Carrousel)target;
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Number of Images:");
        myTarget.numberOfImages = EditorGUILayout.IntSlider(myTarget.numberOfImages, 2, 20);


        EditorGUILayout.LabelField("Carrousel Radius:");
        myTarget.radius = EditorGUILayout.IntSlider(myTarget.radius, 1, 500);

        EditorGUILayout.LabelField("Sprite Orientation:");
        myTarget.SpriteOrienataion = EditorGUILayout.Slider(myTarget.SpriteOrienataion, 0.0f, 10.0f);

        if (GUILayout.Button("Build"))
        {
            myTarget.buildImages();
        }
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


