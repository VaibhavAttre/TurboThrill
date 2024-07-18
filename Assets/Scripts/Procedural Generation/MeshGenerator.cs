using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

//[RequireComponent(typeof(MeshFilter))]
public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail
        , bool useFlatshading) {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
        int increment = (levelOfDetail == 0)?1:levelOfDetail * 2;
        int borderedSize = heightMap.GetLength(0);
        int meshSize = borderedSize-2*increment;
        int meshSizeUnsimplified = borderedSize-2;
        float topleftX = (meshSizeUnsimplified-1)/-2f;
        float topleftZ = (meshSizeUnsimplified-1)/2f;
        int verticiesPerLine = (meshSize-1)/increment + 1;
        MeshData meshData = new MeshData(verticiesPerLine, verticiesPerLine, useFlatshading);
        int[,] vertexIndiciesMap = new int[borderedSize, borderedSize];
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;

        for(int y = 0; y < borderedSize; y += increment) {
            for(int x = 0; x < borderedSize; x+= increment) {
                bool isBorderVertex = (y==0) || (y == borderedSize-1) || (x==0) || (x==borderedSize-1);

                if(isBorderVertex) {
                    vertexIndiciesMap[x,y] = borderVertexIndex;
                    borderVertexIndex--;
                } else {
                    vertexIndiciesMap[x,y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for(int y = 0; y < borderedSize; y += increment) {
            for(int x = 0; x < borderedSize; x+= increment) {
                int vertexIndx = vertexIndiciesMap[x,y];
                Vector2 percent  = new Vector2((x-increment)/(float)meshSize, (y-increment)/(float)meshSize);
                float height = heightCurve.Evaluate(heightMap[x,y])*heightMultiplier;   

                Vector3 vertexPos = new Vector3(topleftX + percent.x * meshSizeUnsimplified, height, topleftZ- percent.y * meshSizeUnsimplified);
                meshData.AddVertex(vertexPos, percent, vertexIndx);
                if(x < borderedSize-1 && y < borderedSize-1) {
                    int a = vertexIndiciesMap[x,y];
                    int b = vertexIndiciesMap[x+increment,y];
                    int c = vertexIndiciesMap[x,y+increment];
                    int d = vertexIndiciesMap[x+increment,y+increment];
                    meshData.AddTriangle(a, d, c);
                    meshData.AddTriangle(d, a, b);
                }
                vertexIndx++;
            }
        }
        meshData.curve = heightCurve;
        meshData.Finalize();
        return meshData;
    }

}

public static class HeightOfMesh {
    public static List<List<float>> height = new List<List<float>>();
}


public class MeshData {
    public AnimationCurve curve;
    public Vector3[] vertices;
    public int[] trianges;
    int triangleIndex;
    Vector3[] bakedNormals;
    public Vector2[] uvs;
    Vector3[] borderVecticies;
    int[] borderTriangles;
    int borderIndex;
    bool useFlatshading;
    public MeshData(int meshWidth, int meshHeight, bool useFlatshading) {
        this.useFlatshading = useFlatshading;
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        trianges = new int[(meshWidth-1)*(meshHeight-1)*6];

        borderVecticies = new Vector3[meshWidth*4 + 4];
        borderTriangles = new int[24*meshWidth];
    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex) {
        if(vertexIndex < 0) {
            borderVecticies[-vertexIndex-1] = vertexPosition;
        } else {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    public void AddTriangle(int a, int b, int c) {
        
        if(a < 0 || b < 0 || c < 0) {
            borderTriangles[borderIndex] = a;
            borderTriangles[borderIndex+1] = b;
            borderTriangles[borderIndex+2] = c;
            borderIndex += 3;
        } else {
            trianges[triangleIndex] = a;
            trianges[triangleIndex+1] = b;
            trianges[triangleIndex+2] = c;
            triangleIndex += 3;
        }

    }

    Vector3[] CalculateNormals() {

        Vector3[] normals = new Vector3[vertices.Length];
        int triCount = trianges.Length/3;
        for(int i = 0; i < triCount; i++) {
            int normalTriangleIdx = i*3;
            int vertexIdxA = trianges[normalTriangleIdx];
            int vertexIdxB = trianges[normalTriangleIdx+1];
            int vertexIdxC = trianges[normalTriangleIdx+2];

            Vector3 triangleNormal = SurfaceNormal(vertexIdxA, vertexIdxB, vertexIdxC);
            normals[vertexIdxA] += triangleNormal;
            normals[vertexIdxB] += triangleNormal;
            normals[vertexIdxC] += triangleNormal;
        }

        int borderTriCount = borderTriangles.Length/3;
        for(int i = 0; i < borderTriCount; i++) {
            int normalTriangleIdx = i*3;
            int vertexIdxA = borderTriangles[normalTriangleIdx];
            int vertexIdxB = borderTriangles[normalTriangleIdx+1];
            int vertexIdxC = borderTriangles[normalTriangleIdx+2];

            Vector3 triangleNormal = SurfaceNormal(vertexIdxA, vertexIdxB, vertexIdxC);
            if(vertexIdxA >= 0) {
                normals[vertexIdxA] += triangleNormal;
            }
            if(vertexIdxB >= 0) {
                normals[vertexIdxB] += triangleNormal;
            }
            if(vertexIdxC >= 0) {
                normals[vertexIdxC] += triangleNormal;
            }
        }

        for(int i = 0; i < normals.Length; i++) {
            normals[i].Normalize();
        }
        return normals;
    }

    Vector3 SurfaceNormal(int indexA, int indexB, int indexC) { 
        Vector3 a = (indexA <0)?borderVecticies[-indexA-1]:vertices[indexA];
        Vector3 b = (indexB <0)?borderVecticies[-indexB-1]:vertices[indexB];
        Vector3 c = (indexC <0)?borderVecticies[-indexC-1]:vertices[indexC];
        Vector3 ab = b - c;
        Vector3 ac = c - a;
        return Vector3.Cross(ab, ac).normalized;
    }

    public void Finalize()
    {
        if (useFlatshading)
        {
            FlatShading();
        } else
        {
            BakeNormals();
        }
    }

    void BakeNormals()
    {
        bakedNormals = CalculateNormals();
    }

    void FlatShading()
    {

        Vector3[] flatShadedVerticies = new Vector3[trianges.Length];
        Vector2[] flatShadedUVs = new Vector2[trianges.Length];

        for (int i = 0; i < trianges.Length; i++)
        {
            flatShadedVerticies[i] = vertices[trianges[i]];
            flatShadedUVs[i] = uvs[trianges[i]];
            trianges[i] = i;

        }
        vertices = flatShadedVerticies;
        uvs = flatShadedUVs;
    }
    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = trianges;
        mesh.uv = uvs;
        if (useFlatshading)
        {
            mesh.RecalculateBounds();
        } else
        {
            mesh.normals = bakedNormals;
        }
        return mesh;
    }
}