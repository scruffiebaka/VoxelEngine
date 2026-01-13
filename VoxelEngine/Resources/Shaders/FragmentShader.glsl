#version 330 core

in vec3 Normal;
in vec2 TexCoords;

out vec4 FragColor;

uniform sampler2D texture0;
uniform vec3 lightDir;

void main()
{
    vec3 albedo = texture(texture0, TexCoords).rgb;

    float diff = max(dot(normalize(Normal), normalize(lightDir)), 0.0);

    vec3 ambient = 0.6 * albedo;
    vec3 diffuse = 1.0 * diff * albedo;

    FragColor = vec4(ambient + diffuse, 1.0);
}