Shader "Custom/Terrain"
{
    Properties
    {
        testTexture("Texture", 2D) = "white"{}
        testScale("Scale", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        const static int maxColorCount = 8;
        const static int epsilon = 1E-4;

        float minHeight;
        float maxHeight;
        int colorCount;
        float3 colors[maxColorCount];
        float heights[maxColorCount];
        float blends[maxColorCount];
        sampler2D testTexture;
        float testScale;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        float inverseLerp(float a, float b, float value) {

            return saturate((value-a)/(b-a));
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
           /* float heightPercent = inverseLerp(minHeight, maxHeight, In.worldPos.y);
            for(int i = 0; i < colorCount; i++) {
                float drawStrength = inverseLerp(-blends[i]/2 - epsilon, blends[i]/2, heightPercent - heights[i]);
                o.Albedo = o.Albedo * (1-drawStrength) + colors[i] * drawStrength;
            }
            float3 scaledWorldPos = IN.worldPos/testScale;
            float3 blendedAxes = abs(IN.worldNormal);
            blendedAxes /= blendedAxes.x + blendedAxes.y + blendedAxes.z;
            float3 xProj = tex2D(testTexture, scaledWorldPos.yz) * blendedAxes.x;
            float3 yProj = tex2D(testTexture, scaledWorldPos.xz) * blendedAxes.y;
            float3 zProj = tex2D(testTexture, scaledWorldPos.yz) * blendedAxes.z;
            o.Albedo = xProj + yProj + zProj;*/
            o.Albedo = float3(0, 0, 0); 
        }
        ENDCG
    }
    FallBack "Diffuse"
}
