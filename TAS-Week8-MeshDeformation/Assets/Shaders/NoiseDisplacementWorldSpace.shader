// Upgrade NOTE: upgraded instancing buffer 'NoiseDisplacementWorldSpace' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NoiseDisplacementWorldSpace"
{
	Properties
	{
		_Albedo("Albedo", Color) = (0.6320754,0.6320754,0.6320754,0)
		_ShadingColor("Shading Color", Color) = (0.5377358,0.5377358,0.5377358,1)
		_Frequency("Frequency", Float) = 1
		_Amplitude("Amplitude", Float) = 1
		_Lacunarity("Lacunarity", Float) = 2
		_Persistance("Persistance", Float) = 0.5
		_ShadingDarkness("Shading Darkness", Float) = 0
		_ShadingLightness("Shading Lightness", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
		};

		uniform float _Frequency;
		uniform float _Amplitude;
		uniform float _Persistance;
		uniform float _Lacunarity;
		uniform float4 _ShadingColor;
		uniform float _ShadingDarkness;
		uniform float _ShadingLightness;

		UNITY_INSTANCING_BUFFER_START(NoiseDisplacementWorldSpace)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Albedo)
#define _Albedo_arr NoiseDisplacementWorldSpace
		UNITY_INSTANCING_BUFFER_END(NoiseDisplacementWorldSpace)


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float simplePerlin3D15 = snoise( ( ase_worldPos * _Frequency ) );
			float simplePerlin3D12 = snoise( ( ( _Frequency * _Lacunarity ) * ase_worldPos ) );
			float3 temp_output_19_0 = ( ase_vertex3Pos * ( ( simplePerlin3D15 * _Amplitude ) + ( _Amplitude * _Persistance * simplePerlin3D12 ) ) );
			v.vertex.xyz += temp_output_19_0;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _Albedo_Instance = UNITY_ACCESS_INSTANCED_PROP(_Albedo_arr, _Albedo);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 ase_worldPos = i.worldPos;
			float simplePerlin3D15 = snoise( ( ase_worldPos * _Frequency ) );
			float simplePerlin3D12 = snoise( ( ( _Frequency * _Lacunarity ) * ase_worldPos ) );
			float3 temp_output_19_0 = ( ase_vertex3Pos * ( ( simplePerlin3D15 * _Amplitude ) + ( _Amplitude * _Persistance * simplePerlin3D12 ) ) );
			o.Albedo = ( _Albedo_Instance * ( _ShadingColor * (_ShadingDarkness + (length( temp_output_19_0 ) - 0.0) * (_ShadingLightness - _ShadingDarkness) / (1.0 - 0.0)) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16301
1;1;1918;1016;1201.032;606.3831;1.3;True;False
Node;AmplifyShaderEditor.CommentaryNode;1;-1272.914,365.9497;Float;False;1050.023;347.5496;Noise Octave 1;6;16;13;12;10;7;5;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;2;-1267.286,-96.26907;Float;False;1046.869;319.1734;Noise Octave 0;4;17;15;11;6;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1204.77,124.6025;Float;False;Property;_Frequency;Frequency;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1198.824,415.9598;Float;False;Property;_Lacunarity;Lacunarity;4;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-987.0895,426.7511;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;3;-1654.005,198.816;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1033.961,-29.63027;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-999.594,612.6686;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-631.0046,268.0066;Float;False;Property;_Amplitude;Amplitude;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;15;-702.4501,32.7644;Float;False;Simplex3D;1;0;FLOAT3;1,1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;12;-671.2616,622.0767;Float;False;Simplex3D;1;0;FLOAT3;1,1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-672.5339,517.4374;Float;False;Property;_Persistance;Persistance;5;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-403.1714,498.3829;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-422.8541,31.80521;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;34;25.29207,-305.1339;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;18;31.15584,196.9715;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;274.1485,-149.6732;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;21;983.0818,-182.5545;Float;False;Property;_ShadingLightness;Shading Lightness;7;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;20;626.9769,-385.2557;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;983.0818,-267.2768;Float;False;Property;_ShadingDarkness;Shading Darkness;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;24;1279.645,-340.873;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;23;1525.731,-532.1118;Float;False;Property;_ShadingColor;Shading Color;1;0;Create;True;0;0;False;0;0.5377358,0.5377358,0.5377358,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;2013.036,-445.8431;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;26;1880.75,-810.1715;Float;False;InstancedProperty;_Albedo;Albedo;0;0;Create;True;0;0;False;0;0.6320754,0.6320754,0.6320754,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;2341.997,-431.2487;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2786.477,-375.9303;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;NoiseDisplacementWorldSpace;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;6;0
WireConnection;7;1;5;0
WireConnection;11;0;3;0
WireConnection;11;1;6;0
WireConnection;10;0;7;0
WireConnection;10;1;3;0
WireConnection;15;0;11;0
WireConnection;12;0;10;0
WireConnection;16;0;14;0
WireConnection;16;1;13;0
WireConnection;16;2;12;0
WireConnection;17;0;15;0
WireConnection;17;1;14;0
WireConnection;18;0;17;0
WireConnection;18;1;16;0
WireConnection;19;0;34;0
WireConnection;19;1;18;0
WireConnection;20;0;19;0
WireConnection;24;0;20;0
WireConnection;24;3;22;0
WireConnection;24;4;21;0
WireConnection;27;0;23;0
WireConnection;27;1;24;0
WireConnection;28;0;26;0
WireConnection;28;1;27;0
WireConnection;0;0;28;0
WireConnection;0;11;19;0
ASEEND*/
//CHKSM=992E87F2D9F61E8CDDBBD72D1AE462D286ECF4BB