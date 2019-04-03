// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "InClassFishAnimExample"
{
	Properties
	{
		_Amplitude("Amplitude", Float) = 0
		_TimeOffset("Time Offset", Float) = 0
		_Frequency("Frequency", Float) = 0
		_AmplitudeOffset("Amplitude Offset", Float) = 0
		_PositionalOffsetScalar("Positional Offset Scalar", Float) = 0
		_PositoinalAmplitudeScalar("Positoinal Amplitude Scalar", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			half filler;
		};

		uniform float _Amplitude;
		uniform float _Frequency;
		uniform float _TimeOffset;
		uniform float _PositionalOffsetScalar;
		uniform float _PositoinalAmplitudeScalar;
		uniform float _AmplitudeOffset;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float4 appendResult15 = (float4(( ( _Amplitude * sin( ( ( _Frequency * _Time.y ) + _TimeOffset + ( ase_vertex3Pos.z * _PositionalOffsetScalar ) ) ) * ( ase_vertex3Pos.z * _PositoinalAmplitudeScalar ) ) + _AmplitudeOffset ) , 0.0 , 0.0 , 0.0));
			v.vertex.xyz += appendResult15.xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16301
7;7;1906;1004;1815.134;109.4854;1.012471;True;False
Node;AmplifyShaderEditor.CommentaryNode;22;-1836.972,220.222;Float;False;922.8853;762;Adding the scaled and offset time value to the vertex's y position;4;13;5;20;21;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;21;-1692.972,270.2219;Float;False;394;324;Scales and Offsets Time Input;4;6;9;8;2;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;20;-1786.972,630.2219;Float;False;498;326;Scales Vertex Y Position;3;17;19;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;16;-1696.305,679.5172;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;2;-1645.087,419.7452;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1758.144,834.1267;Float;False;Property;_PositionalOffsetScalar;Positional Offset Scalar;4;0;Create;True;0;0;False;0;0;17.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1643.087,326.7451;Float;False;Property;_Frequency;Frequency;2;0;Create;True;0;0;False;0;0;5.72;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;28;-1456.918,1002.752;Float;False;543.639;249.309;Uses distance from origin as scalar multiplier of amplitude;2;27;26;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1483.104,725.0176;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1483.087,498.7452;Float;False;Property;_TimeOffset;Time Offset;1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-1459.087,396.7452;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1406.918,1137.061;Float;False;Property;_PositoinalAmplitudeScalar;Positoinal Amplitude Scalar;5;0;Create;True;0;0;False;0;0;16.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-1204.087,393.7451;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;24;-884.4871,222.9452;Float;False;553.9999;353;Scaling and offsetting sin ouput;4;3;4;10;7;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SinOpNode;5;-1056.087,393.7451;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-834.4871,272.9451;Float;False;Property;_Amplitude;Amplitude;0;0;Create;True;0;0;False;0;0;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1082.279,1052.752;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-730.4871,460.9451;Float;False;Property;_AmplitudeOffset;Amplitude Offset;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-664.4871,276.9451;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-484.4872,276.9451;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;25;-300.4957,228.0251;Float;False;217;229;Applying result to x axis;1;15;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;15;-250.4957,278.0251;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;InClassFishAnimExample;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;16;3
WireConnection;17;1;19;0
WireConnection;9;0;8;0
WireConnection;9;1;2;0
WireConnection;13;0;9;0
WireConnection;13;1;6;0
WireConnection;13;2;17;0
WireConnection;5;0;13;0
WireConnection;26;0;16;3
WireConnection;26;1;27;0
WireConnection;4;0;3;0
WireConnection;4;1;5;0
WireConnection;4;2;26;0
WireConnection;7;0;4;0
WireConnection;7;1;10;0
WireConnection;15;0;7;0
WireConnection;0;11;15;0
ASEEND*/
//CHKSM=DB202DE806A87DBA3C816B16130CA91BDD734951