#pragma once

#include "Engine/Level/Actor.h"
#include "Engine/Content/Assets/Texture.h"
#include "Engine/Graphics/PixelFormatExtensions.h"
#include "Engine/Graphics/PixelFormatSampler.h"
#include "Engine/Graphics/Textures/TextureData.h"
#include "Engine/Core/Math/Vector2.h"
#include "memory"

API_STRUCT() struct TRELLIS_API TextureSampler
{
    DECLARE_SCRIPTING_TYPE_MINIMAL();
    public:
        TextureSampler();
        const PixelFormatSampler* sampler;
        FlaxStorage::LockData* lock;
        Float2 size;
        TextureMipData* mipData;
};
