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
        myTarget._NumberOfImages = EditorGUILayout.IntSlider(myTarget._NumberOfImages, 2, 20);


        EditorGUILayout.LabelField("Carrousel _Radius:");
        myTarget._Radius = EditorGUILayout.IntSlider(myTarget._Radius, 1, 500);

        EditorGUILayout.LabelField("Sprite Orientation:");
        myTarget._SpriteOrienataion = EditorGUILayout.Slider(myTarget._SpriteOrienataion, 0.0f, 10.0f);

        if (GUILayout.Button("Build"))
        {
            myTarget.BuildImages();
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


