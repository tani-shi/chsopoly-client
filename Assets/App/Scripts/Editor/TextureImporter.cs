using UnityEditor;
using UnityEngine;

namespace Chsopoly.Editor
{
    public class TextureCustomImporter : AssetPostprocessor
    {
        private const string CharacterAnimationDirPath = "Assets/App/AddressableAssets/Textures/Character/Animation/";

        void OnPreprocessTexture ()
        {
            var importer = (TextureImporter) assetImporter;
            var settings = new TextureImporterSettings ();
            importer.ReadTextureSettings (settings);

            if (assetPath.StartsWith (CharacterAnimationDirPath))
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