#version 460 core


uniform sampler2D Screen;

out vec4 FragColor;
  
in vec2 TexCoords;


void main()
{ 
    FragColor = texture(Screen, TexCoords);
}
