in vec2 texCoord;
uniform sampler2D initTexture;
uniform sampler2D dataTexture;

uniform bool reset;
uniform bool frozen;
uniform float deltat;
uniform float gravitationalStrength;


uniform int dataTexWidth;
uniform int dataTexHeight;

uniform vec3 initialPosition;
uniform float massRatio;
uniform float incidentAngle;
uniform float incomingSpeed;
uniform bool isCorotating;
uniform float incomingInclination;
uniform float staticInclination;

out vec4 FragColor;
const float smoothingLength = 750.0;

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
	// if reset then populate dataTexture with initTexture
	int i = int(gl_FragCoord.x)%(dataTexWidth/2);
	int j = int(gl_FragCoord.y);
	bool isPos = gl_FragCoord.x < dataTexWidth/2;
	float ratio = clamp(massRatio, 0.5,1.5);
	bool isMovingGal = (j>ratio*256);
	float mass=massRatio;
		if (isMovingGal) 
	{
		mass = 2.0 - massRatio;
	}	
	if (reset) {
		vec4 particleData = texelFetch(initTexture,ivec2(i,j),0);
		vec3 particlePos = ratio*20000*(vec3(particleData.r+particleData.b/256.,particleData.g+particleData.a/256.,0.0)-vec3(0.5));
		particlePos.z=0.;
		vec3 incomingVel = 1000.*normalize(-initialPosition) * incomingSpeed * cos(radians(incidentAngle));
		incomingVel += 1000.*normalize(cross(-initialPosition, vec3(0,0,1))) * incomingSpeed * sin(radians(incidentAngle));
		vec3 particleVel = 1000.*normalize(vec3(particlePos.y,-particlePos.x,0))*sqrt(gravitationalStrength * mass / max(length(particlePos),smoothingLength));
		if (isMovingGal) 
		{
			mat4 rotMat = getRotationMatrix(initialPosition, radians(incomingInclination));
			particlePos *= (2.0-ratio)/ratio;
			particleVel = 1000.*normalize(vec3(particlePos.y,-particlePos.x,0))*sqrt(gravitationalStrength * mass / max(length(particlePos),smoothingLength));
			particlePos = (rotMat*vec4(particlePos,1.)).xyz;
			particlePos+=initialPosition;

			if (!isCorotating)
			{
				particleVel *= -1.;
			}
			particleVel = (rotMat*vec4(particleVel,0.)).xyz;
			particleVel += incomingVel;
		} else {
			// Rotation for the static Galaxy
			mat4 rotMat = getRotationMatrix(vec3(0.,1.,0.), radians(staticInclination));
			particlePos = (rotMat*vec4(particlePos,1.)).xyz;
			particleVel = (rotMat*vec4(particleVel,0.)).xyz;
		}
		if (i==0 && j==0) 
		{
			particlePos =  vec3(0.);
			particleVel= vec3(0.);
		} 
		if (i==1 && j==0) 
		{
			particlePos = initialPosition;
			particleVel = incomingVel;
		} 
		if (isPos)
		{
			FragColor = vec4(particlePos,1.);
		}
		else
		{
			FragColor = vec4(0.001*particleVel,1.);//velocity
		}
	}
	else if (frozen)
	{
		FragColor = texelFetch(dataTexture, ivec2(gl_FragCoord.xy),0);
	}
	else
	{
		vec3 particlePos = texelFetch(dataTexture,ivec2(i,j),0).rgb;
		vec3 particleVel = texelFetch(dataTexture,ivec2(i+dataTexWidth/2,j),0).rgb;
		vec3 gal1Pos = texelFetch(dataTexture,ivec2(0,0),0).rgb;
		vec3 gal2Pos = texelFetch(dataTexture,ivec2(1,0),0).rgb;
		float R1 = max(length(particlePos-gal1Pos),smoothingLength);
		float R2 = max(length(particlePos-gal2Pos),smoothingLength);
		vec3 particleAcc=vec3(0.);
		if (i==0 && j==0) 
		{
			vec3 a2 = gravitationalStrength*(2.-massRatio)*(gal2Pos - particlePos)/(R2*R2*R2);
			particleAcc = a2;
		} 
		if (i==1 && j==0) 
		{
			vec3 a1 = gravitationalStrength*(massRatio)*(gal1Pos - particlePos)/(R1*R1*R1);
			particleAcc = a1;
		} 
		if (i>1 || j!=0) 
		{
			vec3 a1 = gravitationalStrength*massRatio*(gal1Pos - particlePos)/(R1*R1*R1);
			vec3 a2 = gravitationalStrength*(2.-massRatio)*(gal2Pos - particlePos)/(R2*R2*R2);
			particleAcc = a1+a2;
		}
		particleVel+=particleAcc*deltat;
		particlePos+=particleVel*deltat + 0.5*particleAcc*deltat*deltat;
		if (isPos)
		{
			FragColor = vec4(particlePos,1.);
		}
		else
		{
			FragColor = vec4(particleVel,0);//velocity
		}

	}
}