using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Color[] colors;
    [Range(0,1)]
    public float[] heights;
    public float[] blends;
    float savedMinHeight;
    float savedMaxHeight;


    public void ApplyToMaterial(Material material)
    {
        material.SetInt("colorCount", colors.Length);
        material.SetColorArray("colors", colors);
        material.SetFloatArray("heights", heights);
        material.SetFloatArray("blends", blends);
        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}
