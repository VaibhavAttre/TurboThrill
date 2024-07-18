using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class GrassGeneratorTest : MonoBehaviour
{
    [System.Serializable]
    public class GrassSettings {

        public float maxBendAngle = 0;
        public float bladeHeight = 1;
        public float bladeHeightVariance = 0.1f;
        public float bladeWidth = 1;
        public float bladeWidthVariance = 0.1f;
        public float bladeCurvature = 1;
        public int maxSegments = 3;
        public Texture2D windNoiseTexture = default;
        public float windTextureScale = 1;
        public float windPeriod = 1;
        public float windScale = 1;
        public float windAmplitude = 0;
        public float cameraLODMin = 3;
        public float cameraLODMax = 30;
        public float cameraLODFactor = 1;
        public float minHeight = 5;
        public float maxHeight = 10;
    }

    [SerializeField] public MeshFilter sourceMeshFilter = default;
    [SerializeField] public ComputeShader grassComputeShader = default;
    [SerializeField] public Material material = default;
    [SerializeField] public GrassSettings settings = default;
    
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct SourceVertex
    {
        public Vector3 position;
    }

    
    private bool initialized;
    private ComputeBuffer sourceVertBuffer;
    private ComputeBuffer sourceTriBuffer;
    private ComputeBuffer drawBuffer;
    private ComputeBuffer argsBuffer;
    private ComputeShader instantiatedGrassComputeShader;
    private Material instantiatedMaterial;
    private int idGrassKernel;
    private int dispatchSize;
    float3 _CameraLOD;
    float4 _CameraPosition;
    public float[,] height;
    private Bounds localBounds;
    [System.NonSerialized] private Mesh sourceMesh = default;

    private const int SOURCE_VERT_STRIDE = sizeof(float) * 3;
    private const int SOURCE_TRI_STRIDE = sizeof(int);
    private const int DRAW_STRIDE = sizeof(float) * (3 + (3 + 1) * 3);
    private const int INDIRECT_ARGS_STRIDE = sizeof(int) * 4;

    private int[] argsBufferReset = new int[] { 0, 1, 0, 0 };

    private void OnEnable()
    {
        Debug.Log("THIS IS THE GEENRATOR TEST");
        sourceMesh = sourceMeshFilter.sharedMesh;
        if (initialized)
        {
            OnDisable();
        }
        initialized = true;

        instantiatedGrassComputeShader = Instantiate(grassComputeShader);
        instantiatedMaterial = Instantiate(material);


        Vector3[] positions = sourceMesh.vertices;
        int[] tris = sourceMesh.triangles;

        // Create the data to upload to the source vert buffer
        SourceVertex[] vertices = new SourceVertex[positions.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new SourceVertex()
            {
                position = positions[i],
            };
        }


        int numSourceTriangles = tris.Length / 3;
        int maxBladeSegments = Mathf.Max(1, settings.maxSegments);
        int maxBladeTriangles = (maxBladeSegments - 1) * 2 + 1;

        sourceVertBuffer = new ComputeBuffer(vertices.Length, SOURCE_VERT_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        sourceVertBuffer.SetData(vertices);
        sourceTriBuffer = new ComputeBuffer(tris.Length, SOURCE_TRI_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        sourceTriBuffer.SetData(tris);
        drawBuffer = new ComputeBuffer(numSourceTriangles * maxBladeTriangles, DRAW_STRIDE, ComputeBufferType.Append);
        drawBuffer.SetCounterValue(0);
        argsBuffer = new ComputeBuffer(1, INDIRECT_ARGS_STRIDE, ComputeBufferType.IndirectArguments);


        
        idGrassKernel = instantiatedGrassComputeShader.FindKernel("Main");

        instantiatedGrassComputeShader.SetBuffer(idGrassKernel, "_SourceVertices", sourceVertBuffer);
        instantiatedGrassComputeShader.SetBuffer(idGrassKernel, "_SourceTriangles", sourceTriBuffer);
        instantiatedGrassComputeShader.SetBuffer(idGrassKernel, "_DrawTriangles", drawBuffer);
        instantiatedGrassComputeShader.SetBuffer(idGrassKernel, "_IndirectArgsBuffer", argsBuffer);
        instantiatedGrassComputeShader.SetInt("_NumSourceTriangles", numSourceTriangles);
        instantiatedGrassComputeShader.SetInt("_MaxBladeSegments", maxBladeSegments);
        instantiatedGrassComputeShader.SetFloat("_BladeCurvature", Mathf.Max(0, settings.bladeCurvature));
        instantiatedGrassComputeShader.SetFloat("_MaxBendAngle", settings.maxBendAngle);
        instantiatedGrassComputeShader.SetFloat("_BladeHeight", settings.bladeHeight);
        instantiatedGrassComputeShader.SetFloat("_BladeHeightVariance", settings.bladeHeightVariance);
        instantiatedGrassComputeShader.SetFloat("_BladeWidth", settings.bladeWidth);
        instantiatedGrassComputeShader.SetFloat("_BladeWidthVariance", settings.bladeWidthVariance);
        instantiatedGrassComputeShader.SetTexture(idGrassKernel, "_WindNoiseTexture", settings.windNoiseTexture);
        instantiatedGrassComputeShader.SetFloat("_WindTexMult", settings.windTextureScale);
        instantiatedGrassComputeShader.SetFloat("_WindTimeMult", settings.windPeriod);
        instantiatedGrassComputeShader.SetFloat("_WindPosMult", settings.windScale);
        instantiatedGrassComputeShader.SetFloat("_WindAmplitude", settings.windAmplitude);
        instantiatedGrassComputeShader.SetVector("_CameraLOD", new Vector4(settings.cameraLODMax, settings.cameraLODMax, Mathf.Max(0, settings.cameraLODFactor), 0));
        instantiatedGrassComputeShader.SetFloat("_minHeight", settings.minHeight);
        instantiatedGrassComputeShader.SetFloat("_maxHeight", settings.maxHeight);

        instantiatedMaterial.SetBuffer("_DrawTriangles", drawBuffer);

        instantiatedGrassComputeShader.GetKernelThreadGroupSizes(idGrassKernel, out uint threadGroupSize, out _, out _);

        dispatchSize = Mathf.CeilToInt((float)numSourceTriangles / threadGroupSize);

        localBounds = sourceMesh.bounds;
        localBounds.Expand(Mathf.Max(settings.bladeHeight + settings.bladeHeightVariance, settings.bladeWidth + settings.bladeWidthVariance));
    }

    private void OnDisable()
    {
        if (initialized)
        {
            
            if(Application.isPlaying)
            {
                Destroy(instantiatedGrassComputeShader);
                Destroy(instantiatedMaterial);
            } else
            {
                DestroyImmediate(instantiatedGrassComputeShader);
                DestroyImmediate(instantiatedMaterial);
            }

            sourceVertBuffer.Release();
            sourceTriBuffer.Release();
            drawBuffer.Release();
            argsBuffer.Release();
        }
        initialized = false;
    }

    
    public Bounds TransformBounds(Bounds boundsOS)
    {
        var center = transform.TransformPoint(boundsOS.center);


        var extents = boundsOS.extents;
        var axisX = transform.TransformVector(extents.x, 0, 0);
        var axisY = transform.TransformVector(0, extents.y, 0);
        var axisZ = transform.TransformVector(0, 0, extents.z);

        extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
        extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
        extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

        return new Bounds { center = center, extents = extents };
    }


    private void LateUpdate()
    {

        if (Application.isPlaying == false)
        {
            OnDisable();
            OnEnable();
        }

        drawBuffer.SetCounterValue(0);
        argsBuffer.SetData(argsBufferReset);

        Bounds bounds = TransformBounds(localBounds);

        instantiatedGrassComputeShader.SetVector("_Time", new Vector4(0, Time.timeSinceLevelLoad, 0, 0));
        instantiatedGrassComputeShader.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
        instantiatedGrassComputeShader.SetVector("_CameraPosition", Camera.main.transform.position);
        instantiatedGrassComputeShader.Dispatch(idGrassKernel, dispatchSize, 1, 1);

        Graphics.DrawProceduralIndirect(instantiatedMaterial, bounds, MeshTopology.Triangles, argsBuffer, 0,
            null, null, ShadowCastingMode.Off, true, gameObject.layer);
    }
}