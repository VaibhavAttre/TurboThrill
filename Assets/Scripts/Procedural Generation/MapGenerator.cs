using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using JetBrains.Annotations;
using System.ComponentModel.Design;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public enum DrawMode { NoiseMap, ColorMap, Mesh, FalloffMap };

    public DrawMode drawMode;

    public bool useFlatShading;
    public TerrainData terrainData;
    public NoiseData noiseData;
    public TextureData textureData;

    public Material terrainMaterial;

    public bool autoUpdate;
    public bool scroll;
    public float scrollSpeed;
    static MapGenerator mapGen;
    //public TerrainType[] regions;

    public static int mapChunkSize {
        get{
            if(mapGen == null)
            {
                mapGen = FindObjectOfType<MapGenerator>();
            }
            if (mapGen.useFlatShading) {
                return 95;
            } else {
                return 239;
            }
        }
    }

    [Range(0,6)]
    public int editorPreviewLOD;
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private void Awake()
    {
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
    }
    void Start() {
        noiseData.offset.x = 0;
        noiseData.offset.y = 0;
    }
    void Update() {
        if(mapDataThreadInfoQueue.Count > 0) {
            for(int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if(meshDataThreadInfoQueue.Count > 0) {
            for(int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    void OnValuesUpdated()
    {
        if(!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    private void OnValidate()
    {
        if(terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        } 
        if (noiseData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
    }
    
    public void DrawMapInEditor() {
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
        MapData mapData = GenerateMap(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap) {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        } else if (drawMode == DrawMode.ColorMap) {
            //display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        } else if (drawMode == DrawMode.Mesh) {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorPreviewLOD, useFlatShading);
            display.DrawMesh(meshData);
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback) {
        ThreadStart threadStart = delegate {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData> callback) {
        MapData mapData = GenerateMap(center);
        lock(mapDataThreadInfoQueue) {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback) {
        ThreadStart threadStart = delegate {
			MeshDataThread (mapData, lod, callback);
		};

		new Thread (threadStart).Start ();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback) {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, lod, useFlatShading);
        lock(meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData> (callback, meshData));
        }
    }

    public MapData GenerateMap(Vector2 center) {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize+2, mapChunkSize+2, noiseData.noiseScale, noiseData.octaves, noiseData.persistence, noiseData.lacunarity, noiseData.seed, center + noiseData.offset, noiseData.normalizedMode);
        /*
        for(int y = 0; y < mapChunkSize; y++) {
            for(int x = 0; x < mapChunkSize; x++) {
                float currentHeight = noiseMap[x,y];
                for(int i = 0; i < regions.Length; i++) {
                    if(currentHeight >= regions[i].height) {
                        colorMap[y*mapChunkSize + x] = regions[i].color;
                    } else {
                        break;
                    }
                }
            }
        }*/
        
        return new MapData(noiseMap);
    }

    struct MapThreadInfo<T> {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter) {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

public struct MapData {
    //public readonly MeshData meshData;
    public readonly float[,] heightMap;

    public MapData(float[,] heightMap) {
        this.heightMap = heightMap;
    }
}
