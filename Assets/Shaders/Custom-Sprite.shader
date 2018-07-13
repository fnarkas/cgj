// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/SpritesDefault"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }
    // float4 fragment(v2f_img i) : SV_Target 
    // {
    //     float4 glitch = tex2D(_NoiseTex, i.uv);

    //     float thresh = 1.001 - _Intensity * 1.001;
    //     float w_d = step(thresh, pow(glitch.z, 2.5)); // displacement glitch
    //     float w_f = step(thresh, pow(glitch.w, 2.5)); // frame glitch
    //     float w_c = step(thresh, pow(glitch.z, 3.5)); // color glitch

    //     // Displacement.
    //     float2 uv = frac(i.uv + glitch.xy * w_d);
    //     float4 source = tex2D(_MainTex, uv);

    //     // Mix with trash frame.
    //     float3 color = lerp(source, tex2D(_TrashTex, uv), w_f).rgb;

    //     // Shuffle color components.
    //     float3 neg = saturate(color.grb + (1 - dot(color, 1)) * 0.5);
    //     color = lerp(color, neg, w_c);

    //     return float4(source.rgb, source.a);
    // }

    // float4 SpriteFrag(v2f_img i) : SV_Target{
        // return source.rgba;
    // }


    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
        ENDCG
        }
    }
}
