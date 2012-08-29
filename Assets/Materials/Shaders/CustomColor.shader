Shader "Custom/VertexColor" {

	Properties {
		_Ambient ("Main Color", Color) = (0.1,0.1,0.1,1)
		_Shininess ("Shininess", Range (0.01, 100)) = 0.7
	}

	Category {
		SubShader {
			Pass {
				Material {
					Diffuse (1,1,1,1)
					Ambient [_Ambient]
					Specular [_SpecColor]
				}
				Lighting On
			}
	
			Pass {
				Blend DstAlpha OneMinusDstAlpha
				Blend One One
				Lighting Off
				BindChannels { 
					Bind "Color", color
					Bind "Vertex", vertex
				}
			}
		}
	}
}
