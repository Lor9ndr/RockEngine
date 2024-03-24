#version 460 core
#extension  GL_ARB_separate_shader_objects : enable


layout(location = 0) out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec2 TexCoords;
    vec3 Normal;
    vec3 ViewPos;
} fs_in;

struct Material {
    vec3 albedo;
    float metallic;
    float roughness;
    float ao;
};

//layout (std140, binding = 3) uniform MaterialData
//{
//    vec3 albedo;
//    float metallic;
//    float roughness;
//    float ao;
//}materialData;

layout (std140, binding = 4) uniform LightData
{
    vec3 lightColor;
    vec3 lightDirection;
    vec3 lightPosition;
    float intensity;

}lightData;

//MaterialData materialData = materialData(;
uniform Material material = Material(vec3(1), 1.0f, 1.0f,0.5f);

const vec3 outlineColor = vec3(1,0,0);

const float PI = 3.14159265359;

vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}  



float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a      = roughness*roughness;
    float a2     = a*a;
    float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;
	
    float num   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;
	
    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float num   = NdotV;
    float denom = NdotV * (1.0 - k) + k;
	
    return num / denom;
}
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2  = GeometrySchlickGGX(NdotV, roughness);
    float ggx1  = GeometrySchlickGGX(NdotL, roughness);
	
    return ggx1 * ggx2;
}

vec3 calculateDirectLight(vec3 fragPos, vec3 N, vec3 V, vec3 F0)
{
    vec3 L = normalize(lightData.lightPosition - fragPos);
    vec3 H = normalize(V + L);
    float distance    = length(lightData.lightPosition - fragPos);
    float attenuation = 1.0 / (distance * distance);
    vec3 radiance     = lightData.lightColor * attenuation * lightData.intensity;        
    
    // cook-torrance brdf
    float NDF = DistributionGGX(N, H, material.roughness);        
    float G   = GeometrySmith(N, V, L, material.roughness);      
    vec3 F    = fresnelSchlick(max(dot(H, V), 0.0), F0);       
    
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - material.metallic;	  
    
    vec3 numerator    = NDF * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.0001;
    vec3 specular     = numerator / denominator;  
        
    // add to outgoing radiance Lo
    float NdotL = max(dot(N, L), 0.0);                

    return (kD * material.albedo / PI + specular) * radiance * NdotL;
}


void main()
{
    vec3 N = normalize(fs_in.Normal);
    vec3 V = normalize(fs_in.ViewPos - fs_in.FragPos);
    
    vec3 F0 = vec3(0.04); 
    F0 = mix(F0, material.albedo, material.metallic);
    vec3 Lo = calculateDirectLight(fs_in.FragPos, N, V, F0);
    
    vec3 ambient = vec3(0.03) * material.albedo * material.ao;
    vec3 color = ambient + Lo;
	
    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0/2.2));  
   
    FragColor = vec4(color, 1.0);
}