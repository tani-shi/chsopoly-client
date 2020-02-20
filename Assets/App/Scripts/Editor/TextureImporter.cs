using UnityEditor;
using UnityEngine;

namespace Chsopoly.Editor
{
    public class TextureCustomImporter : AssetPostprocessor
    {
        private const string CharacterDirPath = "Assets/App/AddressableAssets/Textures/Character/";

        void OnPreprocessTexture ()
        {
            var importer = (TextureImporter) assetImporter;
            var settings = new TextureImporterSettings ();
            importer.ReadTextureSettings (settings);

            if (assetPath.StartsWith (CharacterDirPath))
            {
                settings.spriteMode = (int) SpriteImportMode.Multiple;
            }
            else
            {
                settings.spriteMode = (int) SpriteImportMode.Single;
            }

            settings.textureType = TextureImporterType.Sprite;
            importer.SetTextureSettings (settings);

            importer.npotScale = TextureImporterNPOTScale.None;
        }
    }
}