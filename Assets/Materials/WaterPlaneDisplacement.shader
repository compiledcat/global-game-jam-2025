Shader "Custom/WaveShader"
{
    Properties
    {
        _AmbientColour ("Ambient Colour", Color) = (1,1,1,1)
        _DiffuseColour ("Diffuse Colour", Color) = (1,1,1,1)
        _SpecularColour ("Specular Colour", Color) = (1,1,1,1)
        _SpecularHighlightSize ("Specular Highlight Size", Range(0,300)) = 10
        _Alpha ("Opacity", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #define NUM_WAVES 8
            #define PI 3.141592653589

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _AmbientColour;
            float4 _DiffuseColour;
            float4 _SpecularColour;
            float _SpecularHighlightSize;
            float _Alpha;
            float _Amplitudes[NUM_WAVES];
            float2 _Directions[NUM_WAVES];
            float _Frequencies[NUM_WAVES];
            float _Speeds[NUM_WAVES];
            float _CPUTime;

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
            
                //Calculate world space position
                float3 worldPos = mul(UNITY_MATRIX_M, float4(input.positionOS.xyz, 1.0)).xyz;

                //Calculate wave height using sum of sines
                float height = 0.0;
                for (int i = 0; i < NUM_WAVES; i++)
                {
                    float2 dir = _Directions[i];
                    float freq = _Frequencies[i];
                    float speed = _Speeds[i];
                    float amp = _Amplitudes[i];
                    float time = _CPUTime;

                    float phase = dot(dir, worldPos.xz) * freq + time * speed;
                    height += amp * sin(phase);
                }

                worldPos.y = height;

                //Transform to clip space
                output.positionCS = TransformWorldToHClip(worldPos);
                output.positionWS = worldPos;

                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float partialDerivativeWrtX = 0.0;
                float partialDerivativeWrtZ = 0.0;
                
                for (int i = 0; i < NUM_WAVES; i++)
                {
                    float2 dir = _Directions[i];
                    float freq = _Frequencies[i];
                    float speed = _Speeds[i];
                    float amp = _Amplitudes[i];
                    float time = _CPUTime;

                    //Recalculate phase in the fragment shader to avoid v2f interpolation of non-linear sin function
                    float phase = dot(dir, input.positionWS.xz) * freq + time * speed;
                    
                    //Sum up partial derivatives for normal calculation
                    float derivative = freq * amp * cos(phase);
                    partialDerivativeWrtX += derivative * dir.x;
                    partialDerivativeWrtZ += derivative * dir.y;
                }

                float3 normal = normalize(float3(-partialDerivativeWrtX, 1, -partialDerivativeWrtZ));

	            //Lambertian diffuse
                Light light = GetMainLight();
                float diffuseStrength = pow(dot(normalize(light.direction), normal), 2);
	            diffuseStrength = clamp(diffuseStrength, 0, diffuseStrength);

	            //Blinn phong specular
	            float3 normalisedFragPosToCameraPos = normalize(_WorldSpaceCameraPos - input.positionWS);
	            float3 halfVector = normalize(light.direction + normalisedFragPosToCameraPos);
	            float specularStrength = pow(dot(halfVector, normal), _SpecularHighlightSize);
	            specularStrength = clamp(specularStrength, 0, specularStrength);
	            float3 specularColour = float3(190/255.0, 238/255.0, 237/255.0);

	            //Ambient
	            float ambientStrength = 0.3;

	            //Final color
	            float3 finalColour = clamp(diffuseStrength * _DiffuseColour + specularStrength * _SpecularColour + ambientStrength * _AmbientColour, 0, 1);

                return float4(finalColour, _Alpha);
            }
            ENDHLSL
        }
    }
}