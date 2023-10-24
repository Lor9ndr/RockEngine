#version 460 core
#extension  GL_ARB_separate_shader_objects : enable


layout (std140, binding = 5) uniform PickingData
{
    uint gDrawIndex;
    uint gObjectIndex;
}pickingData;


out vec4 FragColor;

void main()
{
    FragColor = vec4(float(pickingData.gObjectIndex), float(pickingData.gDrawIndex), float(gl_PrimitiveID + 1),1);
} 