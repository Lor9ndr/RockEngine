#version 460 core

flat in uint ObjectIndex;

out vec3 FragColor;

void main()
{
     FragColor = vec3(float(ObjectIndex), 0, float(gl_PrimitiveID + 1));
} 