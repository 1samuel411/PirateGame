// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "StylizedWater/BETA/Desktop"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[HideInInspector] _DummyTex( "", 2D ) = "white" {}
		_MaskClipValue( "Mask Clip Value", Float ) = 0.5
		_WaterColor("Water Color", Color) = (0.1176471,0.6348885,1,0)
		_WaterShallowColor("WaterShallowColor", Color) = (0.4191176,0.7596349,1,0)
		_FresnelColor("Fresnel Color", Color) = (1,1,1,0.484)
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_NormalStrength("NormalStrength", Range( 0 , 1)) = 1
		_Transparency("Transparency", Range( 0 , 1)) = 0.75
		_Glossiness("Glossiness", Range( 0 , 1)) = 1
		_Fresnelexponent("Fresnel exponent", Float) = 4
		_Reflection("Reflection", Range( 0 , 1)) = 0
		[NoScaleOffset][Normal]_Normals("Normals", 2D) = "white" {}
		_RefractionAmount("Refraction Amount", Range( 0 , 0.1)) = 0
		_Worldspacetiling("Worldspace tiling", Float) = 1
		_Tiling("Tiling", Range( 0 , 1)) = 0.9
		[NoScaleOffset]_Shadermap("Shadermap", 2D) = "white" {}
		_RimDistance("Rim Distance", Range( 0.01 , 3)) = 0.2448298
		_RimSize("Rim Size", Range( 0 , 20)) = 0
		_Rimfalloff("Rim falloff", Range( 0.1 , 50)) = 0
		_Rimtiling("Rim tiling", Float) = 2
		_SurfaceHighlight("Surface Highlight", Range( 0 , 1)) = 0.05
		_UseIntersectionHighlight("UseIntersectionHighlight", Float) = 0
		_HighlightPanning("HighlightPanning", Float) = 0
		_Surfacehightlightsize("Surface hightlight size", Float) = 0
		_SurfaceHightlighttiling("Surface Hightlight tiling", Float) = 0.25
		_Depth("Depth", Range( 0 , 100)) = 30
		_Wavesspeed("Waves speed", Range( 0 , 10)) = 0.75
		_Wavesstrength("Waves strength", Range( 0 , 1)) = 0
		_Tessellation("Tessellation", Range( 0.1 , 100)) = 0.1
		_Wavetint("Wave tint", Range( -1 , 1)) = 0
		_Wavefoam("Wave foam", Range( 0 , 10)) = 0
		_WaveSize("Wave Size", Range( 0 , 10)) = 0.5
		_WaveDirection("WaveDirection", Vector) = (0,0,0,0)
		[HideInInspector]_ReflectionTex("ReflectionTex", 2D) = "gray" {}
		[Toggle]_Unlit("Unlit", Float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		LOD 200
		Cull Back
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		BlendOp Add
		GrabPass{ "GrabScreen0" }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
		#pragma surface surf Standard keepalpha noshadow exclude_path:deferred nolightmap  nodirlightmap vertex:vertexDataFunc tessellate:tessFunction nolightmap 
		struct Input
		{
			float2 uv_DummyTex;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		uniform sampler2D _Normals;
		uniform float _Worldspacetiling;
		uniform sampler2D _DummyTex;
		uniform float _Tiling;
		uniform float _Wavesspeed;
		uniform float4 _WaveDirection;
		uniform float _SurfaceHighlight;
		uniform float _UseIntersectionHighlight;
		uniform float _Surfacehightlightsize;
		uniform sampler2D _Shadermap;
		uniform float _SurfaceHightlighttiling;
		uniform float _HighlightPanning;
		uniform float4 _RimColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Rimfalloff;
		uniform float _Rimtiling;
		uniform float _RimSize;
		uniform float _NormalStrength;
		uniform float _Unlit;
		uniform sampler2D GrabScreen0;
		uniform float _RefractionAmount;
		uniform float4 _WaterShallowColor;
		uniform float4 _WaterColor;
		uniform float _Depth;
		uniform float _Transparency;
		uniform sampler2D _ReflectionTex;
		uniform float _Reflection;
		uniform float _WaveSize;
		uniform float _Wavetint;
		uniform float4 _FresnelColor;
		uniform float _Fresnelexponent;
		uniform float _Wavefoam;
		uniform float _Glossiness;
		uniform float _RimDistance;
		uniform float _Wavesstrength;
		uniform float _Tessellation;
		uniform float _MaskClipValue = 0.5;

		float4 tessFunction( appdata v0, appdata v1, appdata v2 )
		{
			float4 temp_cast_8 = (_Tessellation).xxxx;
			return temp_cast_8;
		}

		void vertexDataFunc( inout appdata v )
		{
			v.texcoord.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float2 temp_cast_0 = (-20.0).xx;
			float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex);
			float2 temp_cast_1 = (( 1.0 - _Tiling )).xx;
			float2 Tiling = ( lerp(( temp_cast_0 * v.texcoord.xy ),(ase_worldPos).xz,_Worldspacetiling) * temp_cast_1 );
			float2 temp_cast_2 = (_WaveSize).xx;
			float2 temp_cast_3 = (0.1).xx;
			float2 temp_cast_4 = (( ( _Wavesspeed * 0.1 ) * _Time.y )).xx;
			float2 appendResult336 = float2( _WaveDirection.x , _WaveDirection.z );
			float2 WaveSpeed = ( temp_cast_4 * appendResult336 );
			float2 temp_cast_5 = (0.5).xx;
			float4 tex2DNode94 = tex2Dlod( _Shadermap, float4( ( ( ( Tiling * temp_cast_2 ) * temp_cast_3 ) + ( WaveSpeed * temp_cast_5 ) ), 0.0 , 0.0 ) );
			float temp_output_95_0 = ( _Wavesstrength * tex2DNode94.g );
			float3 temp_cast_7 = (temp_output_95_0).xxx;
			float3 Displacement = ( v.normal * temp_cast_7 );
			v.vertex.xyz += Displacement;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (-20.0).xx;
			float2 texCoordDummy12 = i.uv_DummyTex*float2( 1,1 ) + float2( 0,0 );
			float3 ase_worldPos = i.worldPos;
			float2 temp_cast_1 = (( 1.0 - _Tiling )).xx;
			float2 Tiling = ( lerp(( temp_cast_0 * texCoordDummy12 ),(ase_worldPos).xz,_Worldspacetiling) * temp_cast_1 );
			float2 temp_cast_2 = (( ( _Wavesspeed * 0.1 ) * _Time.y )).xx;
			float2 appendResult336 = float2( _WaveDirection.x , _WaveDirection.z );
			float2 WaveSpeed = ( temp_cast_2 * appendResult336 );
			float2 temp_output_341_0 = ( Tiling + WaveSpeed );
			float2 temp_cast_3 = (0.5).xx;
			float2 temp_output_339_0 = ( ( Tiling * temp_cast_3 ) + -WaveSpeed );
			float3 temp_output_51_0 = BlendNormals( UnpackNormal( tex2D( _Normals, temp_output_341_0 ) ) , UnpackNormal( tex2D( _Normals, temp_output_339_0 ) ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float2 temp_cast_4 = (_SurfaceHightlighttiling).xx;
			float4 tex2DNode66 = tex2D( _Shadermap, ( temp_output_341_0 * temp_cast_4 ) );
			float2 temp_cast_5 = (_SurfaceHightlighttiling).xx;
			float2 temp_cast_6 = (0.5).xx;
			float4 tex2DNode67 = tex2D( _Shadermap, ( temp_cast_5 * temp_output_339_0 ) );
			float SurfaceHighlights = ( _SurfaceHighlight * lerp(step( _Surfacehightlightsize , ( tex2DNode66.r - tex2DNode67.r ) ),( 1.0 - lerp(tex2DNode67.r,( tex2DNode66.r * tex2DNode67.r ),_HighlightPanning) ),_UseIntersectionHighlight) );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float screenDepth402 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth402 = abs( ( screenDepth402 - LinearEyeDepth( ase_screenPos.z/ ase_screenPos.w ) ) / _Rimfalloff );
			float2 temp_cast_7 = (0.5).xx;
			float2 temp_cast_8 = (_Rimtiling).xx;
			float2 temp_output_24_0 = ( Tiling * temp_cast_8 );
			float2 temp_cast_9 = (_Rimtiling).xx;
			float temp_output_30_0 = ( tex2D( _Shadermap, ( ( temp_cast_7 * temp_output_24_0 ) + WaveSpeed ) ).b * tex2D( _Shadermap, ( temp_output_24_0 + ( 1.0 - WaveSpeed ) ) ).b );
			float screenDepth4 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth4 = abs( ( screenDepth4 - LinearEyeDepth( ase_screenPos.z/ ase_screenPos.w ) ) / _RimSize );
			float Intersection = clamp( ( _RimColor.a * ( 1.0 - ( ( distanceDepth402 * temp_output_30_0 * 3.0 ) + distanceDepth4 ) ) ) , 0.0 , 1.0 );
			float3 temp_cast_10 = (_NormalStrength).xxx;
			float3 NormalsFinal = ( lerp( temp_output_51_0 , vertexNormal , ( SurfaceHighlights + Intersection ) ) * temp_cast_10 );
			o.Normal = NormalsFinal;
			float4 temp_cast_11 = (( 1.0 - _Unlit )).xxxx;
			float3 temp_cast_12 = (_RefractionAmount).xxx;
			float2 temp_cast_13 = (0.5).xx;
			float3 NormalsBlended = temp_output_51_0;
			float4 ase_screenPos266 = ase_screenPos;
			#if UNITY_UV_STARTS_AT_TOP
			float scale266 = -1.0;
			#else
			float scale266 = 1.0;
			#endif
			float halfPosW266 = ase_screenPos266.w * 0.5;
			ase_screenPos266.y = ( ase_screenPos266.y - halfPosW266 ) * _ProjectionParams.x* scale266 + halfPosW266;
			#ifdef UNITY_SINGLE_PASS_STEREO
			ase_screenPos266.xy = TransformStereoScreenSpaceTex(ase_screenPos266.xy, ase_screenPos266.w);
			#endif
			ase_screenPos266.xyzw /= ase_screenPos266.w;
			float2 appendResult263 = float2( ase_screenPos266.r , ase_screenPos266.g );
			float3 temp_output_359_0 = ( ( temp_cast_12 * NormalsBlended ) + float3( appendResult263 ,  0.0 ) );
			float4 screenColor372 = tex2D( GrabScreen0, temp_output_359_0.xy );
			float4 RefractionResult = screenColor372;
			float screenDepth105 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth105 = abs( ( screenDepth105 - LinearEyeDepth( ase_screenPos.z/ ase_screenPos.w ) ) / _Depth );
			float Depth = clamp( distanceDepth105 , 0.0 , 1.0 );
			float Opacity = clamp( ( ( _Transparency + Intersection ) - ( ( 1.0 - Depth ) * ( 1.0 - _WaterShallowColor.a ) ) ) , 0.0 , 1.0 );
			float3 temp_cast_16 = (_RefractionAmount).xxx;
			float4 Reflection = tex2D( _ReflectionTex, temp_output_359_0.xy );
			float4 WaterColor = lerp( lerp( RefractionResult , lerp( _WaterShallowColor , _WaterColor , Depth ) , Opacity ) , Reflection , ( Opacity * _Reflection ) );
			float2 temp_cast_19 = (_WaveSize).xx;
			float2 temp_cast_20 = (0.1).xx;
			float2 temp_cast_21 = (0.5).xx;
			float4 tex2DNode94 = tex2D( _Shadermap, ( ( ( Tiling * temp_cast_19 ) * temp_cast_20 ) + ( WaveSpeed * temp_cast_21 ) ) );
			float Heightmap = tex2DNode94.g;
			float4 temp_cast_22 = (( Heightmap * _Wavetint )).xxxx;
			float4 RimColor = _RimColor;
			float4 temp_cast_23 = (3.0).xxxx;
			float4 temp_cast_24 = (1.0).xxxx;
			float4 FresnelColor = _FresnelColor;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float fresnelFinalVal199 = (0.0 + 1.0*pow( 1.0 - dot( vertexNormal, worldViewDir ) , ( _Fresnelexponent * 100.0 )));
			float Fresnel = ( _FresnelColor.a * fresnelFinalVal199 );
			float4 temp_cast_25 = (1.0).xxxx;
			float SurfaceHighlightTex = lerp(step( _Surfacehightlightsize , ( tex2DNode66.r - tex2DNode67.r ) ),( 1.0 - lerp(tex2DNode67.r,( tex2DNode66.r * tex2DNode67.r ),_HighlightPanning) ),_UseIntersectionHighlight);
			float WaveFoam = clamp( ( pow( ( tex2DNode94.g * _Wavefoam ) , 2.0 ) * SurfaceHighlightTex ) , 0.0 , 1.0 );
			float4 temp_output_223_0 = lerp( lerp( lerp( lerp( ( WaterColor - temp_cast_22 ) , ( RimColor * temp_cast_23 ) , Intersection ) , temp_cast_24 , SurfaceHighlights ) , FresnelColor , Fresnel ) , temp_cast_25 , WaveFoam );
			float4 FinalColor = temp_output_223_0;
			o.Albedo = ( temp_cast_11 * FinalColor ).rgb;
			float4 temp_cast_27 = (_Unlit).xxxx;
			o.Emission = ( temp_cast_27 * FinalColor ).rgb;
			o.Smoothness = _Glossiness;
			float screenDepth1 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth1 = abs( ( screenDepth1 - LinearEyeDepth( ase_screenPos.z/ ase_screenPos.w ) ) / _RimDistance );
			o.Alpha = distanceDepth1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=10008
7;29;1906;1004;4105.53;3858.83;2.8;True;False
Node;AmplifyShaderEditor.CommentaryNode;347;-5386.759,-1596.891;Float;False;1730.402;709.7;Comment;10;12;16;17;18;13;20;15;19;21;14;UV;0;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-5298.959,-1546.891;Float;False;Constant;_Float0;Float 0;4;0;-20;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-5336.759,-1420.591;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;348;-5365.659,-516.9902;Float;False;1494.001;706.4609;Comment;9;37;35;38;320;36;39;336;337;40;Speed/direction;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;16;-5308.559,-1216.891;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;17;-5041.56,-1187.891;Float;False;FLOAT2;0;2;2;2;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;37;-5222.758,-351.2891;Float;False;Constant;_Float1;Float 1;9;0;0.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;18;-5282.958,-1002.19;Float;False;Property;_Tiling;Tiling;12;0;0.9;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;35;-5315.659,-466.9902;Float;False;Property;_Wavesspeed;Waves speed;24;0;0.75;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-5044.56,-1450.29;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.ToggleSwitchNode;15;-4754.56,-1296.891;Float;False;Property;_Worldspacetiling;Worldspace tiling;11;0;1;2;0;FLOAT2;0.0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.TimeNode;38;-5136.958,-230.29;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector4Node;320;-5110.518,-17.52931;Float;False;Property;_WaveDirection;WaveDirection;30;0;0,0,0,0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-4992.657,-420.189;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;20;-4867.558,-1031.79;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-4715.158,-331.1895;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.AppendNode;336;-4849.521,22.37057;Float;False;FLOAT2;0;0;0;0;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-4504.56,-1124.192;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;349;-3020.035,-188.2218;Float;False;3325.803;1007.284;Comment;27;22;23;24;41;29;3;28;4;5;30;102;233;222;42;353;354;355;356;402;420;425;426;10;438;440;444;470;Intersection;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-2895.216,310.4862;Float;False;21;0;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;337;-4508.721,-195.2295;Float;False;2;2;0;FLOAT;0.0,0;False;1;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-3899.358,-1123.991;Float;False;Tiling;-1;True;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;23;-2891.404,412.9265;Float;False;Property;_Rimtiling;Rim tiling;16;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;355;-2660.632,221.5706;Float;False;Constant;_Float13;Float 13;34;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-2632.98,351.8964;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.GetLocalVarNode;41;-2795.532,624.812;Float;False;40;0;1;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-4176.856,-194.9893;Float;False;WaveSpeed;-1;True;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.OneMinusNode;470;-2406.911,625.9425;Float;False;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;354;-2400.932,253.1705;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;356;-2116.431,600.6707;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;353;-2114.631,408.9705;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;28;-1926.505,393.3322;Float;True;Property;_TextureSample0;Texture Sample 0;7;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-1923.202,110.9952;Float;False;Property;_Rimfalloff;Rim falloff;15;0;0;0.1;50;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;29;-1924.119,589.0623;Float;True;Property;_TextureSample1;Texture Sample 1;23;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;381;-4535.249,1990.391;Float;False;1019.923;590.8805;Comment;7;46;47;343;342;340;339;341;Cross panning UV;0;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1944.149,258.9192;Float;False;Property;_RimSize;Rim Size;14;0;0;0;20;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;440;-1419.308,363.4426;Float;False;Constant;_Float6;Float 6;33;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1545.278,548.1219;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DepthFade;402;-1545.706,124.8471;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;47;-4485.25,2196.391;Float;False;40;0;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;343;-4132.526,2466.271;Float;False;Constant;_Float12;Float 12;34;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;420;-1215.208,180.6434;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DepthFade;4;-1575.091,279.7891;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;46;-4242.85,2040.391;Float;False;21;0;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;425;-959.9078,276.8434;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;342;-3900.328,2250.271;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;104;-2316.36,-3817.686;Float;False;Property;_Depth;Depth;22;0;30;0;100;0;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;340;-4149.728,2331.271;Float;False;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.ColorNode;10;-908.9308,-146.4158;Float;False;Property;_RimColor;Rim Color;4;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;339;-3675.531,2281.672;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;341;-3669.329,2140.572;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.DepthFade;105;-1977.834,-3821.543;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;426;-764.6081,270.8434;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;380;-2383.622,-3473.776;Float;False;1667.978;443.2719;Comment;10;121;133;149;151;134;119;117;480;487;488;Opacity;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;444;-341.0081,69.84277;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;45;-2653.052,2352.991;Float;True;Property;_TextureSample2;Texture Sample 2;10;0;None;True;0;False;white;Auto;True;Instance;43;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;50;-2655.848,2579.889;Float;True;Property;_TextureSample3;Texture Sample 3;24;0;None;True;0;False;white;Auto;True;Instance;43;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;144;-1711.533,-3859.979;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;480;-2350.512,-3118.96;Float;False;479;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;382;-2980.637,-1888.427;Float;False;1565.489;541.3926;Comment;10;269;360;266;361;263;359;260;265;126;372;Reflection/Refraction;0;0
Node;AmplifyShaderEditor.ColorNode;477;-2440.313,-2853.259;Float;False;Property;_WaterShallowColor;WaterShallowColor;2;0;0.4191176,0.7596349,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;479;-1430.411,-3848.06;Float;False;Depth;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;438;-154.709,-54.85733;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;51;-2211.447,2465.09;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;117;-2333.622,-3423.776;Float;False;Property;_Transparency;Transparency;6;0;0.75;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;42;58.76928,-3.012741;Float;False;Intersection;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;362;-1938.641,2394.769;Float;False;NormalsBlended;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.GrabScreenPosition;266;-2928.616,-1554.035;Float;False;0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;311;-3027.042,1265.19;Float;False;2806.331;669.9998;Comment;16;63;64;65;67;66;74;69;75;68;76;70;77;72;73;244;71;Surface highlights;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;269;-2909.617,-1649.332;Float;False;362;0;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;119;-2283.998,-3309.402;Float;False;42;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;407;-3022.625,3076.09;Float;False;2762.213;843.6829;Comment;25;344;94;92;91;90;87;302;89;301;88;220;219;232;97;98;100;96;95;218;99;231;230;229;401;221;Waves;0;0
Node;AmplifyShaderEditor.OneMinusNode;151;-2099.962,-3222;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;488;-2101.708,-3113.859;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;360;-2930.637,-1766.433;Float;False;Property;_RefractionAmount;Refraction Amount;10;0;0;0;0.1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;302;-2972.625,3343.579;Float;False;Property;_WaveSize;Wave Size;29;0;0.5;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;361;-2583.436,-1749.433;Float;False;2;2;0;FLOAT;0,0,0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;134;-1862.436,-3411.029;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;487;-1787.109,-3195.259;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;87;-2866.145,3226.497;Float;False;21;0;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;63;-2977.042,1579.29;Float;False;Property;_SurfaceHightlighttiling;Surface Hightlight tiling;21;0;0.25;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.AppendNode;263;-2683.617,-1526.834;Float;False;FLOAT2;0;0;0;0;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;149;-1606.138,-3410.348;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;359;-2345.136,-1679.332;Float;False;2;2;0;FLOAT3;0.0,0,0;False;1;FLOAT2;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;89;-2603.592,3428.811;Float;False;Constant;_Float4;Float 4;16;0;0.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-2635.642,1609.79;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-2646.841,1716.89;Float;False;2;2;0;FLOAT;0.0,0;False;1;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;301;-2600.625,3277.579;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;91;-2602.742,3727.092;Float;False;Constant;_Float5;Float 5;16;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;90;-2614.742,3559.092;Float;False;40;0;1;FLOAT2
Node;AmplifyShaderEditor.ScreenColorNode;372;-2019.626,-1838.427;Float;False;Global;GrabScreen0;Grab Screen 0;34;0;Object;-1;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;66;-2373.84,1495.89;Float;True;Property;_TextureSample4;Texture Sample 4;12;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;60;-2429.441,-2657.682;Float;False;Property;_WaterColor;Water Color;1;0;0.1176471,0.6348885,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;482;-2405.114,-2482.561;Float;False;479;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-2387.947,3346.796;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-2358.742,3582.092;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ClampOpNode;133;-1196.159,-3411.45;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;67;-2381.641,1705.19;Float;True;Property;_TextureSample5;Texture Sample 5;25;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;478;-2023.41,-2734.959;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;344;-2130.825,3469.473;Float;False;2;2;0;FLOAT2;0.0,0;False;1;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;126;-1691.149,-1825.214;Float;False;RefractionResult;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;121;-993.4441,-3410.152;Float;False;Opacity;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;367;-2075.343,-2844.358;Float;False;126;0;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;377;-2077.928,-2514.251;Float;False;121;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;260;-2067.917,-1583.235;Float;True;Property;_ReflectionTex;ReflectionTex;33;1;[HideInInspector];None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1937.643,1777.091;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;271;-2056.793,-2355.304;Float;False;Property;_Reflection;Reflection;9;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;374;-1710.825,-2767.75;Float;False;3;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;265;-1669.316,-1569.436;Float;False;Reflection;-1;True;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;456;-1676.105,-2493.457;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;378;-3003.834,-1141.097;Float;False;1284.181;563.468;Comment;10;213;201;211;212;200;199;202;203;205;206;Fresnel;0;0
Node;AmplifyShaderEditor.CommentaryNode;352;-3052.353,4240.27;Float;False;3101.101;422.6461;Comment;21;112;110;111;141;103;62;80;79;61;210;208;78;315;207;224;223;116;114;351;475;476;Master lerp;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;298;-1754.346,-2602.505;Float;False;265;0;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;94;-1942.843,3444.292;Float;True;Property;_TextureSample6;Texture Sample 6;16;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;75;-1725.044,1677.291;Float;False;Property;_HighlightPanning;HighlightPanning;19;0;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;68;-1951.339,1563.49;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;69;-1985.14,1315.19;Float;False;Property;_Surfacehightlightsize;Surface hightlight size;20;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;201;-2953.834,-789.7988;Float;False;Property;_Fresnelexponent;Fresnel exponent;8;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;213;-2911.306,-692.6296;Float;False;Constant;_Float10;Float 10;26;0;100;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;99;-1408.541,3485.688;Float;False;Heightmap;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;112;-3002.353,4547.916;Float;False;Property;_Wavetint;Wave tint;27;0;0;-1;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;297;-1449.721,-2668.257;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.OneMinusNode;76;-1427.844,1700.091;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;220;-1814.714,3695.973;Float;False;Property;_Wavefoam;Wave foam;28;0;0;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;110;-2977.4,4396.651;Float;False;99;0;1;FLOAT
Node;AmplifyShaderEditor.StepOpNode;70;-1583.439,1432.19;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;351;-2647.53,4290.27;Float;False;350;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;-1459.513,3613.473;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;103;-2328.22,4428.309;Float;False;102;0;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-650.3506,-114.4218;Float;False;RimColor;-1;True;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.ToggleSwitchNode;77;-1170.544,1556.091;Float;False;Property;_UseIntersectionHighlight;UseIntersectionHighlight;18;0;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-2612.288,4385.408;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;212;-2672.507,-742.2288;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;232;-1458.014,3804.772;Float;False;Constant;_Float7;Float 7;25;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;350;-1189.179,-2674.707;Float;False;WaterColor;-1;True;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;476;-2327.409,4526.541;Float;False;Constant;_Float2;Float 2;34;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;211;-2682.107,-839.8298;Float;False;Constant;_Float8;Float 8;26;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;200;-2713.677,-1002.911;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;71;-1155.741,1387.991;Float;False;Property;_SurfaceHighlight;Surface Highlight;17;0;0.05;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;475;-2080.409,4437.541;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleSubtractOpNode;141;-2371.558,4302.319;Float;False;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;202;-2443.337,-1091.098;Float;False;Property;_FresnelColor;Fresnel Color;3;0;1,1,1,0.484;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-780.641,1421.691;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.FresnelNode;199;-2432.007,-868.4578;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;231;-1252.414,3617.073;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;229;-1242.914,3791.573;Float;False;244;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;244;-519.71,1570.168;Float;False;SurfaceHighlightTex;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;62;-2153.24,4574.445;Float;False;42;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;203;-2125.562,-909.3528;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-923.914,3621.772;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;61;-1868.118,4305.361;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-506.8405,1428.191;Float;False;SurfaceHighlights;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;80;-1798.273,4444.555;Float;False;Constant;_Float3;Float 3;16;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;79;-1867.659,4538.541;Float;False;73;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;208;-1407.701,4509.015;Float;False;205;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;205;-1962.654,-915.1938;Float;False;Fresnel;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;210;-1417.476,4423.76;Float;False;206;0;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;384;-1949.427,2863.996;Float;False;42;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;206;-2112.112,-1058.588;Float;False;FresnelColor;-1;True;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;83;-1984.646,2698.189;Float;False;73;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;78;-1574.559,4301.943;Float;False;3;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.ClampOpNode;401;-717.4038,3622.245;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;312;-1701.819,2751.971;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;96;-1617.643,3194.592;Float;False;Property;_Wavesstrength;Waves strength;25;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;363;-1849.341,2547.47;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;315;-939.3166,4437.275;Float;False;Constant;_Float9;Float 9;32;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;221;-503.4121,3621.273;Float;False;WaveFoam;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;207;-1104.577,4309.773;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;224;-973.8135,4537.375;Float;False;221;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;128;-1492.449,2708.49;Float;False;Property;_NormalStrength;NormalStrength;5;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;82;-1372.946,2451.789;Float;False;3;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0,0;False;2;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;223;-713.6131,4304.675;Float;False;3;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1283.044,3319.088;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;97;-1306.842,3126.09;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;458;700.8949,-369.1569;Float;False;Property;_Unlit;Unlit;34;1;[Toggle];0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;2;897.963,9.144379;Float;False;Property;_RimDistance;Rim Distance;13;0;0.2448298;0.01;3;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;460;959.8949,-370.1569;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;115;685.358,-231.2139;Float;False;114;0;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;114;-194.2513,4307.592;Float;False;FinalColor;-1;True;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;466;-1089.909,2455.541;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-965.142,3185.088;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;101;1165.459,102.189;Float;False;100;0;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;43;1474.061,-735.0397;Float;True;Property;_Normals;Normals;31;2;[NoScaleOffset];[Normal];Assets/StylizedWater/StylizedWater.sbsar;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;457;1245.895,-163.1569;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.DepthFade;1;1229.954,16.68527;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;218;-980.01,3324.471;Float;False;WaveHeight;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;233;-1364.415,660.1724;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;56;1115.356,-68.0119;Float;False;Property;_Glossiness;Glossiness;7;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;459;1234.895,-360.1569;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;234;1120.286,191.6716;Float;False;Property;_Tessellation;Tessellation;26;0;0.1;0.1;100;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;222;-1140.413,636.2725;Float;False;IntersectionTex;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;53;1163.457,-243.0103;Float;False;52;0;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-859.0439,2450.59;Float;False;NormalsFinal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;100;-733.541,3180.291;Float;False;Displacement;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;27;1061.068,-736.4829;Float;True;Property;_Shadermap;Shadermap;32;1;[NoScaleOffset];None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;309;-1933.219,2788.171;Float;False;99;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;116;-412.8031,4310.789;Float;False;3;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;106;-2051.297,-2234.298;Float;False;Property;_Depthdarkness;Depth darkness;23;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1616.9,-227;Float;False;True;6;Float;ASEMaterialInspector;200;Standard;StylizedWater/BETA/Desktop;False;False;False;False;False;False;True;False;True;False;False;False;False;False;True;True;Back;1;0;False;0;0;Custom;0.5;True;False;0;True;Transparent;Geometry;ForwardOnly;True;True;True;True;True;True;False;False;False;False;False;False;False;True;True;True;True;False;0;255;255;0;0;0;0;True;2;6;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;Add;Add;1;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;200;;0;-1;-1;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;16;0
WireConnection;13;0;14;0
WireConnection;13;1;12;0
WireConnection;15;0;13;0
WireConnection;15;1;17;0
WireConnection;36;0;35;0
WireConnection;36;1;37;0
WireConnection;20;0;18;0
WireConnection;39;0;36;0
WireConnection;39;1;38;2
WireConnection;336;0;320;1
WireConnection;336;1;320;3
WireConnection;19;0;15;0
WireConnection;19;1;20;0
WireConnection;337;0;39;0
WireConnection;337;1;336;0
WireConnection;21;0;19;0
WireConnection;24;0;22;0
WireConnection;24;1;23;0
WireConnection;40;0;337;0
WireConnection;470;0;41;0
WireConnection;354;0;355;0
WireConnection;354;1;24;0
WireConnection;356;0;24;0
WireConnection;356;1;470;0
WireConnection;353;0;354;0
WireConnection;353;1;41;0
WireConnection;28;1;353;0
WireConnection;29;1;356;0
WireConnection;30;0;28;3
WireConnection;30;1;29;3
WireConnection;402;0;5;0
WireConnection;420;0;402;0
WireConnection;420;1;30;0
WireConnection;420;2;440;0
WireConnection;4;0;3;0
WireConnection;425;0;420;0
WireConnection;425;1;4;0
WireConnection;342;0;46;0
WireConnection;342;1;343;0
WireConnection;340;0;47;0
WireConnection;339;0;342;0
WireConnection;339;1;340;0
WireConnection;341;0;46;0
WireConnection;341;1;47;0
WireConnection;105;0;104;0
WireConnection;426;0;425;0
WireConnection;444;0;10;4
WireConnection;444;1;426;0
WireConnection;45;1;341;0
WireConnection;50;1;339;0
WireConnection;144;0;105;0
WireConnection;479;0;144;0
WireConnection;438;0;444;0
WireConnection;51;0;45;0
WireConnection;51;1;50;0
WireConnection;42;0;438;0
WireConnection;362;0;51;0
WireConnection;151;0;480;0
WireConnection;488;0;477;4
WireConnection;361;0;360;0
WireConnection;361;1;269;0
WireConnection;134;0;117;0
WireConnection;134;1;119;0
WireConnection;487;0;151;0
WireConnection;487;1;488;0
WireConnection;263;0;266;1
WireConnection;263;1;266;2
WireConnection;149;0;134;0
WireConnection;149;1;487;0
WireConnection;359;0;361;0
WireConnection;359;1;263;0
WireConnection;64;0;341;0
WireConnection;64;1;63;0
WireConnection;65;0;63;0
WireConnection;65;1;339;0
WireConnection;301;0;87;0
WireConnection;301;1;302;0
WireConnection;372;0;359;0
WireConnection;66;1;64;0
WireConnection;88;0;301;0
WireConnection;88;1;89;0
WireConnection;92;0;90;0
WireConnection;92;1;91;0
WireConnection;133;0;149;0
WireConnection;67;1;65;0
WireConnection;478;0;477;0
WireConnection;478;1;60;0
WireConnection;478;2;482;0
WireConnection;344;0;88;0
WireConnection;344;1;92;0
WireConnection;126;0;372;0
WireConnection;121;0;133;0
WireConnection;260;1;359;0
WireConnection;74;0;66;1
WireConnection;74;1;67;1
WireConnection;374;0;367;0
WireConnection;374;1;478;0
WireConnection;374;2;377;0
WireConnection;265;0;260;0
WireConnection;456;0;377;0
WireConnection;456;1;271;0
WireConnection;94;1;344;0
WireConnection;75;0;67;1
WireConnection;75;1;74;0
WireConnection;68;0;66;1
WireConnection;68;1;67;1
WireConnection;99;0;94;2
WireConnection;297;0;374;0
WireConnection;297;1;298;0
WireConnection;297;2;456;0
WireConnection;76;0;75;0
WireConnection;70;0;69;0
WireConnection;70;1;68;0
WireConnection;219;0;94;2
WireConnection;219;1;220;0
WireConnection;102;0;10;0
WireConnection;77;0;70;0
WireConnection;77;1;76;0
WireConnection;111;0;110;0
WireConnection;111;1;112;0
WireConnection;212;0;201;0
WireConnection;212;1;213;0
WireConnection;350;0;297;0
WireConnection;475;0;103;0
WireConnection;475;1;476;0
WireConnection;141;0;351;0
WireConnection;141;1;111;0
WireConnection;72;0;71;0
WireConnection;72;1;77;0
WireConnection;199;0;200;0
WireConnection;199;2;211;0
WireConnection;199;3;212;0
WireConnection;231;0;219;0
WireConnection;231;1;232;0
WireConnection;244;0;77;0
WireConnection;203;0;202;4
WireConnection;203;1;199;0
WireConnection;230;0;231;0
WireConnection;230;1;229;0
WireConnection;61;0;141;0
WireConnection;61;1;475;0
WireConnection;61;2;62;0
WireConnection;73;0;72;0
WireConnection;205;0;203;0
WireConnection;206;0;202;0
WireConnection;78;0;61;0
WireConnection;78;1;80;0
WireConnection;78;2;79;0
WireConnection;401;0;230;0
WireConnection;312;0;83;0
WireConnection;312;1;384;0
WireConnection;221;0;401;0
WireConnection;207;0;78;0
WireConnection;207;1;210;0
WireConnection;207;2;208;0
WireConnection;82;0;51;0
WireConnection;82;1;363;0
WireConnection;82;2;312;0
WireConnection;223;0;207;0
WireConnection;223;1;315;0
WireConnection;223;2;224;0
WireConnection;95;0;96;0
WireConnection;95;1;94;2
WireConnection;460;0;458;0
WireConnection;114;0;223;0
WireConnection;466;0;82;0
WireConnection;466;1;128;0
WireConnection;98;0;97;0
WireConnection;98;1;95;0
WireConnection;457;0;458;0
WireConnection;457;1;115;0
WireConnection;1;0;2;0
WireConnection;218;0;95;0
WireConnection;233;0;30;0
WireConnection;459;0;460;0
WireConnection;459;1;115;0
WireConnection;222;0;233;0
WireConnection;52;0;466;0
WireConnection;100;0;98;0
WireConnection;116;0;223;0
WireConnection;116;2;315;0
WireConnection;0;0;459;0
WireConnection;0;1;53;0
WireConnection;0;2;457;0
WireConnection;0;4;56;0
WireConnection;0;9;1;0
WireConnection;0;11;101;0
WireConnection;0;14;234;0
ASEEND*/
//CHKSM=496823BF597DDAE8FA6EB27B625B0209218F3000