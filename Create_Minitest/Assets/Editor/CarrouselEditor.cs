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
        myTarget._Radius = EditorGUILayout.IntSlider(myTarget._Radius, 1, 100);

        EditorGUILayout.LabelField("Sprite Orientation:");
        myTarget._SpriteOrienataion = EditorGUILayout.Slider(myTarget._SpriteOrienataion, 0.0f, 100.0f);


        if (GUILayout.Button("Set up"))
        {
            myTarget.SetUp();
        }

        if (GUILayout.Button("Rebuild"))
        {
            myTarget.BuildImages();
        }
    }
}


