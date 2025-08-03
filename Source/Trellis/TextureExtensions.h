#pragma once

#include "Engine/Scripting/Script.h"
#include "Engine/Core/Math/Vector4.h"
#include "Engine/Content/Assets/Texture.h"
#include "Engine/Graphics/Textures/TextureData.h"
#include "TextureSampler.h"
#include "memory"

API_CLASS(Static, Internal) class TRELLIS_API TextureExtensions
{
    DECLARE_SCRIPTING_TYPE_MINIMAL(TextureExtensions);
    public:
        API_FUNCTION() static Color SampleAt(API_PARAM(this) TextureSampler& sampler, const Float2& uv);
        API_FUNCTION() static TextureSampler CreateSampler(API_PARAM(this) Texture& texture, int32 mipIndex = 0, int32 arrayIndex = 0);
        API_FUNCTION() static void DisposeSampler(API_PARAM(this) TextureSampler& sampler);
};