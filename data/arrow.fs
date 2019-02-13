in vec2 texcoordFS;

out vec4 FragColor;

uniform float uv_fade;
uniform float uv_alpha;
uniform bool reset;
uniform sampler2D arrowTex;

void main()
{
	if (!reset)
	{
		discard;
	}
	
	vec4 color = texture(arrowTex, texcoordFS);
	color.a *= smoothstep(0.,.1, length(color.rgb)) * uv_fade * uv_alpha;
	color.b=0.0;
	FragColor = color;
}