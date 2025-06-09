#version 330

uniform mat4 uWorldTransform; 
uniform mat4 uViewProjection; 

in vec3 inPosition;

void main() {
    vec4 pos = vec4(inPosition, 1.0);
    gl_Position = uViewProjection * uWorldTransform * pos;
}
