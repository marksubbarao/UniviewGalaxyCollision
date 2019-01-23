in vec2 texcoord;

out vec2 texcoordFS;

uniform mat4 uv_modelViewProjectionMatrix;
uniform mat4 uv_modelViewInverseMatrix;

uniform vec3 initialPosition;
uniform float incidentAngle;
uniform float incomingSpeed;
uniform float incomingInclination;
uniform float arrowThickness = 1000.;

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

	vec3 forwardDir = normalize(-initialPosition);
	vec3 rightDir = normalize(cross(forwardDir, vec3(0,0,1)));
	float radAngle = radians(incidentAngle);
	vec3 velocity = 5000. * incomingSpeed * (cos(radAngle)*forwardDir + sin(radAngle)*rightDir);
	
	vec3 cameraPos = (uv_modelViewInverseMatrix * vec4(0,0,0,1)).xyz;
	vec3 dirToOrigin = normalize(cameraPos - initialPosition);
	vec3 normalToVel = abs(dot(dirToOrigin, normalize(velocity))) < .999 ? normalize(cross(dirToOrigin,velocity)) : vec3(0,0,1);
	
	vec3 modelPos = initialPosition + texcoord.x * velocity + (texcoord.y - 0.5) * (arrowThickness/2.) * normalToVel;
	
	gl_Position = uv_modelViewProjectionMatrix * vec4(modelPos,1.);
}