//TODO:
//It has to have some types as fields wrapped with 
// If it is an VertexShader
// Then we pass
// LayoutIn<SomeStruct> "NameOfVariable1";
// LayoutIn < SomeAnotherStruct > "NameOfVariable2";
// VSOut<SomeOutData> vs_out;
// 
// Then we can wrap
// Uniforms
// As 
// Uniform<someStructAlso> model;
// and so on
// Also i need to support uniformBuffers
// UniformBuffer<SomeStructAnother> myUniformBuffer
// 
// If it is an Fragment shader
// Then we have to declare in class Generic of<SomeOutData> that we passed in our VertexShader
// To match the linking of shaders
// Then we also support uniform buffers, uniforms and 
// Out as LayoutOut<SomeMyStruct> FragColor;
// All of that shaders has to implement void Main 
// that has to be also interptetated in to the GLSL shader code 

using RockEngine.ShaderGenerator;

var str = ShaderGenerator.GenerateShader<TestShader>();
Console.WriteLine(str);
Console.ReadKey();