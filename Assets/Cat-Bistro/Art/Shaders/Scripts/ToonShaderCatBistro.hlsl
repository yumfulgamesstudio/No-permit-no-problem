


void AdditionalLightsCalc_float(float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView, half4 Shadowmask,
						float PointLightBands, float SpotLightBands,
						out float3 Diffuse, out float3 Specular) {
        float3 diffuseColor = 0;
        float3 specularColor = 0;

    #ifndef SHADERGRAPH_PREVIEW
        Smoothness = exp2(10 * Smoothness + 1);
        uint pixelLightCount = GetAdditionalLightsCount();
        uint meshRenderingLayers = GetMeshRenderingLayer();

        #if USE_CLUSTER_LIGHT_LOOP
        for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++) {
            CLUSTER_LIGHT_LOOP_SUBTRACTIVE_LIGHT_CHECK
            Light light = GetAdditionalLight(lightIndex, WorldPosition, Shadowmask);
        #ifdef _LIGHT_LAYERS
            if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        #endif
            {
                if (PointLightBands <= 1 && SpotLightBands <= 1){
                    // Solid colour lights
                    diffuseColor += light.color * step(0.0001, light.distanceAttenuation * light.shadowAttenuation);
                }else{
                    // Multiple bands
                    diffuseColor += light.color * light.shadowAttenuation * ToonAttenuation(lightIndex, WorldPosition, PointLightBands, SpotLightBands);
                }
            }
        }
        #endif

        // For Foward+ the LIGHT_LOOP_BEGIN macro will use inputData.normalizedScreenSpaceUV, inputData.positionWS, so create that:
        InputData inputData = (InputData)0;
        float4 screenPos = ComputeScreenPos(TransformWorldToHClip(WorldPosition));
        inputData.normalizedScreenSpaceUV = screenPos.xy / screenPos.w;
        inputData.positionWS = WorldPosition;

        LIGHT_LOOP_BEGIN(pixelLightCount)
            Light light = GetAdditionalLight(lightIndex, WorldPosition, Shadowmask);
        #ifdef _LIGHT_LAYERS
            if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        #endif
            {
                if (PointLightBands <= 1 && SpotLightBands <= 1){
                    // Solid colour lights
                    diffuseColor += light.color * step(0.0001, light.distanceAttenuation * light.shadowAttenuation);
                }else{
                    // Multiple bands
                    diffuseColor += light.color * light.shadowAttenuation * ToonAttenuation(lightIndex, WorldPosition, PointLightBands, SpotLightBands);
                }
            }
        LIGHT_LOOP_END
    #endif

        Diffuse = diffuseColor;
        Specular = specularColor;
}