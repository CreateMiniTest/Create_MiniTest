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
        myTarget.NumberOfImages = EditorGUILayout.IntSlider(myTarget.NumberOfImages, 2, 20);


        EditorGUILayout.LabelField("Carrousel _Radius:");
        myTarget.Radius = EditorGUILayout.IntSlider(myTarget.Radius, 1, 100);

        EditorGUILayout.LabelField("Sprite Orientation:");
        myTarget.SpriteOrienataion = EditorGUILayout.Slider(myTarget.SpriteOrienataion, 0.0f, 100.0f);


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


