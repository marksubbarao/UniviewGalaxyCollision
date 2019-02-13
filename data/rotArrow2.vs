in vec2 texcoord;

out vec2 texcoordFS;

uniform mat4 uv_modelViewProjectionMatrix;
uniform float staticInclination;
uniform float massRatio;
uniform bool isCorotating;

mat4 getRotationMatrix(vec3 axis, float angle)
{
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    
    return mat4(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,  0.0,
                oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,  0.0,
                oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c,           0.0,
                0.0,                                0.0,                                0.0,                                1.0);
}


void main()
{
	texcoordFS = texcoord;

	vec2 centeredCoords = 2.*texcoord - 1.;
	
	float ratio = clamp(massRatio, 0.5,1.5);
	float radius = 10000. * ratio;
	
	mat4 rotMat = getRotationMatrix(vec3(0.,1.,0.), radians(staticInclination));
	vec4 modelPos = (rotMat * vec4(radius*centeredCoords,0.,1.)) ;
	gl_Position = uv_modelViewProjectionMatrix * modelPos;
}