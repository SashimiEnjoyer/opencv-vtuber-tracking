// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "HairShaders/HairShader2_Advanced"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_EdgeRimLight("EdgeRimLight", Range( 0 , 4)) = 0
		_TranslucentEffect("TranslucentEffect", Range( 0 , 100)) = 0.25
		_HairStrand_Coloring("HairStrand_Coloring", 2D) = "white" {}
		_StrandTone1("StrandTone1", Color) = (0.9044118,0.7517808,0.4788062,0)
		_StrandTone2("StrandTone2", Color) = (0.375,0.2175076,0.09650736,0)
		_BaseColorGloss("BaseColor-Gloss", Color) = (0.8455882,0.8076825,0.6839316,0.522)
		_BaseTone_Alpha("BaseTone_Alpha", 2D) = "white" {}
		_BaseTipColorPower("BaseTipColor-Power", Color) = (1,0,0,0)
		_AlphaLevel("AlphaLevel", Range( 0 , 2)) = 2
		_VariaitonFromBase("VariaitonFromBase", Range( 0 , 1)) = 0.8
		_NormalMap("NormalMap", 2D) = "bump" {}
		_BumpPower("BumpPower", Range( 0 , 1)) = 0.5
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_MainHighlight_Color("MainHighlight_Color", Color) = (0,0,0,0)
		_MainHighlightPosition("Main Highlight Position", Range( -1 , 3)) = 0
		_MainHighlightStrength("Main Highlight Strength", Range( 0 , 2)) = 0.25
		_MainHighlightExponent("Main Highlight Exponent", Range( 0 , 1000)) = 0.2
		_SecondaryHighlightColor("Secondary Highlight Color", Color) = (0.8862069,0.8862069,0.8862069,0)
		_SecondaryHighlightPosition("Secondary Highlight Position", Range( -1 , 3)) = 0
		_SecondaryHighlightStrength("Secondary Highlight Strength", Range( 0 , 2)) = 0.25
		_SecondaryHighlightExponent("Secondary Highlight Exponent", Range( 0 , 1000)) = 0.2
		_Spread("Spread", Range( -2 , 2)) = 0
		_Noise("Noise", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend One Zero , SrcAlpha DstColor
		
		AlphaToMask On
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform float _BumpPower;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float4 _StrandTone1;
		uniform sampler2D _HairStrand_Coloring;
		uniform float4 _HairStrand_Coloring_ST;
		uniform float4 _StrandTone2;
		uniform float4 _BaseTipColorPower;
		uniform float4 _BaseColorGloss;
		uniform sampler2D _BaseTone_Alpha;
		uniform float4 _BaseTone_Alpha_ST;
		uniform float4 _MainHighlight_Color;
		uniform float _MainHighlightPosition;
		uniform float _Noise;
		uniform float _Spread;
		uniform float _MainHighlightExponent;
		uniform float _MainHighlightStrength;
		uniform float _SecondaryHighlightPosition;
		uniform float _SecondaryHighlightExponent;
		uniform float _SecondaryHighlightStrength;
		uniform float4 _SecondaryHighlightColor;
		uniform float _VariaitonFromBase;
		uniform float _Metallic;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _EdgeRimLight;
		uniform float _TranslucentEffect;
		uniform float _AlphaLevel;
		uniform float _Cutoff = 0.5;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 tex2DNode7 = UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _BumpPower );
			float3 lerpResult113 = lerp( float3(0.5,0.5,1) , tex2DNode7 , _BumpPower);
			float3 normalizeResult15 = normalize( lerpResult113 );
			o.Normal = normalizeResult15;
			float2 uv_HairStrand_Coloring = i.uv_texcoord * _HairStrand_Coloring_ST.xy + _HairStrand_Coloring_ST.zw;
			float4 tex2DNode315 = tex2D( _HairStrand_Coloring, uv_HairStrand_Coloring );
			float4 blendOpSrc323 = ( _StrandTone1 * tex2DNode315 );
			float4 blendOpDest323 = ( ( 1.0 - tex2DNode315 ) * _StrandTone2 );
			float4 temp_output_323_0 = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc323 ) * ( 1.0 - blendOpDest323 ) ) ));
			float2 uv_BaseTone_Alpha = i.uv_texcoord * _BaseTone_Alpha_ST.xy + _BaseTone_Alpha_ST.zw;
			float4 tex2DNode1 = tex2D( _BaseTone_Alpha, uv_BaseTone_Alpha );
			float4 lerpResult293 = lerp( _BaseTipColorPower , _BaseColorGloss , pow( tex2DNode1.g , ( _BaseTipColorPower.a * 10.0 ) ));
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 T200 = cross( ase_worldTangent , ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float2 uv_TexCoord330 = i.uv_texcoord * float2( 1200,10 );
			float simplePerlin2D329 = snoise( uv_TexCoord330 );
			float lerpResult328 = lerp( tex2DNode7.g , simplePerlin2D329 , _Noise);
			float NoiseFX312 = ( ( tex2DNode7.g + lerpResult328 ) * _Spread );
			float4 appendResult305 = (float4(ase_worldlightDir.x , ( ( _MainHighlightPosition + NoiseFX312 ) + ase_worldlightDir.y ) , ase_worldlightDir.z , 0.0));
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float4 normalizeResult78 = normalize( ( appendResult305 + float4( ase_worldViewDir , 0.0 ) ) );
			float4 H93 = normalizeResult78;
			float dotResult95 = dot( float4( T200 , 0.0 ) , H93 );
			float dotTH94 = dotResult95;
			float sinTH97 = sqrt( ( 1.0 - ( dotTH94 * dotTH94 ) ) );
			float smoothstepResult103 = smoothstep( -1.0 , 0.0 , dotTH94);
			float dirAtten102 = smoothstepResult103;
			float dotResult279 = dot( ase_worldlightDir , ase_worldNormal );
			float clampResult290 = clamp( ( dotResult279 * dotResult279 * dotResult279 ) , 0.0 , 1.0 );
			float lightZone285 = clampResult290;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 temp_output_268_0 = ( _MainHighlight_Color * ( pow( sinTH97 , _MainHighlightExponent ) * _MainHighlightStrength * dirAtten102 * lightZone285 ) * ase_lightColor * ase_lightColor.a );
			float4 appendResult241 = (float4(ase_worldlightDir.x , ( ( _SecondaryHighlightPosition + NoiseFX312 ) + ase_worldlightDir.y ) , ase_worldlightDir.z , 0.0));
			float4 normalizeResult246 = normalize( ( appendResult241 + float4( ase_worldViewDir , 0.0 ) ) );
			float4 HL2247 = normalizeResult246;
			float dotResult249 = dot( HL2247 , float4( T200 , 0.0 ) );
			float DotTHL2252 = dotResult249;
			float sinTHL2256 = sqrt( ( 1.0 - ( DotTHL2252 * DotTHL2252 ) ) );
			float4 temp_output_265_0 = ( ( pow( sinTHL2256 , _SecondaryHighlightExponent ) * _SecondaryHighlightStrength * dirAtten102 * lightZone285 ) * _SecondaryHighlightColor * ase_lightColor * ase_lightColor.a );
			float4 temp_cast_4 = (1.0).xxxx;
			float4 lerpResult144 = lerp( temp_cast_4 , tex2DNode1 , _VariaitonFromBase);
			float4 clampResult275 = clamp( ( ( lerpResult293 + temp_output_268_0 + temp_output_265_0 ) * lerpResult144 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			o.Albedo = ( temp_output_323_0 * clampResult275 ).rgb;
			o.Emission = ( temp_output_268_0 + temp_output_265_0 ).rgb;
			o.Metallic = ( tex2DNode1.r * _Metallic );
			o.Smoothness = _BaseColorGloss.a;
			float fresnelNdotV31 = dot( ase_worldNormal, ase_worldlightDir );
			float fresnelNode31 = ( -0.9 + 0.45 * pow( 1.0 - fresnelNdotV31, _EdgeRimLight ) );
			float fresnelNdotV205 = dot( ase_worldNormal, ase_worldlightDir );
			float fresnelNode205 = ( -0.1 + 0.9 * pow( 1.0 - fresnelNdotV205, 1.51 ) );
			float4 temp_output_39_0 = ( clampResult275 * ( ( 1.0 - fresnelNode31 ) * fresnelNode205 ) );
			float4 clampResult223 = clamp( temp_output_39_0 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			o.Translucency = ( temp_output_323_0 * ( clampResult223 * _TranslucentEffect ) ).rgb;
			float temp_output_4_0 = pow( ( tex2DNode1.a * tex2DNode1.a ) , _AlphaLevel );
			o.Alpha = temp_output_4_0;
			#if UNITY_PASS_SHADOWCASTER
			clip( temp_output_4_0 - _Cutoff );
			#endif
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustom keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			AlphaToMask Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandardCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16204
431;311;1377;732;-828.0188;327.1451;2.577205;True;False
Node;AmplifyShaderEditor.RangedFloatNode;114;-68.51652,344.9161;Float;False;Property;_BumpPower;BumpPower;19;0;Create;True;0;0;False;0;0.5;0.974;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;330;-442.6895,663.8214;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1200,10;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;261.4775,232.6432;Float;True;Property;_NormalMap;NormalMap;18;0;Create;True;0;0;False;0;None;49d56c07f751a7943850c72eb004d29d;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;331;-17.27001,771.6235;Float;False;Property;_Noise;Noise;30;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;329;-120.1106,651.8582;Float;False;Simplex2D;1;0;FLOAT2;0.5,0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;328;508.5192,592.8309;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;300;5.488602,441.9024;Float;False;Property;_Spread;Spread;29;0;Create;True;0;0;False;0;0;-0.176;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;311;722.4737,330.1337;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;299;896.8495,466.2956;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;312;1240.075,552.5405;Float;False;NoiseFX;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;110;-3870.355,-768.6287;Float;False;2882.904;1761.292;BaseSpec;51;314;109;267;261;259;102;106;104;286;108;285;258;260;107;262;105;103;290;256;97;289;132;255;254;134;279;280;284;99;253;252;98;94;95;249;200;96;248;251;93;78;197;198;77;79;17;305;304;298;303;313;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;313;-3422.639,-661.655;Float;True;312;NoiseFX;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;303;-3837.977,-578.5528;Float;False;Property;_MainHighlightPosition;Main Highlight Position;22;0;Create;True;0;0;False;0;0;-0.42;-1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;25;-3378.034,-934.0533;Float;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;244;-3194.839,-1160.074;Float;False;Property;_SecondaryHighlightPosition;Secondary Highlight Position;26;0;Create;True;0;0;False;0;0;-0.28;-1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;298;-3054.06,-489.056;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;304;-2731.667,-511.2789;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;306;-2809.895,-1131.572;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;305;-2566.309,-562.2385;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;243;-2639.251,-1096.934;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;17;-2918.777,-752.0027;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;77;-2438.491,-699.083;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;241;-2389.892,-1077.019;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;245;-2103.269,-1022.311;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NormalizeNode;78;-2240.693,-704.3386;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexTangentNode;79;-3749.171,-83.18208;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;198;-3694.432,119.9227;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;246;-1910.786,-1029.374;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CrossProductOpNode;197;-2980.105,-376.9262;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;-2038.877,-674.1208;Float;False;H;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;96;-2742.952,-240.0831;Float;False;93;H;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;200;-2691.425,-380.3658;Float;False;T;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;247;-1442.094,-1029.97;Float;False;HL2;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;248;-2806.46,468.8795;Float;False;247;HL2;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;251;-2790.21,571.2278;Float;False;200;T;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;95;-2443.141,-344.7966;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;94;-2266.202,-344.448;Float;False;dotTH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;249;-2470.71,495.7466;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;252;-2225.851,501.045;Float;False;DotTHL2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;-2757.617,-130.2655;Float;False;94;dotTH;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;280;-3693.497,528.7839;Float;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;284;-3699.141,715.5179;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;253;-1936.716,504.4529;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-2466.195,-129.2428;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;134;-2262.328,-161.3798;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;279;-3054.784,196.8065;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;254;-1735.666,520.1598;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SqrtOpNode;255;-1533.047,518.5891;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SqrtOpNode;132;-2060.191,-205.6331;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;289;-2866.36,215.7459;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;256;-1333.569,518.5888;Float;False;sinTHL2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;97;-1856.662,-211.6534;Float;False;sinTH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;290;-2646.9,258.096;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;103;-2436.922,17.70354;Float;False;3;0;FLOAT;-1;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;297;-682.1694,-646.6733;Float;False;Constant;_Float2;Float 2;19;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;262;-1864.757,693.3102;Float;False;Property;_SecondaryHighlightExponent;Secondary Highlight Exponent;28;0;Create;True;0;0;False;0;0.2;78.77;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;285;-2468.629,205.9621;Float;False;lightZone;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;292;-749.9877,-878.1818;Float;False;Property;_BaseTipColorPower;BaseTipColor-Power;15;0;Create;True;0;0;False;0;1,0,0,0;0.3823529,0.09865172,0.03373703,0.222;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-2180.057,8.771833;Float;False;dirAtten;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-1799.963,149.5014;Float;False;Property;_MainHighlightExponent;Main Highlight Exponent;24;0;Create;True;0;0;False;0;0.2;417;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;257;-1832.492,927.9147;Float;False;256;sinTHL2;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;-1718.307,71.29671;Float;False;97;sinTH;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;258;-1483.864,638.1011;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-819.8445,-1.959623;Float;True;Property;_BaseTone_Alpha;BaseTone_Alpha;14;0;Create;True;0;0;False;0;None;d278b4721754dd64ebf3ae49a82076b9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;286;-1650.262,391.5542;Float;False;285;lightZone;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;259;-1846.26,784.1027;Float;False;Property;_SecondaryHighlightStrength;Secondary Highlight Strength;27;0;Create;True;0;0;False;0;0.25;1.108;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;106;-1444.119,-10.46254;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-1794.714,230.1034;Float;False;Property;_MainHighlightStrength;Main Highlight Strength;23;0;Create;True;0;0;False;0;0.25;0.79;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;295;-489.3649,-648.4099;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;260;-1824.733,860.6061;Float;False;102;dirAtten;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;-1759.939,316.7973;Float;False;102;dirAtten;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-872.1382,-576.4238;Float;False;Property;_BaseColorGloss;BaseColor-Gloss;13;0;Create;True;0;0;False;0;0.8455882,0.8076825,0.6839316,0.522;0.2586207,0.1469004,0.09698275,0.478;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;294;-284.2756,-494.4894;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;264;-1219.661,851.2394;Float;False;Property;_SecondaryHighlightColor;Secondary Highlight Color;25;0;Create;True;0;0;False;0;0.8862069,0.8862069,0.8862069,0;0.2763448,0.1472329,0.07701414,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;261;-1224.201,693.6574;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;314;-1193.189,506.5096;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-1276.799,-28.76422;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;267;-1502.458,-301.618;Float;False;Property;_MainHighlight_Color;MainHighlight_Color;21;0;Create;True;0;0;False;0;0,0,0,0;0.7482758,0.5152488,0.2530933,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;268;-932.6133,-322.5575;Float;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;209;-365.4094,858.3539;Float;False;Constant;_Bias1;Bias1;19;0;Create;True;0;0;False;0;-0.9;-0.939;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;293;-86.6779,-655.597;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;210;-365.4094,938.3539;Float;False;Constant;_Scale1;Scale1;21;0;Create;True;0;0;False;0;0.45;0.459;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;265;-801.9812,596.2473;Float;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;211;-363.4138,1011.591;Float;False;Property;_EdgeRimLight;EdgeRimLight;8;0;Create;True;0;0;False;0;0;2.25;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-637.1672,-236.493;Float;False;Constant;_Float1;Float 1;12;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;145;-647.7953,-153.0608;Float;False;Property;_VariaitonFromBase;VariaitonFromBase;17;0;Create;True;0;0;False;0;0.8;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;214;-371.3101,1334.191;Float;False;Constant;_Power2;Power2;25;0;Create;True;0;0;False;0;1.51;1.51;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;213;-364.4909,1151.055;Float;False;Constant;_Bias2;Bias2;23;0;Create;True;0;0;False;0;-0.1;0.148;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;144;-246.6127,-167.5198;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;31;935.6188,721.78;Float;False;Standard;WorldNormal;LightDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;212;-365.4909,1238.055;Float;False;Constant;_Scale2;Scale2;24;0;Create;True;0;0;False;0;0.9;0.905;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;273;179.0984,-470.9795;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;205;948.2639,937.8943;Float;False;Standard;WorldNormal;LightDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;204;1228.807,723.0435;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;1414.221,-320.6941;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;315;1992.316,-529.3062;Float;True;Property;_HairStrand_Coloring;HairStrand_Coloring;10;0;Create;True;0;0;False;0;None;7824c364858be85418a76b325d7de738;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;1521.662,805.4762;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;275;2047.285,-184.0741;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;319;2374.467,-653.2032;Float;False;Property;_StrandTone1;StrandTone1;11;0;Create;True;0;0;False;0;0.9044118,0.7517808,0.4788062,0;0.9044118,0.7517808,0.4788062,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;320;2456.458,-116.1353;Float;False;Property;_StrandTone2;StrandTone2;12;0;Create;True;0;0;False;0;0.375,0.2175076,0.09650736,0;0.375,0.2175076,0.09650736,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;321;2646.344,-417.3043;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;2259.973,348.4926;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;112;277.38,40.46222;Float;False;Constant;_Vector0;Vector 0;10;0;Create;True;0;0;False;0;0.5,0.5,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;217;2432.967,756.2677;Float;False;Property;_TranslucentEffect;TranslucentEffect;9;0;Create;True;0;0;False;0;0.25;1;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;223;2602.774,496.0627;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;324;2680.344,-520.3043;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;322;2887.344,-399.3043;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;224;2851.923,642.8226;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;29;584.7592,-105.4266;Float;False;Property;_Metallic;Metallic;20;0;Create;True;0;0;False;0;0;0.364;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-966.4499,247.1846;Float;False;Property;_AlphaLevel;AlphaLevel;16;0;Create;True;0;0;False;0;2;1.2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;323;3147.344,-481.3043;Float;False;Screen;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;113;914.9343,77.11578;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-304.6455,93.49822;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;1206.498,-145.8922;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;291;2597.496,266.8466;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;316;3132.524,-67.27289;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;318;3354.03,443.5029;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;15;1105.024,37.289;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.AbsOpNode;222;2538.647,637.7281;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;4;3521.55,804.2662;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4104.841,646.0191;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;HairShaders/HairShader2_Advanced;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;1;5;False;-1;2;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;0;1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;0;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;5;114;0
WireConnection;329;0;330;0
WireConnection;328;0;7;2
WireConnection;328;1;329;0
WireConnection;328;2;331;0
WireConnection;311;0;7;2
WireConnection;311;1;328;0
WireConnection;299;0;311;0
WireConnection;299;1;300;0
WireConnection;312;0;299;0
WireConnection;298;0;303;0
WireConnection;298;1;313;0
WireConnection;304;0;298;0
WireConnection;304;1;25;2
WireConnection;306;0;244;0
WireConnection;306;1;313;0
WireConnection;305;0;25;1
WireConnection;305;1;304;0
WireConnection;305;2;25;3
WireConnection;243;0;306;0
WireConnection;243;1;25;2
WireConnection;77;0;305;0
WireConnection;77;1;17;0
WireConnection;241;0;25;1
WireConnection;241;1;243;0
WireConnection;241;2;25;3
WireConnection;245;0;241;0
WireConnection;245;1;17;0
WireConnection;78;0;77;0
WireConnection;246;0;245;0
WireConnection;197;0;79;0
WireConnection;197;1;198;0
WireConnection;93;0;78;0
WireConnection;200;0;197;0
WireConnection;247;0;246;0
WireConnection;95;0;200;0
WireConnection;95;1;96;0
WireConnection;94;0;95;0
WireConnection;249;0;248;0
WireConnection;249;1;251;0
WireConnection;252;0;249;0
WireConnection;253;0;252;0
WireConnection;253;1;252;0
WireConnection;99;0;98;0
WireConnection;99;1;98;0
WireConnection;134;0;99;0
WireConnection;279;0;280;0
WireConnection;279;1;284;0
WireConnection;254;0;253;0
WireConnection;255;0;254;0
WireConnection;132;0;134;0
WireConnection;289;0;279;0
WireConnection;289;1;279;0
WireConnection;289;2;279;0
WireConnection;256;0;255;0
WireConnection;97;0;132;0
WireConnection;290;0;289;0
WireConnection;103;0;98;0
WireConnection;285;0;290;0
WireConnection;102;0;103;0
WireConnection;258;0;257;0
WireConnection;258;1;262;0
WireConnection;106;0;105;0
WireConnection;106;1;107;0
WireConnection;295;0;292;4
WireConnection;295;1;297;0
WireConnection;294;0;1;2
WireConnection;294;1;295;0
WireConnection;261;0;258;0
WireConnection;261;1;259;0
WireConnection;261;2;260;0
WireConnection;261;3;286;0
WireConnection;109;0;106;0
WireConnection;109;1;108;0
WireConnection;109;2;104;0
WireConnection;109;3;286;0
WireConnection;268;0;267;0
WireConnection;268;1;109;0
WireConnection;268;2;314;0
WireConnection;268;3;314;2
WireConnection;293;0;292;0
WireConnection;293;1;2;0
WireConnection;293;2;294;0
WireConnection;265;0;261;0
WireConnection;265;1;264;0
WireConnection;265;2;314;0
WireConnection;265;3;314;2
WireConnection;144;0;143;0
WireConnection;144;1;1;0
WireConnection;144;2;145;0
WireConnection;31;1;209;0
WireConnection;31;2;210;0
WireConnection;31;3;211;0
WireConnection;273;0;293;0
WireConnection;273;1;268;0
WireConnection;273;2;265;0
WireConnection;205;1;213;0
WireConnection;205;2;212;0
WireConnection;205;3;214;0
WireConnection;204;0;31;0
WireConnection;3;0;273;0
WireConnection;3;1;144;0
WireConnection;219;0;204;0
WireConnection;219;1;205;0
WireConnection;275;0;3;0
WireConnection;321;0;315;0
WireConnection;39;0;275;0
WireConnection;39;1;219;0
WireConnection;223;0;39;0
WireConnection;324;0;319;0
WireConnection;324;1;315;0
WireConnection;322;0;321;0
WireConnection;322;1;320;0
WireConnection;224;0;223;0
WireConnection;224;1;217;0
WireConnection;323;0;324;0
WireConnection;323;1;322;0
WireConnection;113;0;112;0
WireConnection;113;1;7;0
WireConnection;113;2;114;0
WireConnection;28;0;1;4
WireConnection;28;1;1;4
WireConnection;30;0;1;1
WireConnection;30;1;29;0
WireConnection;291;0;268;0
WireConnection;291;1;265;0
WireConnection;316;0;323;0
WireConnection;316;1;275;0
WireConnection;318;0;323;0
WireConnection;318;1;224;0
WireConnection;15;0;113;0
WireConnection;222;0;39;0
WireConnection;4;0;28;0
WireConnection;4;1;5;0
WireConnection;0;0;316;0
WireConnection;0;1;15;0
WireConnection;0;2;291;0
WireConnection;0;3;30;0
WireConnection;0;4;2;4
WireConnection;0;7;318;0
WireConnection;0;9;4;0
WireConnection;0;10;4;0
ASEEND*/
//CHKSM=D4F12C89B518E68096D6CEFC5AE3AF4440E60C08