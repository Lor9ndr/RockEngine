#version 460 core
#extension  GL_ARB_separate_shader_objects : enable


layout (location = 0) in vec3 aPos;

layout (std140, binding = 1) uniform TransformData
{
    mat4 Model;
}transformData;


layout (std140, binding = 2) uniform CameraData
{
    mat4 View;
    mat4 Projection;
    vec3 ViewPos;
}cameraData;

void main()
{
   gl_Position = cameraData.Projection * cameraData.View * transformData.Model * vec4(aPos,1.0);
}  