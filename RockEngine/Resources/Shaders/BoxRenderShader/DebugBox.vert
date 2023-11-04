#version 460 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;

layout (std140, binding = 1) uniform TransformData
{
    mat4 model;
}transformData;

layout (std140, binding = 2) uniform CameraData
{
    mat4 view;
    mat4 projection;
    vec3 viewPos;
}cameraData;

out vec3 color;


void main()
{
    color = aColor;
    gl_Position = cameraData.projection * cameraData.view * transformData.model * vec4(aPos, 1.0);
}
