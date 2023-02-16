// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/TetrahedronShader"
{
   Properties{}

   SubShader{
      Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
      ZWrite On
      //Blend SrcColor OneMinusSrcAlpha
      //Blend SrcAlpha OneMinusSrcAlpha
       //Blend One OneMinusSrcAlpha
       Blend One One
       //Blend OneMinusDstColor One
       //Blend DstColor Zero
       //Blend DstColor SrcColor
      Cull Back
      LOD 100

    Pass {
      CGPROGRAM
      #pragma vertex vert alpha
      #pragma fragment frag alpha
      #include "UnityCG.cginc"

      struct v2f {
          float4 pos : SV_POSITION;
          fixed4 color : COLOR;
      };

      v2f vert(appdata_base v)
      {
          v2f o;
          o.pos = UnityObjectToClipPos(v.vertex);
          o.color.xyz = v.normal;
          o.color.w = 0.3;
          return o;
      }

      fixed4 frag(v2f i) : SV_Target { return i.color; }
      ENDCG
    }
   }
}
