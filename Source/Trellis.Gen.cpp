// This code was auto-generated. Do not modify it.

#include "Engine/Scripting/BinaryModule.h"
#include "Trellis.Gen.h"

StaticallyLinkedBinaryModuleInitializer StaticallyLinkedBinaryModuleTrellis(GetBinaryModuleTrellis);

extern "C" BinaryModule* GetBinaryModuleTrellis()
{
    static NativeBinaryModule module("Trellis");
    return &module;
}
