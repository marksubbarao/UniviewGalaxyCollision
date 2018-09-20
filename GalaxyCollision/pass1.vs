#extension GL_EXT_gpu_shader4:enable 

in int i;
in int j;

uniform sampler2D dataTexture;
uniform int dataTexHeight;
uniform int dataTexWidth;
uniform vec3 initialPosition;
uniform float massRatio;
out int xInd;
out int yInd;

void main(){

    vec4 particlePos = texelFetch(dataTexture,ivec2(i,j),0);
	xInd = i;
	yInd = j;
	gl_Position = vec4(particlePos.rgb,1.);
	
}