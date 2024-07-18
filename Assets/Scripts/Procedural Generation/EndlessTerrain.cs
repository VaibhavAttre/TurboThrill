using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Tarodev;
using Unity.AI.Navigation;
using Unity.Mathematics;
using Unity.VisualScripting;

//using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EndlessTerrain : MonoBehaviour
{
    public static float maxViewDist;
    const float viewerThreshold = 25f;
    const float squareViewerThreshold = viewerThreshold*viewerThreshold;
    public LODInfo[] detailLevels;
    public Transform viewer;
    public static Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    public Material mapMaterial;
    public float range;
    public Material waterPrefab;
    public GameObject treePrefab;
    public GameObject grassPrefab;
    public GameObject missiles;
    public Texture2D grassNoise;
    public GameObject grass;
    public GameObject car;
    public ComputeShader grassComputeShader;
    public Material grassMaterial;
    public GameObject healthBar;
    int chunkSize;
    int chunksVisibleInViewDst;
    static MapGenerator mapGenerator;
    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunkVisibleLastUpdate = new List<TerrainChunk>();

    void Start() {
        mapGenerator = FindObjectOfType<MapGenerator>();
        maxViewDist = detailLevels[detailLevels.Length-1].visibleDstTreshold;
        chunkSize = MapGenerator.mapChunkSize-1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDist/chunkSize);
        UpdateVisibleChunks();
    }    
    void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z)/ mapGenerator.terrainData.scale;
        if((viewerPositionOld-viewerPosition).sqrMagnitude > squareViewerThreshold) {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks() {

        for(int i = 0; i < terrainChunkVisibleLastUpdate.Count; i++) {
            if(terrainChunkVisibleLastUpdate[i] != null) {
                terrainChunkVisibleLastUpdate[i].SetVisible(false);
            }
        }
        terrainChunkVisibleLastUpdate.Clear();
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x/chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y/chunkSize);

        for(int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
            for(int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if(terrainChunkDict.ContainsKey(viewedChunkCoord)) {
                    terrainChunkDict[viewedChunkCoord].UpdateChunk();
                } else {
                    terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize,detailLevels, transform, mapMaterial, waterPrefab, treePrefab, range, grassPrefab, grassComputeShader, grassMaterial, grassNoise, grass, missiles));
                }
            }
        }
    }

    public class TerrainChunk {
        int numTimesRun = 0;
        Vector2 pos;
        GameObject mesh;
        Bounds bounds;
        MapData mapData;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;
        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;
        bool mapDataReceived;
        int previousLODIndex = -1;
        List<GameObject> tree = new List<GameObject>();
        GameObject water;
        GameObject treePrefab;
        GameObject grassInstance;
        int size;
        float range;
        GameObject treesChunk;
        GameObject grassChunk;
        ComputeShader grassComputeShader;
        Texture2D grassNoise;
        Material grassMaterial;
        int meshColliderIndex;
        float[,] heightMap;
        LODMesh collisionLODMesh;
        bool grassActive = false;
        GameObject missiles;
        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material, Material waterPrefab, GameObject treePrefab, float range, GameObject grassPrefab
            , ComputeShader grassComputeShader, Material grassMaterial, Texture2D grassNoise, GameObject grass, GameObject missiles) {
            heightMap = new float[size, size];
            this.range = range;
           // Debug.Log("LKSDJFHKESFJKJHESFKHLJESFKJHGSELHKJESF");
            this.grassComputeShader = grassComputeShader;
            this.grassMaterial = grassMaterial;
            this.meshColliderIndex = 0;
            this.treePrefab = treePrefab;
            this.size = size;
            this.grassNoise = grassNoise;
            this.missiles = missiles;
            this.detailLevels = detailLevels;
            pos = coord*size;
            bounds = new Bounds(pos, Vector2.one * size);
            Vector3 pos3D = new Vector3(pos.x, 0, pos.y);

            mesh = new GameObject("Terrain Chunk");

            meshRenderer = mesh.AddComponent<MeshRenderer>();
            meshFilter = mesh.AddComponent<MeshFilter>();
            meshCollider = mesh.AddComponent<MeshCollider>();
            meshRenderer.material = material;
            mesh.transform.position = pos3D * mapGenerator.terrainData.scale;
            mesh.transform.parent = parent;
            mesh.transform.localScale = Vector3.one * mapGenerator.terrainData.scale;

            grassInstance = Instantiate(grass, mesh.transform.position, Quaternion.identity);

            treesChunk = new GameObject("TreesChunk");


            water = GameObject.CreatePrimitive(PrimitiveType.Plane);
            water.name = "Water";
            water.layer = LayerMask.NameToLayer("Unwalkable");
            MeshCollider mc = water.GetComponent<MeshCollider>();
            Destroy(mc);
            water.GetComponent<MeshRenderer>().material = waterPrefab;
            water.AddComponent<Floater>();
            water.GetComponent<Floater>().rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
            water.GetComponent<Floater>().depthBeforeSubmerge = 5f;
            water.GetComponent<Floater>().displacementAmount = 7f;
            water.transform.parent = parent;
            water.transform.position = mesh.transform.position;
            water.transform.localScale = mesh.transform.localScale * 25;

            SetVisible(false);
            lodMeshes = new LODMesh[detailLevels.Length];
            for(int i = 0; i < detailLevels.Length; i++) {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateChunk);
                if (detailLevels[i].useForCollider)
                {
                    collisionLODMesh = lodMeshes[i];
                }
            }
            mapGenerator.RequestMapData(pos, OnMapDataReceived);
        }
        
        void OnMapDataReceived(MapData mapData) {
            //mapGenerator.RequestMeshData(mapData, OnMeshDataRecieved);
            this.mapData = mapData;
            mapDataReceived = true;
            UpdateChunk();
        }

        void MapHeights()
        {
            float worldX = 0;
            float worldZ = 0;
            Vector3 meshPosition = mesh.transform.position;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    worldX = meshPosition.x + x;
                    worldZ = meshPosition.z + y;
                    RaycastHit hit;
                    if (Physics.Raycast(new Vector3(worldX, 100, worldZ), -Vector3.up, out hit))
                    {
                        if (hit.collider.name == "Terrain Chunk")
                        {
                            heightMap[x, y] = 100 - hit.distance;
                        }
                    }
                }
            }
        }

        public void UpdateChunk() {
            if(mapDataReceived) {
                float distFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = distFromNearestEdge <= maxViewDist;
                if(visible) {
                    int lodIndex = 0;
                    for(int i = 0; i < detailLevels.Length-1; i++) {
                        if(distFromNearestEdge > detailLevels[i].visibleDstTreshold) {
                            lodIndex = i+1;
                        } else {
                            break;
                        }
                    }
                    if(lodIndex != previousLODIndex) {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if(lodMesh.hasMesh) {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                            MeshAccessor.mesh = lodMesh.mesh;
                            meshCollider.sharedMesh = lodMesh.mesh;
                            
                            grassInstance.GetComponent<GrassGeneratorTest>().sourceMeshFilter = meshFilter;
                            grassInstance.SetActive(true);
                            grassActive = true;
                            meshColliderIndex++;
                            
                            if (lodMesh.lod <= 1)
                            {
                                MapHeights();
                                GenerateTrees();
                                
                            }

                            
                            //Debug.Log(numTimesRun);
                        } else if (!lodMesh.hasRequestedMesh) {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                    terrainChunkVisibleLastUpdate.Add(this);
                }
                SetVisible(visible);
            }
        }


        void GenerateTrees()
        {
            float[,] treeNoise = Noise.GenerateTrees(size, size, 0.5f);
            int treeCount = 0;
            int treeCount2 = 0;
            float worldX = 0;
            float worldZ = 0;
            float heightMesh = 0f;
            Vector3 meshPosition = mesh.transform.position;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float rngTree = UnityEngine.Random.Range(0f, range);
                    worldX = meshPosition.x + x;
                    worldZ = meshPosition.z + y;

                    if (treeNoise[x, y] < rngTree)
                    {
                       if (heightMap[x,y] > 5) { 
                            GameObject newTree = Instantiate(treePrefab, new Vector3(worldX, heightMap[x,y], worldZ), Quaternion.identity);
                            newTree.transform.parent = treesChunk.transform;
                            newTree.tag = "Tree";
                          //  GameObject treeHealthbar = Instantiate(healthBar);

                       }
                        if (NumMissiles.countOfMissiles < 10)
                        {
                            
                            Vector3 positionOfMissile = new Vector3(worldX, 100, worldZ);
                            GameObject missile = Instantiate(missiles, positionOfMissile, Quaternion.identity);
                            NumMissiles.countOfMissiles++;
                        }
                    }

                    
                }
            }
        }


        public void SetVisible(bool visible) {
            if(mesh != null) {
                mesh.SetActive(visible);
                water.SetActive(visible);
                treesChunk.SetActive(visible);
                if (grassActive)
                {
                    grassInstance.SetActive(visible);
                }
            }
        }

        public bool IsVisible() {
            return mesh.activeSelf;
        }
    }

    class LODMesh {
        
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        public int lod;
        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback) {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataRecieved(MeshData meshData) {
            mesh = meshData.CreateMesh();
            hasMesh = true;
            updateCallback();
        }

        public void RequestMesh(MapData mapData) {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataRecieved);
        }
    }
    [System.Serializable] 
    public struct LODInfo {
        public int lod;
        public float visibleDstTreshold;
        public bool useForCollider;
    }

    public static class MeshAccessor {
        public static Mesh mesh;
    }
}
