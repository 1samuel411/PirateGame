// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FAE/Grass"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_ColorTop("ColorTop", Color) = (0.3001064,0.6838235,0,1)
		_ColorBottom("Color Bottom", Color) = (0.232,0.5,0,1)
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		[NoScaleOffset][Normal]_BumpMap("BumpMap", 2D) = "bump" {}
		_ColorVariation("ColorVariation", Range( -0.1 , 0.1)) = 0.2
		_AmbientOcclusion("AmbientOcclusion", Range( 0 , 1)) = 0
		_TransmissionSize("Transmission Size", Range( 0 , 20)) = 1
		_TransmissionAmount("Transmission Amount", Range( 0 , 10)) = 2.696819
		_WindWeight("WindWeight", Range( 0 , 1)) = 0.126967
		_WindSwinging("WindSwinging", Range( 0 , 1)) = 0
		_WindAmplitudeMultiplier("WindAmplitudeMultiplier", Float) = 1
		_HeightmapInfluence("HeightmapInfluence", Range( 0 , 1)) = 0
		_MinHeight("MinHeight", Range( -1 , 0)) = -0.5
		_MaxHeight("MaxHeight", Range( -1 , 1)) = 0
		_BendingInfluence("BendingInfluence", Range( 0 , 1)) = 0
		_PigmentMapInfluence("PigmentMapInfluence", Range( 0 , 1)) = 0
		_PigmentMapHeight("PigmentMapHeight", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		LOD 200
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma exclude_renderers xbox360 psp2 n3ds wiiu 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows nolightmap  nodirlightmap dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float3 worldPos;
		};

		uniform sampler2D _BumpMap;
		uniform float4 _ColorTop;
		uniform float4 _ColorBottom;
		uniform sampler2D _MainTex;
		uniform sampler2D _PigmentMap;
		uniform float4 _TerrainUV;
		uniform float _PigmentMapHeight;
		uniform float _PigmentMapInfluence;
		uniform sampler2D _WindVectors;
		uniform float _WindAmplitudeMultiplier;
		uniform float _WindSpeed;
		uniform float4 _WindDirection;
		uniform float _ColorVariation;
		uniform float _TransmissionSize;
		uniform float _TransmissionAmount;
		uniform float _MinHeight;
		uniform float _HeightmapInfluence;
		uniform float _MaxHeight;
		uniform float _WindDebug;
		uniform float _AmbientOcclusion;
		uniform float _WindWeight;
		uniform float _WindSwinging;
		uniform float4 _ObstaclePosition;
		uniform float _BendingStrength;
		uniform float _BendingRadius;
		uniform float _BendingInfluence;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 appendResult469 = (float2(_WindDirection.x , _WindDirection.z));
			float3 WindVector91 = UnpackScaleNormal( tex2Dlod( _WindVectors, float4( ( ( _WindAmplitudeMultiplier * ( (ase_worldPos).xz * 0.01 ) ) + ( ( ( _WindSpeed * 0.05 ) * _Time.w ) * appendResult469 ) ), 0, 0.0) ) ,1.0 );
			float3 appendResult495 = (float3(WindVector91.x , 0.0 , WindVector91.y));
			float3 temp_cast_0 = (-1.0).xxx;
			float3 lerpResult249 = lerp( (float3( 0,0,0 ) + (appendResult495 - temp_cast_0) * (float3( 1,1,0 ) - float3( 0,0,0 )) / (float3( 1,1,0 ) - temp_cast_0)) , appendResult495 , _WindSwinging);
			float3 lerpResult74 = lerp( ( ( _WindWeight * 2.0 ) * lerpResult249 ) , float3( 0,0,0 ) , ( 1.0 - v.color.r ));
			float3 Wind84 = lerpResult74;
			float4 normalizeResult184 = normalize( ( _ObstaclePosition - float4( ase_worldPos , 0.0 ) ) );
			float temp_output_186_0 = ( _BendingStrength * 0.1 );
			float3 appendResult468 = (float3(temp_output_186_0 , 0.0 , temp_output_186_0));
			float clampResult192 = clamp( ( distance( _ObstaclePosition , float4( ase_worldPos , 0.0 ) ) / _BendingRadius ) , 0.0 , 1.0 );
			float4 Bending201 = ( v.color.r * -( ( ( normalizeResult184 * float4( appendResult468 , 0.0 ) ) * ( 1.0 - clampResult192 ) ) * _BendingInfluence ) );
			float4 temp_output_203_0 = ( float4( Wind84 , 0.0 ) + Bending201 );
			float2 appendResult483 = (float2(_TerrainUV.z , _TerrainUV.w));
			float2 TerrainUV324 = ( ( ( 1.0 - appendResult483 ) / _TerrainUV.x ) + ( ( _TerrainUV.x / ( _TerrainUV.x * _TerrainUV.x ) ) * (ase_worldPos).xz ) );
			float4 PigmentMapTex320 = tex2Dlod( _PigmentMap, float4( TerrainUV324, 0, 1.0) );
			float4 lerpResult508 = lerp( temp_output_203_0 , ( temp_output_203_0 * (PigmentMapTex320).a ) , _PigmentMapInfluence);
			float temp_output_499_0 = ( 1.0 - v.color.r );
			float lerpResult344 = lerp( ( ( 1.0 - (PigmentMapTex320).a ) * _MinHeight ) , 0 , temp_output_499_0);
			float lerpResult388 = lerp( _MaxHeight , 0.0 , temp_output_499_0);
			float GrassLength365 = ( ( lerpResult344 * _HeightmapInfluence ) + lerpResult388 );
			float3 appendResult391 = (float3(lerpResult508.x , GrassLength365 , lerpResult508.z));
			float3 VertexOffset330 = appendResult391;
			v.vertex.xyz += VertexOffset330;
			v.normal = float3(0,1,0);
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BumpMap = i.uv_texcoord;
			float3 Normals174 = UnpackScaleNormal( tex2D( _BumpMap, uv_BumpMap ) ,1.0 );
			o.Normal = Normals174;
			float temp_output_501_0 = ( 1.0 - i.vertexColor.r );
			float4 lerpResult363 = lerp( _ColorTop , _ColorBottom , temp_output_501_0);
			float2 uv_MainTex = i.uv_texcoord;
			float4 tex2DNode97 = tex2D( _MainTex, uv_MainTex );
			float2 appendResult483 = (float2(_TerrainUV.z , _TerrainUV.w));
			float3 ase_worldPos = i.worldPos;
			float2 TerrainUV324 = ( ( ( 1.0 - appendResult483 ) / _TerrainUV.x ) + ( ( _TerrainUV.x / ( _TerrainUV.x * _TerrainUV.x ) ) * (ase_worldPos).xz ) );
			float4 PigmentMapTex320 = tex2D( _PigmentMap, TerrainUV324 );
			float lerpResult416 = lerp( temp_output_501_0 , 1.0 , _PigmentMapHeight);
			float4 lerpResult376 = lerp( _ColorTop , PigmentMapTex320 , lerpResult416);
			float4 lerpResult290 = lerp( ( lerpResult363 * tex2DNode97 ) , lerpResult376 , _PigmentMapInfluence);
			float4 temp_cast_0 = (2.0).xxxx;
			float2 appendResult469 = (float2(_WindDirection.x , _WindDirection.z));
			float3 WindVector91 = UnpackScaleNormal( tex2D( _WindVectors, ( ( _WindAmplitudeMultiplier * ( (ase_worldPos).xz * 0.01 ) ) + ( ( ( _WindSpeed * 0.05 ) * _Time.w ) * appendResult469 ) ) ) ,1.0 );
			float lerpResult271 = lerp( ( WindVector91.x * WindVector91.y ) , 0.0 , ( 1.0 - i.vertexColor.r ));
			float4 lerpResult273 = lerp( lerpResult290 , temp_cast_0 , ( ( lerpResult271 * _ColorVariation ) * 2.0 ));
			float4 Color161 = lerpResult273;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float dotResult141 = dot( -ase_worldViewDir , ase_worldlightDir );
			float lerpResult151 = lerp( ( pow( dotResult141 , _TransmissionSize ) * _TransmissionAmount ) , 0.0 , ( ( 1.0 - i.vertexColor.r ) * 1.33 ));
			float temp_output_499_0 = ( 1.0 - i.vertexColor.r );
			float lerpResult344 = lerp( ( ( 1.0 - (PigmentMapTex320).a ) * _MinHeight ) , 0 , temp_output_499_0);
			float lerpResult388 = lerp( _MaxHeight , 0.0 , temp_output_499_0);
			float GrassLength365 = ( ( lerpResult344 * _HeightmapInfluence ) + lerpResult388 );
			float clampResult152 = clamp( ( lerpResult151 * GrassLength365 ) , 0.0 , 1.0 );
			float Subsurface153 = clampResult152;
			float4 lerpResult106 = lerp( Color161 , ( Color161 * 2.0 ) , Subsurface153);
			float4 FinalColor205 = lerpResult106;
			float4 lerpResult310 = lerp( FinalColor205 , float4( WindVector91 , 0.0 ) , _WindDebug);
			o.Albedo = lerpResult310.rgb;
			float clampResult302 = clamp( ( ( i.vertexColor.r * 1.33 ) * _AmbientOcclusion ) , 0.0 , 1.0 );
			float lerpResult115 = lerp( 1.0 , clampResult302 , _AmbientOcclusion);
			float AmbientOcclusion207 = lerpResult115;
			o.Occlusion = AmbientOcclusion207;
			o.Alpha = 1;
			float Alpha98 = tex2DNode97.a;
			float lerpResult313 = lerp( Alpha98 , 1.0 , _WindDebug);
			clip( lerpResult313 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Nature/SpeedTree"
	CustomEditor "FAE.GrassShaderGUI"
}
/*ASEBEGIN
Version=13701
1927;29;1906;1004;7248.723;4121.857;5.8;True;True
Node;AmplifyShaderEditor.CommentaryNode;494;-3977.208,949.6618;Float;False;1606.407;683.4922;Comment;11;324;483;484;485;486;487;488;489;490;491;493;TerrainUV;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;368;-4869.987,-1564.988;Float;False;1396.71;948.4431;Comment;15;319;77;308;69;75;67;79;297;72;221;298;222;383;384;469;Wind engine;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;493;-3927.208,1084.961;Float;False;Global;_TerrainUV;_TerrainUV;2;0;0,0,0,0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;319;-4815.187,-1219.843;Float;False;Global;_WindSpeed;_WindSpeed;11;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;77;-4744.984,-1403.96;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;383;-4716.698,-1120.336;Float;False;Constant;_Float7;Float 7;19;0;0.05;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;485;-3612.408,1313.662;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;484;-3654.354,1454.154;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;483;-3597.408,1000.662;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector4Node;308;-4496.986,-823.5449;Float;False;Global;_WindDirection;_WindDirection;13;0;1,0,0,0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-4386.083,-1166.863;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;69;-4511.581,-1021.262;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;75;-4518.385,-1393.961;Float;False;FLOAT2;0;2;2;2;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleDivideOpNode;486;-3415.408,1234.662;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;488;-3413.408,999.6618;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;384;-4493.496,-1293.735;Float;False;Constant;_Float8;Float 8;19;0;0.01;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;487;-3355.351,1451.154;Float;False;FLOAT2;0;2;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.DynamicAppendNode;469;-4193.525,-786.7736;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-4326.583,-1395.16;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;297;-4208.547,-1514.988;Float;False;Property;_WindAmplitudeMultiplier;WindAmplitudeMultiplier;12;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-4173.885,-1066.363;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;490;-3054.408,1025.662;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;489;-3091.15,1270.754;Float;False;2;2;0;FLOAT;0,0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;298;-3822.147,-1384.588;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;202;-2496.631,-2502.175;Float;False;2627.3;775.1997;Bending;23;181;183;186;188;184;194;189;191;192;193;195;196;197;200;199;198;201;231;232;234;386;387;468;Foliage bending away from obstacle;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;221;-3933.48,-940.0239;Float;False;2;2;0;FLOAT;0,0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;372;-2174,1111.814;Float;False;1452.146;348.7988;Comment;6;290;291;325;320;376;458;Pigment map;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;491;-2812.644,1155.852;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.WorldPosInputsNode;181;-2432.531,-2064.678;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;222;-3627.277,-1289.723;Float;False;2;2;0;FLOAT2;0.0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;411;-3431.091,-1195.542;Float;False;Constant;_Float15;Float 15;22;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;325;-2124,1183.468;Float;False;324;0;1;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;324;-2613.8,1171.067;Float;False;TerrainUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector4Node;231;-2449.17,-2346.924;Float;False;Global;_ObstaclePosition;_ObstaclePosition;18;1;[HideInInspector];0,0,0,0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;234;-2052.17,-2126.924;Float;False;Global;_BendingStrength;_BendingStrength;15;1;[HideInInspector];0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;386;-2033.196,-2047.436;Float;False;Constant;_Float10;Float 10;19;0;0.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DistanceOpNode;189;-2027.732,-1964.974;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;232;-2070.17,-1830.923;Float;False;Global;_BendingRadius;_BendingRadius;14;1;[HideInInspector];0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;458;-1822.705,1188.649;Float;True;Global;_PigmentMap;_PigmentMap;19;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;367;-5331.701,-2521.331;Float;False;2596.326;763.0897;Comment;14;388;365;389;360;344;361;71;342;343;353;392;466;467;499;Grass length;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;410;-3257.792,-1341.04;Float;True;Global;_WindVectors;_WindVectors;9;1;[Normal];None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-1824.531,-2117.877;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;91;-2843.424,-1318.258;Float;False;WindVector;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.CommentaryNode;369;-2527.654,-1466.986;Float;False;2670.73;665.021;Comment;14;277;248;16;247;83;249;66;70;74;84;385;408;495;500;Wind animations;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;191;-1813.931,-1918.974;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;183;-2016.431,-2313.579;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;387;-1791.397,-1812.136;Float;False;Constant;_Float11;Float 11;19;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;320;-1468.689,1171.858;Float;False;PigmentMapTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;466;-5241.107,-2455.851;Float;False;320;0;1;COLOR
Node;AmplifyShaderEditor.BreakToComponentsNode;277;-2478.654,-1311.989;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.NormalizeNode;184;-1738.534,-2314.377;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.DynamicAppendNode;468;-1618.13,-2136.973;Float;False;FLOAT3;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.ClampOpNode;192;-1642.931,-1918.974;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;467;-4871.107,-2434.851;Float;False;FLOAT;3;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;373;-2187.382,1907.354;Float;False;1550.595;376.3005;Comment;10;93;240;86;239;271;101;274;307;407;502;Color through wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;160;-2656.509,2621.972;Float;False;2711.621;557.9603;Subsurface scattering;18;153;152;380;151;149;147;148;146;145;150;141;143;139;140;138;454;455;503;Subsurface color simulation;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;495;-2117.922,-1326.672;Float;False;FLOAT3;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;138;-2510.711,2676.372;Float;False;World;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-1378.531,-2307.876;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.OneMinusNode;193;-1398.931,-1916.974;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;408;-2106.496,-1198.74;Float;False;Constant;_Float14;Float 14;20;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;353;-4587.101,-2459.73;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;371;-2164.253,-187.5875;Float;False;1401.6;1171.933;Comment;11;98;293;97;363;416;362;364;418;417;292;501;Color gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-2137.383,1957.353;Float;False;91;0;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;392;-4659.487,-2282.831;Float;False;Property;_MinHeight;MinHeight;14;0;-0.5;-1;0;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;343;-4730.892,-2068.831;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TFHCRemapNode;247;-1889.517,-1331.735;Float;False;5;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,1,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;1,1,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;248;-1968.817,-1114.336;Float;False;Property;_WindSwinging;WindSwinging;11;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-2106.482,-1416.986;Float;False;Property;_WindWeight;WindWeight;10;0;0.126967;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;364;-2109.1,358.1681;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;385;-1578.297,-1361.734;Float;False;Constant;_Float9;Float 9;19;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;194;-1140.731,-2148.375;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;195;-1068.93,-2019.775;Float;False;Property;_BendingInfluence;BendingInfluence;16;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;240;-1889.973,1960.51;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;499;-4497.814,-2055.856;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;86;-1848.981,2098.654;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.NegateNode;139;-2350.711,2679.372;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;342;-4261.1,-2315.93;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;140;-2608.709,2838.372;Float;False;1;0;FLOAT;0.0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;361;-4117.398,-2097.63;Float;False;Property;_HeightmapInfluence;HeightmapInfluence;13;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;143;-2302.409,2899.172;Float;False;Property;_TransmissionSize;Transmission Size;7;0;1;0;20;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;83;-1220.283,-1161.465;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DotProductOpNode;141;-2148.712,2736.372;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;417;-1909.692,672.5588;Float;False;Constant;_Float17;Float 17;22;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;501;-1911.019,379.4436;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;249;-1550.217,-1281.035;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;418;-2041.291,768.9584;Float;False;Property;_PigmentMapHeight;PigmentMapHeight;18;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;148;-1555.112,2873.572;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;-1612.174,1959.109;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;292;-2114.253,-137.5875;Float;False;Property;_ColorTop;ColorTop;1;0;0.3001064,0.6838235,0,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;71;-4135.585,-1997.961;Float;False;Property;_MaxHeight;MaxHeight;15;0;0;-1;1;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;362;-2110.3,70.16856;Float;False;Property;_ColorBottom;Color Bottom;2;0;0.232,0.5,0,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-1412.383,-1408.666;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;196;-752.131,-2146.676;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.OneMinusNode;502;-1578.219,2122.444;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;344;-4031.5,-2310.331;Float;False;3;0;FLOAT;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;146;-1870.512,2895.772;Float;False;Property;_TransmissionAmount;Transmission Amount;8;0;2.696819;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;500;-943.9202,-1136.156;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;198;-833.1311,-2452.175;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;97;-1603.925,174.9452;Float;True;Property;_MainTex;MainTex;3;1;[NoScaleOffset];None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;150;-1480.913,3055.173;Float;False;Constant;_TransmissionHeight;TransmissionHeight;12;0;1.33;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;101;-1360.739,2159.153;Float;False;Property;_ColorVariation;ColorVariation;5;0;0.2;-0.1;0.1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;388;-3648.988,-2005.233;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;503;-1364.919,2893.644;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;416;-1628.192,699.2587;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;363;-1673.701,20.66853;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;360;-3758.896,-2320.53;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;197;-570.631,-2144.675;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-1190.383,-1322.265;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;271;-1386.749,1985.81;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;145;-1933.711,2738.372;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;200;-346.1306,-2287.775;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT4;0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;74;-775.5834,-1327.363;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;293;-1270.853,11.3124;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;389;-3355.987,-2315.133;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;149;-1194.712,2904.772;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;-1548.112,2738.172;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;291;-1305.555,1345.614;Float;False;Property;_PigmentMapInfluence;PigmentMapInfluence;17;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;376;-1134.404,1162.263;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.CommentaryNode;374;-474.603,508.4473;Float;False;1993.711;524.3113;Comment;12;330;391;437;366;508;507;203;426;85;204;456;509;Vertex function layer blend;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;274;-1029.651,1981.51;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;407;-910.7954,2122.06;Float;False;Constant;_Float13;Float 13;20;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;85;-389.1235,558.4475;Float;False;84;0;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;201;-112.3306,-2292.976;Float;False;Bending;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;497;-321.8141,1439.534;Float;False;Constant;_ColorVarValue;ColorVarValue;20;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;151;-994.5121,2737.772;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;159;-2234.034,3511.791;Float;False;1813.59;398.8397;AO;11;207;115;114;117;301;118;113;111;302;381;382;Ambient Occlusion by Red vertex color channel;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;290;-881.855,1189.814;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;365;-3054.6,-2332.23;Float;False;GrassLength;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;455;-911.2034,2893.349;Float;False;365;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;307;-805.7888,1997.455;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;456;-418.0031,763.4494;Float;False;320;0;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;204;-384.6308,651.2245;Float;False;201;0;1;FLOAT4
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;-106.5239,-1316.954;Float;False;Wind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;273;-23.95152,1402.711;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;203;-21.43014,573.0236;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.VertexColorNode;111;-2184.033,3561.791;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;380;-646.8998,2937.164;Float;False;Constant;_Float4;Float 4;19;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;236;425.4059,1353.126;Float;False;1576.244;290.1742;SSS Blending with color;4;205;106;295;296;Final color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;426;238.806,635.1555;Float;False;219;183;Mask wind/bending by height;1;420;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;454;-674.2034,2740.25;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;382;-1904.201,3673.864;Float;False;Constant;_Float6;Float 6;19;0;1.33;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;507;-66.32008,763.1448;Float;False;FLOAT;3;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;161;217.5887,1402.872;Float;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ClampOpNode;152;-440.7127,2730.272;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;509;429.8795,862.7468;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;296;476.5475,1538.814;Float;False;Constant;_Float1;Float 1;21;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;113;-2115.814,3769.771;Float;False;Property;_AmbientOcclusion;AmbientOcclusion;6;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;301;-1664.347,3582.513;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;420;290.8063,685.1555;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-1461.516,3606.97;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;153;-264.2131,2733.172;Float;False;Subsurface;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;295;657.5471,1493.814;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;118;-1346.713,3787.771;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;237;-2228.413,4167.212;Float;False;978.701;287.5597;;3;174;172;419;Normal map;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;508;521.4804,575.9468;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;381;-1439.799,3819.264;Float;False;Constant;_Float5;Float 5;19;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;437;706.5005,586.9504;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;106;932.2648,1446.696;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;117;-1308.615,3727.071;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;375;2964.505,1790.556;Float;False;352;249.0994;Comment;2;312;311;Debug switch;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;235;2843.666,889.9761;Float;False;452.9371;811.1447;Final;5;99;208;175;206;331;Outputs;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;366;743.3975,738.5689;Float;False;365;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;302;-1228.647,3589.713;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;419;-2115.794,4295.757;Float;False;Constant;_Float18;Float 18;21;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;-1267.766,268.9144;Float;False;Alpha;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;312;3073.705,1840.556;Float;False;91;0;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;311;3014.505,1924.656;Float;False;Global;_WindDebug;_WindDebug;20;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;206;3072.166,941.4243;Float;False;205;0;1;COLOR
Node;AmplifyShaderEditor.DynamicAppendNode;391;1015.014,587.7674;Float;False;FLOAT3;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;99;3096.573,1235.245;Float;False;98;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;172;-1877.412,4221.771;Float;True;Property;_BumpMap;BumpMap;4;2;[NoScaleOffset];[Normal];None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;205;1636.98,1493.291;Float;False;FinalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;406;3416.104,1297.26;Float;False;Constant;_Float12;Float 12;20;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;115;-946.4141,3606.971;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;175;3082.283,1039.971;Float;False;174;0;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;208;3025.767,1141.224;Float;False;207;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;331;3064.599,1369.667;Float;False;330;0;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;199;-581.131,-2378.575;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;330;1248.001,595.7672;Float;False;VertexOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.Vector3Node;451;3155.699,2077.05;Float;False;Constant;_Vector0;Vector 0;21;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;207;-726.8301,3600.526;Float;False;AmbientOcclusion;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;313;3587.307,1254.955;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;310;3589.109,973.5546;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;174;-1492.712,4217.211;Float;False;Normals;-1;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3894.866,1047.348;Float;False;True;2;Float;FAE.GrassShaderGUI;200;0;Standard;FAE/Grass;False;False;False;False;False;False;True;False;True;False;False;False;True;False;False;False;True;Off;0;0;False;0;0;Custom;0.5;True;True;0;True;Opaque;AlphaTest;All;True;True;True;True;True;True;True;False;True;True;False;False;False;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;200;Nature/SpeedTree;0;-1;-1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;485;0;493;1
WireConnection;485;1;493;1
WireConnection;483;0;493;3
WireConnection;483;1;493;4
WireConnection;67;0;319;0
WireConnection;67;1;383;0
WireConnection;75;0;77;0
WireConnection;486;0;493;1
WireConnection;486;1;485;0
WireConnection;488;0;483;0
WireConnection;487;0;484;0
WireConnection;469;0;308;1
WireConnection;469;1;308;3
WireConnection;72;0;75;0
WireConnection;72;1;384;0
WireConnection;79;0;67;0
WireConnection;79;1;69;4
WireConnection;490;0;488;0
WireConnection;490;1;493;1
WireConnection;489;0;486;0
WireConnection;489;1;487;0
WireConnection;298;0;297;0
WireConnection;298;1;72;0
WireConnection;221;0;79;0
WireConnection;221;1;469;0
WireConnection;491;0;490;0
WireConnection;491;1;489;0
WireConnection;222;0;298;0
WireConnection;222;1;221;0
WireConnection;324;0;491;0
WireConnection;189;0;231;0
WireConnection;189;1;181;0
WireConnection;458;1;325;0
WireConnection;410;1;222;0
WireConnection;410;5;411;0
WireConnection;186;0;234;0
WireConnection;186;1;386;0
WireConnection;91;0;410;0
WireConnection;191;0;189;0
WireConnection;191;1;232;0
WireConnection;183;0;231;0
WireConnection;183;1;181;0
WireConnection;320;0;458;0
WireConnection;277;0;91;0
WireConnection;184;0;183;0
WireConnection;468;0;186;0
WireConnection;468;2;186;0
WireConnection;192;0;191;0
WireConnection;192;2;387;0
WireConnection;467;0;466;0
WireConnection;495;0;277;0
WireConnection;495;2;277;1
WireConnection;188;0;184;0
WireConnection;188;1;468;0
WireConnection;193;0;192;0
WireConnection;353;0;467;0
WireConnection;247;0;495;0
WireConnection;247;1;408;0
WireConnection;194;0;188;0
WireConnection;194;1;193;0
WireConnection;240;0;93;0
WireConnection;499;0;343;1
WireConnection;139;0;138;0
WireConnection;342;0;353;0
WireConnection;342;1;392;0
WireConnection;141;0;139;0
WireConnection;141;1;140;0
WireConnection;501;0;364;1
WireConnection;249;0;247;0
WireConnection;249;1;495;0
WireConnection;249;2;248;0
WireConnection;239;0;240;0
WireConnection;239;1;240;1
WireConnection;66;0;16;0
WireConnection;66;1;385;0
WireConnection;196;0;194;0
WireConnection;196;1;195;0
WireConnection;502;0;86;1
WireConnection;344;0;342;0
WireConnection;344;2;499;0
WireConnection;500;0;83;1
WireConnection;388;0;71;0
WireConnection;388;2;499;0
WireConnection;503;0;148;1
WireConnection;416;0;501;0
WireConnection;416;1;417;0
WireConnection;416;2;418;0
WireConnection;363;0;292;0
WireConnection;363;1;362;0
WireConnection;363;2;501;0
WireConnection;360;0;344;0
WireConnection;360;1;361;0
WireConnection;197;0;196;0
WireConnection;70;0;66;0
WireConnection;70;1;249;0
WireConnection;271;0;239;0
WireConnection;271;2;502;0
WireConnection;145;0;141;0
WireConnection;145;1;143;0
WireConnection;200;0;198;1
WireConnection;200;1;197;0
WireConnection;74;0;70;0
WireConnection;74;2;500;0
WireConnection;293;0;363;0
WireConnection;293;1;97;0
WireConnection;389;0;360;0
WireConnection;389;1;388;0
WireConnection;149;0;503;0
WireConnection;149;1;150;0
WireConnection;147;0;145;0
WireConnection;147;1;146;0
WireConnection;376;0;292;0
WireConnection;376;1;320;0
WireConnection;376;2;416;0
WireConnection;274;0;271;0
WireConnection;274;1;101;0
WireConnection;201;0;200;0
WireConnection;151;0;147;0
WireConnection;151;2;149;0
WireConnection;290;0;293;0
WireConnection;290;1;376;0
WireConnection;290;2;291;0
WireConnection;365;0;389;0
WireConnection;307;0;274;0
WireConnection;307;1;407;0
WireConnection;84;0;74;0
WireConnection;273;0;290;0
WireConnection;273;1;497;0
WireConnection;273;2;307;0
WireConnection;203;0;85;0
WireConnection;203;1;204;0
WireConnection;454;0;151;0
WireConnection;454;1;455;0
WireConnection;507;0;456;0
WireConnection;161;0;273;0
WireConnection;152;0;454;0
WireConnection;152;2;380;0
WireConnection;509;0;291;0
WireConnection;301;0;111;1
WireConnection;301;1;382;0
WireConnection;420;0;203;0
WireConnection;420;1;507;0
WireConnection;114;0;301;0
WireConnection;114;1;113;0
WireConnection;153;0;152;0
WireConnection;295;0;161;0
WireConnection;295;1;296;0
WireConnection;118;0;113;0
WireConnection;508;0;203;0
WireConnection;508;1;420;0
WireConnection;508;2;509;0
WireConnection;437;0;508;0
WireConnection;106;0;161;0
WireConnection;106;1;295;0
WireConnection;106;2;153;0
WireConnection;117;0;118;0
WireConnection;302;0;114;0
WireConnection;302;2;381;0
WireConnection;98;0;97;4
WireConnection;391;0;437;0
WireConnection;391;1;366;0
WireConnection;391;2;437;2
WireConnection;172;5;419;0
WireConnection;205;0;106;0
WireConnection;115;0;381;0
WireConnection;115;1;302;0
WireConnection;115;2;117;0
WireConnection;199;0;198;1
WireConnection;330;0;391;0
WireConnection;207;0;115;0
WireConnection;313;0;99;0
WireConnection;313;1;406;0
WireConnection;313;2;311;0
WireConnection;310;0;206;0
WireConnection;310;1;312;0
WireConnection;310;2;311;0
WireConnection;174;0;172;0
WireConnection;0;0;310;0
WireConnection;0;1;175;0
WireConnection;0;5;208;0
WireConnection;0;10;313;0
WireConnection;0;11;331;0
WireConnection;0;12;451;0
ASEEND*/
//CHKSM=BAC0D7D9CDA09C55143085BAC3B733ADE091010D