using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MapGenerator))]
public class MapGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = (MapGenerator) target;
        if(DrawDefaultInspector()) {
            if(map.autoUpdate) {
                map.DrawMapInEditor();
            } 
        }
        if(GUILayout.Button("Generate")) {
            map.DrawMapInEditor();
        }
    }
}
