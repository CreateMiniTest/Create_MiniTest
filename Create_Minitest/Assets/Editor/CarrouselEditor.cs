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
        int nrImg = EditorGUILayout.IntSlider(10, 2, 20);


        EditorGUILayout.LabelField("Carrousel Radius:");
        int radius = EditorGUILayout.IntSlider(20, 1, 500);

        if (GUILayout.Button("Build"))
        {
            myTarget.buildImages(nrImg);
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


