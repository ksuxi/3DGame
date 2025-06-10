#version 330

uniform sampler2D uTexture; 
uniform vec3 uCameraPos;    
uniform float uSpecPower;   
uniform vec3 uAmbientColor; 


struct DirectionalLight
{
    vec3 mDirection;    
    vec3 mDiffuseColor; 
    vec3 mSpecColor;    
};
uniform DirectionalLight uDirLight;

in vec2 fragTexCoord; 
in vec3 fragNormal;   
in vec3 fragWorldPos; 

out vec4 outColor;

void main() {
    
    vec3 N = normalize(fragNormal);                
    vec3 L = normalize(-uDirLight.mDirection);     
    vec3 V = normalize(uCameraPos - fragWorldPos); 
    vec3 R = normalize(reflect(-L, N));            

    
    
    vec3 Phong = uAmbientColor; 
    
    float NdotL = dot(N, L);
    if (NdotL > 0)
    {
        
        vec3 Diffuse = uDirLight.mDiffuseColor * NdotL; 
        vec3 Specular = uDirLight.mSpecColor * pow(max(0.0, dot(R, V)), uSpecPower); 
        Phong += Diffuse + Specular;
    }
    
    outColor = texture(uTexture, fragTexCoord) * vec4(Phong, 1.0f);
}
