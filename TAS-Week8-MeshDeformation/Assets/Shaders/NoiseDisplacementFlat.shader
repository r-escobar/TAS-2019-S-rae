// Upgrade NOTE: upgraded instancing buffer 'NoiseDisplacementFlat' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NoiseDisplacementFlat"
{
	Properties
	{
		_Albedo("Albedo", Color) = (0.6320754,0.6320754,0.6320754,0)
		_ShadingColor("Shading Color", Color) = (0.5377358,0.5377358,0.5377358,1)
		_Frequency("Frequency", Float) = 1
		_Amplitude("Amplitude", Float) = 1
		_Lacunarity("Lacunarity", Float) = 2
		_Persistence("Persistence", Float) = 0.5
		_TimeScale("Time Scale", Float) = 1
		_ShadingDarkness("Shading Darkness", Float) = 0
		_ShadingLightness("Shading Lightness", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
		};

		uniform float _TimeScale;
		uniform float _Frequency;
		uniform float _Amplitude;
		uniform float _Persistence;
		uniform float _Lacunarity;
		uniform float4 _ShadingColor;
		uniform float _ShadingDarkness;
		uniform float _ShadingLightness;

		UNITY_INSTANCING_BUFFER_START(NoiseDisplacementFlat)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Albedo)
#define _Albedo_arr NoiseDisplacementFlat
		UNITY_INSTANCING_BUFFER_END(NoiseDisplacementFlat)


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
			float mulTime3 = _Time.y * _TimeScale;
			float simplePerlin3D1 = snoise( ( ( ase_worldPos + mulTime3 ) * _Frequency ) );
			float simplePerlin3D27 = snoise( ( ( _Frequency * _Lacunarity ) * ( ase_worldPos + mulTime3 ) ) );
			float3 temp_output_12_0 = ( ase_vertex3Pos * ( ( simplePerlin3D1 * _Amplitude ) + ( _Amplitude * _Persistence * simplePerlin3D27 ) ) );
			v.vertex.xyz += temp_output_12_0;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 _Albedo_Instance = UNITY_ACCESS_INSTANCED_PROP(_Albedo_arr, _Albedo);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 ase_worldPos = i.worldPos;
			float mulTime3 = _Time.y * _TimeScale;
			float simplePerlin3D1 = snoise( ( ( ase_worldPos + mulTime3 ) * _Frequency ) );
			float simplePerlin3D27 = snoise( ( ( _Frequency * _Lacunarity ) * ( ase_worldPos + mulTime3 ) ) );
			float3 temp_output_12_0 = ( ase_vertex3Pos * ( ( simplePerlin3D1 * _Amplitude ) + ( _Amplitude * _Persistence * simplePerlin3D27 ) ) );
			o.Emission = ( _Albedo_Instance * ( _ShadingColor * (_ShadingDarkness + (length( temp_output_12_0 ) - 0.0) * (_ShadingLightness - _ShadingDarkness) / (1.0 - 0.0)) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16301
474;73;943;435;2263.907;-303.9056;1.925048;True;False
Node;AmplifyShaderEditor.CommentaryNode;30;-1086.845,854.3942;Float;False;896.6229;263.0496;Noise Octave 1;7;24;23;27;19;26;21;18;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1590.418,711.3015;Float;False;Property;_TimeScale;Time Scale;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;29;-1083.817,455.8749;Float;False;795.969;238.5734;Noise Octave 0;5;31;1;8;4;9;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1071.256,909.6046;Float;False;Property;_Lacunarity;Lacunarity;4;0;Create;True;0;0;False;0;2;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;3;-1375.387,715.8029;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;58;-1520.282,437.0124;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;9;-1065.501,615.6462;Float;False;Property;_Frequency;Frequency;2;0;Create;True;0;0;False;0;1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-1051.382,1008.347;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-888.1207,909.9955;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;4;-1054.408,504.5027;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-726.4249,994.5131;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-864.7917,544.6136;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-616.5352,734.3508;Float;False;Property;_Amplitude;Amplitude;3;0;Create;True;0;0;False;0;1;0.47;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;27;-565.7917,1029.92;Float;False;Simplex3D;1;0;FLOAT3;1,1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;1;-680.181,553.7082;Float;False;Simplex3D;1;0;FLOAT3;1,1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-550.1645,916.1824;Float;False;Property;_Persistence;Persistence;5;0;Create;True;0;0;False;0;0.5;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-431.7848,552.749;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-331.5019,934.8278;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-143.8774,661.034;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;2;-502.7176,156.0429;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;53;191.7208,27.65765;Float;False;1115.954;394.4793;Apply Noise Color;8;44;43;34;33;42;5;35;37;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-9.073303,187.8206;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;43;224.6167,205.9684;Float;False;Property;_ShadingDarkness;Shading Darkness;7;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;220.3109,322.9493;Float;False;Property;_ShadingLightness;Shading Lightness;8;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;34;253.3114,104.0711;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;35;502.2863,76.25009;Float;False;Property;_ShadingColor;Shading Color;1;0;Create;True;0;0;False;0;0.5377358,0.5377358,0.5377358,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;33;525.0522,248.4324;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;826.8685,250.4521;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;5;806.3332,72.85085;Float;False;InstancedProperty;_Albedo;Albedo;0;0;Create;True;0;0;False;0;0.6320754,0.6320754,0.6320754,0;0,0.3784517,0.8867924,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;1093.942,160.3101;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;57;1362.838,504.1443;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;NoiseDisplacementFlat;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;13;0
WireConnection;24;0;58;0
WireConnection;24;1;3;0
WireConnection;21;0;9;0
WireConnection;21;1;18;0
WireConnection;4;0;58;0
WireConnection;4;1;3;0
WireConnection;26;0;21;0
WireConnection;26;1;24;0
WireConnection;8;0;4;0
WireConnection;8;1;9;0
WireConnection;27;0;26;0
WireConnection;1;0;8;0
WireConnection;31;0;1;0
WireConnection;31;1;10;0
WireConnection;23;0;10;0
WireConnection;23;1;19;0
WireConnection;23;2;27;0
WireConnection;28;0;31;0
WireConnection;28;1;23;0
WireConnection;12;0;2;0
WireConnection;12;1;28;0
WireConnection;34;0;12;0
WireConnection;33;0;34;0
WireConnection;33;3;43;0
WireConnection;33;4;44;0
WireConnection;37;0;35;0
WireConnection;37;1;33;0
WireConnection;42;0;5;0
WireConnection;42;1;37;0
WireConnection;57;2;42;0
WireConnection;57;11;12;0
ASEEND*/
//CHKSM=487EBCCC4E9CA11426CBA80BA87400EE357AAAE8