using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class UpdatableData : ScriptableObject
{

    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate()
    {
        if(autoUpdate)
        {
            UnityEditor.EditorApplication.update += NotifyOfUpdate;
        }
    }

    public void NotifyOfUpdate()
    {
        UnityEditor.EditorApplication.update -= NotifyOfUpdate;
        if(OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }
}
