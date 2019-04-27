//MIT License

//Copyright(c) 2019 Antony Vitillo(a.k.a. "Skarredghost")

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

//The code is a modification of a tutorial by Ronja, that is released until a CC-0 license
//You can find his tutorial at this link https://www.ronja-tutorials.com/2018/07/01/postprocessing-depth.html
//And the related GitHub repository at this link https://github.com/ronja-tutorials/ShaderTutorials

Shader "Photo3D/Depth_Postprocessing"{
    //show values to edit in inspector
    Properties{
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
        _DepthMultiplier("Depth Multiplier", Float) = 1
    }

    SubShader{
        // markers that specify that we don't need culling 
        // or comparing/writing to the depth buffer
        Cull Off
        ZWrite Off 
        ZTest Always

        Pass{
            CGPROGRAM
            //include useful shader functions
            #include "UnityCG.cginc"

            //define vertex and fragment shader
            #pragma vertex vert
            #pragma fragment frag

            //the rendered screen so far
            sampler2D _MainTex;

            //the depth texture
            sampler2D _CameraDepthTexture;

			//the depth multiplier
			float _DepthMultiplier;

            //the object data that's put into the vertex shader
            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            //the data that's used to generate fragments and can be read by the fragment shader
            struct v2f{
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            //the vertex shader
            v2f vert(appdata v){
                v2f o;
                //convert the vertex positions from object space to clip space so they can be rendered
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            //the fragment shader
            fixed4 frag(v2f i) : SV_TARGET{
                //get depth from depth texture
                float depth = tex2D(_CameraDepthTexture, i.uv).r;

                //linear depth between camera and far clipping plane
                depth = Linear01Depth(depth);

				//if a pixel is beyond the far plane, draw it in black
				if (depth >= 1)
					return fixed4(0, 0, 0, 1);

                //depth as distance from camera in units 
				depth = depth * _DepthMultiplier;

				//return depth color
                return fixed4(1 - depth, 1 - depth, 1 - depth, 1);;
            }
            ENDCG
        }
    }
}
