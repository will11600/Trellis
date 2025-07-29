# Trellis: A Layer-Based 2D Rendering Library for Flax Engine

Trellis is a powerful C# library designed for programmatic 2D image composition and rendering within the Flax Engine. It provides a flexible, layer-based system, similar to digital content creation tools like Photoshop or GIMP, allowing developers to create and manipulate complex visual assets directly through code.

## Purpose and Scope

The primary goal of Trellis is to offer a robust framework for procedural image generation and dynamic texture manipulation. It moves beyond simple sprite rendering, giving developers fine-grained control over how textures are combined, blended, and rendered.
This makes it ideal for a variety of applications, including:

- Generating dynamic textures for UI elements.
- Creating complex procedural terrain maps or masks.
- Applying real-time visual effects to textures.
- Compositing multiple character customization textures (e.g., clothing, scars, tattoos) into a single material.

## Core Concepts

Trellis is built around a few key concepts:

- **Canvas:** The central object that manages the entire rendering process. It holds a collection of layers and orchestrates the final output.Layers: Individual layers within the canvas that can be blended together. Each layer can be enabled or disabled and has its own blend mode.
- **Channels:** Each layer contains channels (e.g., Red, Green, Blue, Alpha) that hold texture data. This allows for precise control over the color information being rendered.
- **Texture Importer:** A utility for loading standard image files (.png, .jpg, etc.) into a format that Trellis can use.
- **Pixel Mappers:** Control how textures are sampled when coordinates fall outside the standard [0, 1] range. Trellis includes Clamping (stretching the edge pixels) and Tiling (repeating the texture).
- **Blend Modes:** Define how the colors of one layer combine with the colors of the layers beneath it. The library is extensible, allowing for custom blend modes to be created.

## Getting Started & Usage

Here is a basic example of how to use Trellis to load two images, place them on separate layers, and render the result.

### Prerequisites

- A Flax Engine project.
- The Trellis library added to your project's Source directory.

### Example Code

This script demonstrates creating a canvas, loading textures, and rendering them.

```csharp
using FlaxEngine;
using System.Threading.Tasks;
using Trellis;
using Trellis.PixelMapping;

public class TrellisTerrainGenerator : Script
{
    private Terrain _terrain;

    [Header("Terrain Settings")]
    [Tooltip("The noise texture to use for the heightmap. The Red channel will be used for height.")]
    public Texture NoiseTexture;

    [Tooltip("The number of terrain patches to generate on the X and Z axes.")]
    public Int2 PatchesCount = new Int2(2, 2);

    [Tooltip("The maximum height of the generated terrain.")]
    public float MaxHeight = 3000f;

    public override void OnStart()
    {
        // 1. Create a new dynamic terrain actor and add it to the scene
        _terrain = new Terrain
        {
            HideFlags = HideFlags.DontSave,
            Name = "Trellis Procedural Terrain"
        };
        _terrain.Setup();

        // 2. Generate terrain using an async task to prevent game stalls
        Task.Run(GenerateTerrain);
    }

    private void GenerateTerrain()
    {
        if (NoiseTexture == null)
        {
            Debug.LogError("Noise Texture is not assigned!");
            return;
        }

        // 3. Set up the Trellis Canvas
        // The canvas size doesn't have to match the final terrain patch size.
        // Trellis will correctly sample from it using the viewport.
        var canvas = new Canvas(NoiseTexture.Size);

        // Load the noise texture into a Trellis-compatible format.
        var noiseTextureInfo = TextureImporter.FromTexture(NoiseTexture);

        // Create a layer for our noise and assign the texture to its Red channel.
        var noiseLayer = new Layer("Noise");
        noiseLayer.R = new TextureChannel(noiseTextureInfo, new TilingPixelMapper());
        
        canvas.Layers.Add(noiseLayer);

        // 4. Generate each terrain patch
        var chunkSize = _terrain.ChunkSize;
        var heightMapSize = chunkSize * FlaxEngine.Terrain.PatchEdgeChunksCount + 1;
        var heightMapLength = heightMapSize * heightMapSize;
        
        for (int patchZ = 0; patchZ < PatchesCount.Y; patchZ++)
        {
            for (int patchX = 0; patchX < PatchesCount.X; patchX++)
            {
                // 5. Define a Viewport
                // This tells Trellis which part of the canvas to render for this patch.
                // We are mapping the whole canvas (1x1 in UV space) to the total patches area.
                var viewport = new Viewport
                {
                    // The rectangle in UV space to sample from the canvas.
                    SampleRect = new Rectangle(
                        (float)patchX / PatchesCount.X,
                        (float)patchZ / PatchesCount.Y,
                        1.0f / PatchesCount.X,
                        1.0f / PatchesCount.Y
                    ),
                    // The output resolution for the render.
                    Width = heightMapSize,
                    Height = heightMapSize
                };

                // 6. Draw the canvas for the current viewport
                float[] pixelBuffer = canvas.Draw(viewport, Multisample.None);
                
                // 7. Extract height data from the rendered pixels (Red channel)
                var heightMap = new float[heightMapLength];
                for (int i = 0; i < heightMapLength; i++)
                {
                    // The pixel buffer contains R,G,B,A values sequentially. We only need R.
                    float redValue = pixelBuffer[i * 4]; 
                    heightMap[i] = redValue * MaxHeight;
                }

                // 8. Initialize the terrain patch with the generated heightmap
                var patchCoord = new Int2(patchX, patchZ);
                _terrain.AddPatch(ref patchCoord);
                _terrain.SetupPatchHeightMap(ref patchCoord, heightMap, null, true);
            }
        }

        // 9. Spawn the terrain into the scene on the main thread
        Scripting.InvokeOnUpdate(() => _terrain.Parent = Actor);
        Debug.Log("Trellis terrain generation complete.");
    }
    
    public override void OnDestroy()
    {
        // Clean up the created terrain actor
        if (_terrain)
        {
            Destroy(_terrain);
        }
        base.OnDestroy();
    }
}
```
