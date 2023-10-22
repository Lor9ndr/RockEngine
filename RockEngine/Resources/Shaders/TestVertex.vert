#version 460 core
#extension  GL_ARB_separate_shader_objects : enable

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;


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

out VS_OUT {
    vec3 FragPos;
    vec2 TexCoords;
    vec3 Normal;
    vec3 ViewPos;
} vs_out;


void main()
{
   
    vs_out.FragPos = vec3(transformData.Model * vec4(aPos, 1.0));
    vs_out.Normal = mat3(transpose(inverse(transformData.Model))) * aNormal;
    vs_out.TexCoords = aTexCoords;
    vs_out.ViewPos = cameraData.ViewPos;
    
    gl_Position = cameraData.Projection * cameraData.View * transformData.Model * vec4(aPos,1.0);
}
