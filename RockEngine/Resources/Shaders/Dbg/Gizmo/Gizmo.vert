#version 460 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;


layout (location = 10) in mat4 instanceMatrix;


layout (std140, binding = 2) uniform CameraData
{
    mat4 View;
    mat4 Projection;
    vec3 ViewPos;
}cameraData;

out vec3 color;

void main()
{
    vec4 pos = instanceMatrix * vec4(aPos, 1.0);
    color = aColor;
    gl_Position = cameraData.Projection * cameraData.View * pos;

}
