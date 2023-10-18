#version 460 core


uniform sampler2D sampler;

out vec4 FragColor;
  
in vec2 TexCoords;


void main()
{ 
    FragColor = texture(sampler, TexCoords);
}
