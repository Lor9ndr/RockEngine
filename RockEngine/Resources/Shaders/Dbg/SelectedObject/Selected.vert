#version 460 core
#extension  GL_ARB_separate_shader_objects : enable
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 10) in mat4 instanceMatrix;


layout (std140, binding = 2) uniform CameraData
{
    mat4 View;
    mat4 Projection;
    vec3 ViewPos;
}cameraData;



void main()
{
    vec4 pos = instanceMatrix  * vec4(aPos + aNormal * 0.08, 1.0);
    gl_Position = cameraData.Projection * cameraData.View * pos;

}
