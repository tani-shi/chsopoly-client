using System.IO;
using System.Text;
using Chsopoly.Libs.Extensions;
using UnityEditor;

namespace Chsopoly.BaseSystem.Audio.Editor
{
    public static class AudioDefineGenerator
    {
        private const string PathRootAudioAssets = "Assets/App/AddressableAssets/Audio";
        private const string PathAudioDefineScript = "Assets/App/Scripts/Audio/Generated/AudioDefine.cs";
        private const string TemplateAudioDefineScript = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.

namespace Chsopoly.Audio
{
    public enum Bgm
    {
        None,
${BGM_DEFINES}
    }

    public enum Efx
    {
        None,
${EFX_DEFINES}
    }
}";

        [MenuItem ("Project/Audio/Update Audio Define")]
        public static void Generate ()
        {
            GenerateAudioDefineScript ();

            AssetDatabase.Refresh ();
        }

        private static void GenerateAudioDefineScript ()
        {
            var bgmBuilder = new StringBuilder ();
            var efxBuilder = new StringBuilder ();

            foreach (var path in Directory.GetFiles (PathRootAudioAssets, "*.*", SearchOption.AllDirectories))
            {
                if (path.EndsWith (".meta"))
                {
                    continue;
                }

                var name = Path.GetFileNameWithoutExtension (path);
                if (name.StartsWith (AudioSettings.BgmPrefix))
                {
                    bgmBuilder.AppendLine (name.Replace (AudioSettings.BgmPrefix, "").Snake2Pascal () + ",");
                }
                else if (name.StartsWith (AudioSettings.EfxPrefix))
                {
                    efxBuilder.AppendLine (name.Replace (AudioSettings.EfxPrefix, "").Snake2Pascal () + ",");
                }
            }

            var content = TemplateAudioDefineScript
                .Replace ("${BGM_DEFINES}", bgmBuilder.ToString ().Indent (8))
                .Replace ("${EFX_DEFINES}", efxBuilder.ToString ().Indent (8));
            Directory.CreateDirectory (Path.GetDirectoryName (PathAudioDefineScript));
            File.WriteAllText (PathAudioDefineScript, content);
        }
    }
}