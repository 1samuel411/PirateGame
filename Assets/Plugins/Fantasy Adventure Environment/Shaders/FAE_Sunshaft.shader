// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33149,y:32718,varname:node_4013,prsc:2|custl-3843-RGB,alpha-7024-OUT;n:type:ShaderForge.SFN_ViewPosition,id:3224,x:31091,y:33121,varname:node_3224,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2086,x:31950,y:33199,varname:node_2086,prsc:2|A-8491-OUT,B-5805-OUT;n:type:ShaderForge.SFN_Divide,id:5805,x:31622,y:33257,varname:node_5805,prsc:2|A-9639-OUT,B-3718-OUT;n:type:ShaderForge.SFN_Slider,id:3718,x:31118,y:33476,ptovrint:False,ptlb:Fade distance,ptin:_Fadedistance,varname:_Fadedistance,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:150;n:type:ShaderForge.SFN_DepthBlend,id:652,x:32234,y:33288,varname:node_652,prsc:2|DIST-1319-OUT;n:type:ShaderForge.SFN_Slider,id:1319,x:31847,y:33451,ptovrint:False,ptlb:Intersection fade,ptin:_Intersectionfade,varname:_Intersectionfade,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:10,max:20;n:type:ShaderForge.SFN_Multiply,id:1109,x:32471,y:33115,varname:node_1109,prsc:2|A-2086-OUT,B-652-OUT;n:type:ShaderForge.SFN_LightColor,id:3843,x:32209,y:32876,varname:node_3843,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9840,x:32688,y:33010,varname:node_9840,prsc:2|A-3843-A,B-1109-OUT;n:type:ShaderForge.SFN_Distance,id:9639,x:31351,y:33223,varname:node_9639,prsc:2|A-3224-XYZ,B-7916-XYZ;n:type:ShaderForge.SFN_FragmentPosition,id:7916,x:31105,y:33275,varname:node_7916,prsc:2;n:type:ShaderForge.SFN_Clamp01,id:7024,x:32876,y:33010,varname:node_7024,prsc:2|IN-9840-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:6743,x:30142,y:32758,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:_Alpha,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9afc9fffc8d5eb04fbf46f8da66a5439,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8070,x:30384,y:33365,varname:_node_8070,prsc:2,tex:9afc9fffc8d5eb04fbf46f8da66a5439,ntxv:0,isnm:False|TEX-6743-TEX;n:type:ShaderForge.SFN_Tex2d,id:3381,x:30384,y:32987,varname:_SunShaftA,prsc:2,tex:9afc9fffc8d5eb04fbf46f8da66a5439,ntxv:0,isnm:False|UVIN-9752-UVOUT,TEX-6743-TEX;n:type:ShaderForge.SFN_Tex2d,id:6322,x:30384,y:33180,varname:_SunShaftB,prsc:2,tex:9afc9fffc8d5eb04fbf46f8da66a5439,ntxv:0,isnm:False|UVIN-3508-UVOUT,TEX-6743-TEX;n:type:ShaderForge.SFN_Time,id:9770,x:29653,y:32954,varname:node_9770,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9878,x:29923,y:33012,varname:node_9878,prsc:2|A-9770-TSL,B-1180-OUT;n:type:ShaderForge.SFN_Slider,id:1180,x:29496,y:33162,ptovrint:False,ptlb:PanningSpeed,ptin:_PanningSpeed,varname:_PanningSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.33,max:1;n:type:ShaderForge.SFN_Panner,id:9752,x:30142,y:32987,varname:node_9752,prsc:2,spu:1,spv:0|UVIN-2620-UVOUT,DIST-9878-OUT;n:type:ShaderForge.SFN_TexCoord,id:2620,x:29923,y:33199,varname:node_2620,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:3508,x:30142,y:33180,varname:node_3508,prsc:2,spu:-1,spv:0|UVIN-2620-UVOUT,DIST-9878-OUT;n:type:ShaderForge.SFN_Multiply,id:9472,x:30601,y:33105,varname:node_9472,prsc:2|A-3381-R,B-6322-G;n:type:ShaderForge.SFN_Lerp,id:8491,x:30836,y:33105,varname:node_8491,prsc:2|A-4964-OUT,B-9472-OUT,T-8070-A;n:type:ShaderForge.SFN_Vector1,id:4964,x:30601,y:33041,varname:node_4964,prsc:2,v1:0;proporder:3718-1319-6743-1180;pass:END;sub:END;*/

Shader "FAE/Sunshaft" {
    Properties {
        _Fadedistance ("Fade distance", Range(0, 150)) = 1
        _Intersectionfade ("Intersection fade", Range(0, 20)) = 10
        _Alpha ("Alpha", 2D) = "white" {}
        _PanningSpeed ("PanningSpeed", Range(0, 1)) = 0.33
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
            uniform float4 _TimeEditor;
            uniform float _Fadedistance;
            uniform float _Intersectionfade;
            uniform sampler2D _Alpha; uniform float4 _Alpha_ST;
            uniform float _PanningSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 projPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
                float3 finalColor = _LightColor0.rgb;
                float4 node_9770 = _Time + _TimeEditor;
                float node_9878 = (node_9770.r*_PanningSpeed);
                float2 node_9752 = (i.uv0+node_9878*float2(1,0));
                float4 _SunShaftA = tex2D(_Alpha,TRANSFORM_TEX(node_9752, _Alpha));
                float2 node_3508 = (i.uv0+node_9878*float2(-1,0));
                float4 _SunShaftB = tex2D(_Alpha,TRANSFORM_TEX(node_3508, _Alpha));
                float4 _node_8070 = tex2D(_Alpha,TRANSFORM_TEX(i.uv0, _Alpha));
                fixed4 finalRGBA = fixed4(finalColor,saturate((_LightColor0.a*((lerp(0.0,(_SunShaftA.r*_SunShaftB.g),_node_8070.a)*(distance(_WorldSpaceCameraPos,i.posWorld.rgb)/_Fadedistance))*saturate((sceneZ-partZ)/_Intersectionfade)))));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
