﻿#version 460 core
#extension  GL_ARB_separate_shader_objects : enable

layout(location = 0) out vec4 FragColor;

in vec3 color;


void main()
{
    FragColor = vec4(color, 1);
}
