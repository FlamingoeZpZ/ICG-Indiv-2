Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ToonRamp("ToonRamp", 2D) = "" {}
        _Frequency("Frequency", Range(0,50)) = 3
        _Speed("Speed", Range(0,100)) = 10
        _Amp("Amplitude", Range(0,1)) = 0.5 
    }
    SubShader
    {
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf ToonRamp vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        

        struct Input
        {
            float2 uv_MainTex;
            float3 vertColor;
        };
        
        sampler2D _MainTex, _ToonRamp;
        fixed4 _Color;
        float _Frequency;
        float _Speed;
        float _Amp;

        struct appdata
        {
            float4 vertex: POSITION;
            float3 normal: NORMAL;
            float4 texcoord:TEXCOORD0;
            float4 texcoord1:TEXCOORD1;
            float4 texcoord2:TEXCOORD2;
        };

        void vert(inout appdata v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float t = _Time * _Speed;

            float waveHeight = sin(t + v.vertex.x * _Frequency) + sin(t*2 + v.vertex.x * _Frequency*2);
            waveHeight = waveHeight > 0? _Amp: -_Amp;
            
            v.vertex.y += waveHeight;
            v.normal = normalize(float3(v.normal.x + waveHeight, v.normal.y, v.normal.z));
            o.vertColor = waveHeight + 2;
        }

        half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
        {
            const half diff = max(0, dot(s.Normal, lightDir));
            const float h = diff*0.5f+0.5f; //Make difference noticable
            const float2 rh = h;
            const float3 ramp = tex2D(_ToonRamp, rh).rgb;

            float4 c; // Why not a fixed 4?
            c.rgb = s.Albedo * _LightColor0.rgb  * ramp;
            c.a = s.Alpha;
            return c;
        }
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            
        }
        
        ENDCG
    }
    FallBack "Diffuse"

}
