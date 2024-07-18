using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Noise {

    public enum NormalizeMode{Local, Global};
    private static int seedOverall;
    public static float[,] GenerateNoiseMap (int mapW, int mapH, float scale, int octaves, float persistence, 
        float lacunarity, int seed, Vector2 offset, NormalizeMode normalizeMode) {
        seedOverall = seed;
        float[,] noiseMap = new float[mapW, mapH];
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for(int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
            maxPossibleHeight += amplitude;
            amplitude *= persistence;
        }
        if(scale <= 0) {
            scale = 0.0001f;
        }

        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;
        float halfWidth = mapW/2;
        float halfHeight = mapH/2;
        for (int y = 0; y < mapH; y++) {
            for(int x = 0; x < mapW; x++) {
                
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++) {
                    float currX = (x-halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float currY = (y-halfHeight+ octaveOffsets[i].y) / scale * frequency;

                    float perlin = Mathf.PerlinNoise(currX, currY)*2-1;
                    noiseHeight += perlin*amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxHeight) {
                    maxHeight = noiseHeight;
                } else if (noiseHeight < minHeight) {
                    minHeight = noiseHeight;
                }
                noiseMap [x,y] = noiseHeight;
            }
        }
        for (int y = 0; y < mapH; y++) {
            for(int x = 0; x < mapW; x++) {
                if(normalizeMode == NormalizeMode.Local) {
                    noiseMap[x,y] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[x,y]);
                } else {
                    float normalizedHeight = (noiseMap [x, y] + 1) / (maxPossibleHeight/0.9f);
                    noiseMap[x,y] = Mathf.Clamp(normalizedHeight,0,int.MaxValue);
                }
                
            }
        }
        return noiseMap;
    }
    
    public static float[,] GenerateTrees(int mapW, int mapH, float treeNoiseScale)
    {
        System.Random prng = new System.Random(seedOverall);
        float[,] noiseMapTrees = new float[mapW, mapH];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for(int y = 0; y < mapH; y++)
        {
            for(int x = 0; x < mapW; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset, y * treeNoiseScale + yOffset);
                noiseMapTrees[x, y] = noiseValue;
            }
        }

        return noiseMapTrees;
    }
}
