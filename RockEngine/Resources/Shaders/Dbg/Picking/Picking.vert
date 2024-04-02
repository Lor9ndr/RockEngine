#version 460 core

layout (location = 0) in vec3 aPos;

layout (location = 5) in uint objIndex;

layout (location = 10) in mat4 instanceMatrix;

layout (std140, binding = 2) uniform CameraData
{
    mat4 View;
    mat4 Projection;
    vec3 ViewPos;
}cameraData;

out flat uint ObjectIndex;

void main()
{
   gl_Position = cameraData.Projection * cameraData.View * instanceMatrix * vec4(aPos,1.0);
   ObjectIndex = objIndex;
}  