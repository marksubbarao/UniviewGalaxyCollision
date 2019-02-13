uniform float uv_fade;
uniform float uv_alpha;
in vec2 texcoord;
out vec4 FragColor;
void main(){


	float rad = length(texcoord);
	FragColor = vec4(1,1,0,1);
	FragColor.a *=  uv_fade * uv_alpha;	
}