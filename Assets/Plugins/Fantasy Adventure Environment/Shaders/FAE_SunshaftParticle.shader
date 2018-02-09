// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33149,y:32718,varname:node_4013,prsc:2|custl-3428-OUT,alpha-7024-OUT;n:type:ShaderForge.SFN_Tex2d,id:2444,x:31373,y:32929,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:_Alpha,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:True,tagnrm:False,tex:9afc9fffc8d5eb04fbf46f8da66a5439,ntxv:0,isnm:False;n:type:ShaderForge.SFN_VertexColor,id:6072,x:31373,y:32739,varname:node_6072,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3914,x:31647,y:32913,varname:node_3914,prsc:2|A-6072-A,B-2444-A;n:type:ShaderForge.SFN_ViewPosition,id:3224,x:31091,y:33121,varname:node_3224,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2086,x:31950,y:33199,varname:node_2086,prsc:2|A-3914-OUT,B-5805-OUT;n:type:ShaderForge.SFN_Divide,id:5805,x:31622,y:33257,varname:node_5805,prsc:2|A-9639-OUT,B-3718-OUT;n:type:ShaderForge.SFN_Slider,id:3718,x:31118,y:33476,ptovrint:False,ptlb:Fade distance,ptin:_Fadedistance,varname:_Fadedistance,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:150;n:type:ShaderForge.SFN_DepthBlend,id:652,x:32234,y:33288,varname:node_652,prsc:2|DIST-1319-OUT;n:type:ShaderForge.SFN_Slider,id:1319,x:31847,y:33451,ptovrint:False,ptlb:Intersection fade,ptin:_Intersectionfade,varname:_Intersectionfade,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:10,max:20;n:type:ShaderForge.SFN_Multiply,id:1109,x:32471,y:33115,varname:node_1109,prsc:2|A-5130-OUT,B-652-OUT;n:type:ShaderForge.SFN_LightColor,id:3843,x:32209,y:32876,varname:node_3843,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3428,x:32525,y:32731,varname:node_3428,prsc:2|A-6072-RGB,B-3843-RGB;n:type:ShaderForge.SFN_Multiply,id:9840,x:32688,y:33010,varname:node_9840,prsc:2|A-3843-A,B-1109-OUT;n:type:ShaderForge.SFN_Multiply,id:5130,x:32146,y:33105,varname:node_5130,prsc:2|A-3914-OUT,B-2086-OUT;n:type:ShaderForge.SFN_Distance,id:9639,x:31351,y:33223,varname:node_9639,prsc:2|A-3224-XYZ,B-7916-XYZ;n:type:ShaderForge.SFN_FragmentPosition,id:7916,x:31105,y:33275,varname:node_7916,prsc:2;n:type:ShaderForge.SFN_Clamp01,id:7024,x:32876,y:33010,varname:node_7024,prsc:2|IN-9840-OUT;proporder:2444-3718-1319;pass:END;sub:END;*/

Shader "FAE/Sunshaft particle" {
    Properties {
        [NoScaleOffset]_Alpha ("Alpha", 2D) = "white" {}
        _Fadedistance ("Fade distance", Range(0, 150)) = 1
        _Intersectionfade ("Intersection fade", Range(0, 20)) = 10
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _Alpha;
            uniform float _Fadedistance;
            uniform float _Intersectionfade;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float3 finalColor = (i.vertexColor.rgb*_LightColor0.rgb);
                float4 _Alpha_var = tex2D(_Alpha,i.uv0);
                float node_3914 = (i.vertexColor.a*_Alpha_var.a);
                fixed4 finalRGBA = fixed4(finalColor,saturate((_LightColor0.a*((node_3914*(node_3914*(distance(_WorldSpaceCameraPos,i.posWorld.rgb)/_Fadedistance)))*saturate((sceneZ-partZ)/_Intersectionfade)))));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
