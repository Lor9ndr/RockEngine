#version 460 core
#extension  GL_ARB_separate_shader_objects : enable



flat in uint ObjectIndex;
flat in uint DrawIndex;

out vec3 FragColor;

void main()
{
     FragColor = vec3(float(ObjectIndex), float(DrawIndex), float(gl_PrimitiveID + 1));
} 