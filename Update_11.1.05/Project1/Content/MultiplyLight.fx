Texture2D SceneTex;
Texture2D LightTex; // optional ถ้าใช้ mask

float2 PlayerLightPos;       // normalized 0..1
float PlayerLightRadius;
float PlayerLightAlpha;

float2 PointLightPos;     // normalized 0..1
float PointLightRadius;
float PointLightAlpha;
float4 PointLightColor; 

float2 CampfireLightPos;     // normalized 0..1
float CampfireLightRadius;
float CampfireLightAlpha;
float4 CampfireColor;        // สี campfire

float AmbientIntensity;      // ความมืดรอบฉาก (0..1)

sampler s0 = sampler_state { Filter = POINT; };

float4 ps_main(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 sceneCol = SceneTex.Sample(s0, texCoord);

    // Player light (ขาว)
    float2 diffP = texCoord - PlayerLightPos;
    float distP = length(diffP);
    float lightFactorP = saturate(1 - distP / PlayerLightRadius) * PlayerLightAlpha;
    float3 playerLight = lerp(AmbientIntensity, 1.0, lightFactorP).xxx;

    // Campfire light (สีเฉพาะตรงไฟ)
    float2 diffC = texCoord - CampfireLightPos;
    float distC = length(diffC);
    float lightFactorC = saturate(1 - distC / CampfireLightRadius) * CampfireLightAlpha;
    float3 campfireLight = CampfireColor.rgb * lightFactorC;

    // PointLight
    float2 diffPL = texCoord - PointLightPos;
    float distPL = length(diffPL);
    float lightFactorPL = saturate(1 - distPL / PointLightRadius) * PointLightAlpha;
    float3 pointLight = PointLightColor * lightFactorPL;

    // รวมทั้งสาม
    float3 finalLight = saturate(playerLight + campfireLight + pointLight);

    return sceneCol * float4(finalLight, 1.0);
}

technique tech_main
{
    pass P0 { PixelShader = compile ps_2_0 ps_main(); }
}
