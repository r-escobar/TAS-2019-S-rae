// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ScreenWipe"
{
	Properties
	{
		_Cutoff("Cutoff", Range( 0 , 1)) = 0
		_WipeLookupTexture("Wipe Lookup Texture", 2D) = "white" {}
		_LookupOverlayTexture("Lookup Overlay Texture", 2D) = "white" {}
		[Toggle]_InterstitialBackgroundToggle("Interstitial Background Toggle", Float) = 1
		_NextCameraTexture("Next Camera Texture", 2D) = "white" {}
		_BackgroundTexture("Background Texture", 2D) = "white" {}
		_BackgroundColor("Background Color", Color) = (0,0,0,0)
		_BackgroundTextureScale("Background Texture Scale", Float) = 1
		_BackgroundScrollSpeedX("Background Scroll Speed X", Float) = 0
		_BackgroundScrollSpeedY("Background Scroll Speed Y", Float) = 0
		[HideInInspector]_MainTex("MainTex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
			};

			uniform sampler2D _WipeLookupTexture;
			uniform float4 _WipeLookupTexture_ST;
			uniform sampler2D _LookupOverlayTexture;
			uniform float4 _LookupOverlayTexture_ST;
			uniform float _Cutoff;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _InterstitialBackgroundToggle;
			uniform sampler2D _NextCameraTexture;
			uniform float4 _NextCameraTexture_ST;
			uniform float4 _BackgroundColor;
			uniform sampler2D _BackgroundTexture;
			uniform float _BackgroundScrollSpeedX;
			uniform float _BackgroundScrollSpeedY;
			uniform float _BackgroundTextureScale;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				float3 vertexValue =  float3(0,0,0) ;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 finalColor;
				float2 uv_WipeLookupTexture = i.ase_texcoord.xy * _WipeLookupTexture_ST.xy + _WipeLookupTexture_ST.zw;
				float2 uv_LookupOverlayTexture = i.ase_texcoord.xy * _LookupOverlayTexture_ST.xy + _LookupOverlayTexture_ST.zw;
				float2 uv_MainTex = i.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode2 = tex2D( _MainTex, uv_MainTex );
				float2 uv_NextCameraTexture = i.ase_texcoord.xy * _NextCameraTexture_ST.xy + _NextCameraTexture_ST.zw;
				float mulTime21 = _Time.y * _BackgroundScrollSpeedX;
				float mulTime20 = _Time.y * _BackgroundScrollSpeedY;
				float4 appendResult24 = (float4(mulTime21 , mulTime20 , 0.0 , 0.0));
				float4 screenPos = i.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float temp_output_18_0 = ( ( _ScreenParams.z + -1.0 ) * _BackgroundTextureScale );
				float4 appendResult25 = (float4(( ase_screenPosNorm.x * _ScreenParams.x * temp_output_18_0 ) , ( ase_screenPosNorm.y * _ScreenParams.y * temp_output_18_0 ) , 0.0 , 0.0));
				float4 ifLocalVar4 = 0;
				if( (0.0 + (( tex2D( _WipeLookupTexture, uv_WipeLookupTexture ) * tex2D( _LookupOverlayTexture, uv_LookupOverlayTexture ) ).r - 0.0) * (0.999 - 0.0) / (1.0 - 0.0)) >= _Cutoff )
				ifLocalVar4 = tex2DNode2;
				else
				ifLocalVar4 = lerp(tex2D( _NextCameraTexture, uv_NextCameraTexture ),( _BackgroundColor * tex2D( _BackgroundTexture, ( appendResult24 + appendResult25 ).xy ) ),_InterstitialBackgroundToggle);
				
				
				finalColor = ifLocalVar4;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16500
626;73;1232;443;3429.508;-50.31229;2.581045;True;False
Node;AmplifyShaderEditor.ScreenParams;13;-2748.326,1008.997;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;15;-2660.646,1362.293;Float;False;Property;_BackgroundTextureScale;Background Texture Scale;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-2557.033,1205.224;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-2476.824,620.7291;Float;False;Property;_BackgroundScrollSpeedX;Background Scroll Speed X;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-2381.685,730.0749;Float;False;Property;_BackgroundScrollSpeedY;Background Scroll Speed Y;9;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;19;-2713.85,811.6265;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-2420.799,1214.089;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;20;-2167.566,732.0626;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;21;-2166.901,622.0898;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-2223.229,1140.11;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-2223.859,847.1835;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;25;-1988.198,958.7219;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-1945.936,652.4785;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-1689.449,884.42;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;30;-1321.087,-172.0125;Float;True;Property;_LookupOverlayTexture;Lookup Overlay Texture;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;-1320.29,-472.3526;Float;True;Property;_WipeLookupTexture;Wipe Lookup Texture;1;0;Create;True;0;0;False;0;None;6e6cba53deb4f4e41a81667b73a1ca42;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;27;-1439.582,864.918;Float;True;Property;_BackgroundTexture;Background Texture;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-896.4075,-177.1264;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;6;-1424.873,610.893;Float;False;Property;_BackgroundColor;Background Color;6;0;Create;True;0;0;False;0;0,0,0,0;0.1818356,0,0.245283,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;32;-602.9597,-214.7367;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;11;-923.4639,406.0797;Float;True;Property;_NextCameraTexture;Next Camera Texture;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-957.309,828.1793;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-542.1611,202.3876;Float;True;Property;_MainTex;MainTex;10;1;[HideInInspector];Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;3;-627.7697,41.24741;Float;False;Property;_Cutoff;Cutoff;0;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;29;-247.902,-145.5568;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.999;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;12;-586.2991,532.2642;Float;False;Property;_InterstitialBackgroundToggle;Interstitial Background Toggle;3;0;Create;True;0;0;False;0;1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;4;37.88078,85.41852;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;410.2394,53.33116;Float;False;True;2;Float;ASEMaterialInspector;0;1;ScreenWipe;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;0;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;14;0;13;3
WireConnection;18;0;14;0
WireConnection;18;1;15;0
WireConnection;20;0;17;0
WireConnection;21;0;16;0
WireConnection;23;0;19;2
WireConnection;23;1;13;2
WireConnection;23;2;18;0
WireConnection;22;0;19;1
WireConnection;22;1;13;1
WireConnection;22;2;18;0
WireConnection;25;0;22;0
WireConnection;25;1;23;0
WireConnection;24;0;21;0
WireConnection;24;1;20;0
WireConnection;26;0;24;0
WireConnection;26;1;25;0
WireConnection;27;1;26;0
WireConnection;31;0;10;0
WireConnection;31;1;30;0
WireConnection;32;0;31;0
WireConnection;28;0;6;0
WireConnection;28;1;27;0
WireConnection;29;0;32;0
WireConnection;12;0;11;0
WireConnection;12;1;28;0
WireConnection;4;0;29;0
WireConnection;4;1;3;0
WireConnection;4;2;2;0
WireConnection;4;3;2;0
WireConnection;4;4;12;0
WireConnection;1;0;4;0
ASEEND*/
//CHKSM=8C7C8CD2970B66DDC48DDD202AD95EB3DA9B6523