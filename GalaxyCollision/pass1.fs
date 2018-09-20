uniform float uv_fade;
uniform float uv_alpha;
flat in vec4 partColor;
in vec2 texcoord;
out vec4 FragColor;
void main(){

	float rad = length(texcoord);
	//make a circle
	if (rad > 1.){
		discard;
	}
	FragColor = partColor;
	FragColor.a *=  uv_fade *smoothstep(0.0,0.5,1-rad) * uv_alpha;	
}