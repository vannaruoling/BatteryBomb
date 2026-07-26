Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Range(0, 8)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _OutlineColor;
            float _OutlineThickness;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);

                // inside the sprite: draw nothing, keeps the interior see-through
                if (c.a > 0.1) return fixed4(0, 0, 0, 0);

                float2 t = _MainTex_TexelSize.xy * _OutlineThickness;

                float a = 0;
                a = max(a, tex2D(_MainTex, i.uv + float2( t.x, 0)).a);
                a = max(a, tex2D(_MainTex, i.uv + float2(-t.x, 0)).a);
                a = max(a, tex2D(_MainTex, i.uv + float2(0,  t.y)).a);
                a = max(a, tex2D(_MainTex, i.uv + float2(0, -t.y)).a);
                a = max(a, tex2D(_MainTex, i.uv + float2( t.x,  t.y)).a);
                a = max(a, tex2D(_MainTex, i.uv + float2(-t.x,  t.y)).a);
                a = max(a, tex2D(_MainTex, i.uv + float2( t.x, -t.y)).a);
                a = max(a, tex2D(_MainTex, i.uv + float2(-t.x, -t.y)).a);

                if (a > 0.1) return _OutlineColor;
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}