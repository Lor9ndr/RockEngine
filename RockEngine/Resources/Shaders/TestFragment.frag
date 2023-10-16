#version 460 core
#extension  GL_ARB_separate_shader_objects : enable


layout(location = 0) out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec2 TexCoords;
    vec3 Normal;
    vec3 ViewPos;
} fs_in;

layout (std140, binding = 3) uniform MaterialData
{
    vec3 ambientColor;
    vec3 diffuseColor;
    vec3 specularColor;
    float shininess;
}materialData;

layout (std140, binding = 4) uniform LightData
{
    vec3 lightColor;
    vec3 lightDirection;
    vec3 lightPosition;
    float intensity;

}lightData;

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
    vec3 L = normalize(-lightData.lightDirection);
    vec3 viewDir = normalize(V);
    vec3 reflectDir = reflect(-L, N);
    vec3 H = normalize(viewDir + L); // Calculate the half vector
    // Calculate the diffuse color using Lambert's law
    float NdotL = max(dot(N, L), 0.0);
    vec3 diffuse = NdotL * materialData.diffuseColor;

    // Calculate the specular color using the Cook-Torrance model
    float roughness = 1.0 - materialData.shininess; // Inverse of shininess is roughness
    vec3 F = fresnelSchlick(max(dot(N, viewDir), 0.0), F0);
    float D = DistributionGGX(N, H, roughness);
    float G = GeometrySmith(N, viewDir, L, roughness);
    vec3 specular = (D * G * F) / (4.0 * max(dot(N, viewDir), 0.0) * max(dot(N, L), 0.0));

    // Calculate the ambient color
    vec3 ambient = materialData.ambientColor;

    // Combine the ambient, diffuse, and specular colors
    vec3 color = (ambient + diffuse + specular) * lightData.lightColor * lightData.intensity;

    return color;
}


void main()
{
    vec3 N = normalize(fs_in.Normal);
    vec3 V = normalize(fs_in.ViewPos - fs_in.FragPos);
    
    vec3 color = vec3(0);
    color += calculateDirectLight(fs_in.FragPos, N, V, vec3(0.04)); // Example F0 value
    
    FragColor = vec4(color, 1.0);
}





