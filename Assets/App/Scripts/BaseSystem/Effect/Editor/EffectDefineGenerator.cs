using System.IO;
using System.Text;
using Chsopoly.Libs.Extensions;
using UnityEditor;

namespace Chsopoly.BaseSystem.Effect.Editor
{
    public static class EffectDefineGenerator
    {
        private const string PathRootEffectAssets = "Assets/App/AddressableAssets/Effect";
        private const string PathEffectDefineScript = "Assets/App/Scripts/Effect/Generated/EffectDefine.cs";
        private const string TemplateEffectDefineScript = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.

namespace Chsopoly.Effect
{
    public enum Eff
    {
        None,
${EFF_DEFINES}
    }
}";

        [MenuItem ("Project/Effect/Update Effect Define")]
        public static void Generate ()
        {
            GenerateEffectDefineScript ();

            AssetDatabase.Refresh ();
        }

        private static void GenerateEffectDefineScript ()
        {
            var effBuilder = new StringBuilder ();

            foreach (var path in Directory.GetFiles (PathRootEffectAssets, "*.*", SearchOption.AllDirectories))
            {
                if (path.EndsWith (".meta"))
                {
                    continue;
                }

                var name = Path.GetFileNameWithoutExtension (path);
                if (name.StartsWith (EffectSettings.EffectPrefix))
                {
                    effBuilder.AppendLine (name.Replace (EffectSettings.EffectPrefix, "").Snake2Pascal () + ",");
                }
            }

            var content = TemplateEffectDefineScript
                .Replace ("${EFF_DEFINES}", effBuilder.ToString ().Indent (8));
            Directory.CreateDirectory (Path.GetDirectoryName (PathEffectDefineScript));
            File.WriteAllText (PathEffectDefineScript, content);
        }
    }
}