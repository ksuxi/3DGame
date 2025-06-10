#version 330

uniform sampler2D uTexture; 
uniform vec3 uAmbientColor; 


struct DirectionalLight
{
    vec3 mDirection;    
    vec3 mDiffuseColor; 
};
uniform DirectionalLight uDirLight;

in vec2 fragTexCoord; 
in vec3 fragNormal;   

out vec4 outColor;

void main() {
    
    vec3 N = normalize(fragNormal);                
    vec3 L = normalize(-uDirLight.mDirection);     

    
    
    vec3 Lambert = uAmbientColor; 
    
    float NdotL = dot(N, L);
    if (NdotL > 0)
    {
        
        vec3 Diffuse = uDirLight.mDiffuseColor * NdotL; 
        Lambert += Diffuse;
    }
    
    outColor = texture(uTexture, fragTexCoord) * vec4(Lambert, 1.0f);
}
