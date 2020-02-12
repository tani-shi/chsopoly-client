using UnityEditor;
using UnityEngine;

namespace Chsopoly.Editor
{
    public class TextureCustomImporter : AssetPostprocessor
    {
        void OnPreprocessTexture ()
        {
            var importer = (TextureImporter) assetImporter;
            var settings = new TextureImporterSettings ();
            importer.ReadTextureSettings (settings);
            settings.spriteMode = (int) SpriteImportMode.Single;
            settings.textureType = TextureImporterType.Sprite;
            importer.SetTextureSettings (settings);

            importer.npotScale = TextureImporterNPOTScale.None;
        }
    }
}