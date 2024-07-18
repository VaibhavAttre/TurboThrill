using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    //public GameObject meshObj;

    public void DrawTexture(Texture2D texture) {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData) {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshFilter.transform.localScale = Vector3.one * FindObjectOfType<MapGenerator>().terrainData.scale;
        //meshRenderer.sharedMaterial.mainTexture = texture;
      //  meshObj.AddComponent<MeshCollider>();
        //Mesh mesh = meshObj.GetComponent<MeshFilter>().sharedMesh;
        //mesh.name = "terrain1";//
        //meshObj.GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
