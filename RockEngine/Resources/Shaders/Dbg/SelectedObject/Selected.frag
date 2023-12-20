#version 460 core
#extension  GL_ARB_separate_shader_objects : enable

layout(location = 0) out vec4 FragColor;


uniform vec3 outlineColor = vec3(0.2, 1, 1);


void main()
{
    FragColor = vec4(outlineColor, 1);
}
