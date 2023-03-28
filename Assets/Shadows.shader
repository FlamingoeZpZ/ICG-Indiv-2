Shader "ColoredShadow"
{
    // Define our shader properties, things that are explicitly exposed in unity
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB)", 2D) = "white" {}
        _ShadowColor("Shadow Color", Color) = (1,1,1,1)
        
        //New properties for outlining
        _Outline("Outline width", Range(0.01,1)) = 0.05
        _OutlineColor("Outline Color", Color) = (0,0,0) // Let's access this in code, so when we look at the shark it changes color
    }
    SubShader
    {
        //Define our tags, so our shader has some preset values
        Tags
        {
            //Opaque texture
            "RenderType" = "Opaque"
        }
        LOD 200
        CGPROGRAM
        //Define the type of shader we're using (surface shader w/ CSLambert)
        #pragma surface surf CSLambert

        //Bind our variables from properties or scripts to our shader.
        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _ShadowColor;

        //Take our input (In this case just UVs)
        struct Input
        {
            float2 uv_MainTex;
        };

        //Define our lambert lighting
        half4 LightingCSLambert(SurfaceOutput s, half3
                                lightDir, half atten)
        {
            fixed diff = max(0, dot(s.Normal, lightDir));
            half4 c;
            //Set the objects color W/ lighting
            c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 0.5);
            //Set the shadows color
            c.rgb += _ShadowColor.rgb * (1.0 - atten);
            c.a = s.Alpha;
            return c;
        }

        //Define our surface ssader
        void surf(Input IN, inout SurfaceOutput o)
        {
            //Set our color to our base texture multiplied by an additional color property
            half4 c = tex2D(_MainTex, IN.uv_MainTex) *
                _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
        Pass // Define a second pass for outlines
        {
            Cull Front // CULL THE FRONT! So that the outline isn't visible in the front (Blocking the texture)
            CGPROGRAM
            //Access vertex and fragment shaders
            #pragma vertex vert
            #pragma fragment frag

            //CGinc for heavy lifting
            #include "UnityCG.cginc"

            //Take app information (Positon of object and normals of object)
            struct appdata
            {
                float4 vertex:POSITION;
                float3 normal: NORMAL;
            };

            //Vertex 2 Fragment as halfway to pass through values 
            struct v2f
            {
                float4 pos: SV_POSITION;
                fixed4 color: COLOR;
            };

            float _Outline;
            fixed4 _OutlineColor;

            //in vertex buffer
            v2f vert(appdata v)
            {
                v2f o;

                //Pretty much make a duplicate of our object (With extended verticies,)
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                float2 offset = TransformViewToProjection(norm.xy);

                o.pos.xy += offset * o.pos.z * _Outline;
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag(v2f i):SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}