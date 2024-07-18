#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class TextureImporterSettings : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.textureType = TextureImporterType.Sprite; // 设置纹理类型为 Sprite
        // textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        TextureImporterPlatformSettings platformSettings = textureImporter.GetPlatformTextureSettings("webgl");
        platformSettings.overridden = true;
        // platformSettings.format = TextureImporterFormat.ASTC_8x8;
        platformSettings.compressionQuality = 100;
        textureImporter.SetPlatformTextureSettings(platformSettings);
    }
}

#endif