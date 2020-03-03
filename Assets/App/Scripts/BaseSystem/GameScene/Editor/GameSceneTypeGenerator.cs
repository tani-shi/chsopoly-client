using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Chsopoly.Libs.Extensions;
using UnityEditor;
using UnityEngine;

namespace Chsopoly.BaseSystem.GameScene.Editor
{
    public class GameSceneTypeGenerator : AssetPostprocessor
    {
        private const string PathGameSceneTypeScript = "Assets/App/Scripts/GameScene/Generated/GameSceneType.cs";
        private const string PathGameSceneTypeHelperScript = "Assets/App/Scripts/BaseSystem/GameScene/Generated/GameSceneTypeHelper.cs";

        private const string TemplateGameSceneTypeScript = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.

namespace Chsopoly.GameScene
{
    public enum GameSceneType
    {
        None,
${TYPES}
    }
}";
        private const string TemplateGameSceneTypeHelperScript = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using Chsopoly.GameScene;

namespace Chsopoly.BaseSystem.GameScene
{
    public static class GameSceneTypeHelper
    {
        public static string GetAssetPath (GameSceneType type)
        {
            switch (type)
            {
${PATH_CASES}
                default:
                    throw new ArgumentOutOfRangeException (""Undefined GameSceneType was specified. "" + type.ToString ());
            }
        }
    }
}";

        [MenuItem ("Project/Game Scene/Update GameScene Types")]
        public static void Generate ()
        {
            GenerateTypeScript ();
            GenerateHelperScript ();

            AssetDatabase.Refresh ();
        }

        private static void GenerateTypeScript ()
        {
            var builder = new StringBuilder ();

            var tempList = new List<string> ();
            foreach (var guid in AssetDatabase.FindAssets ("t:prefab"))
            {
                var path = AssetDatabase.GUIDToAssetPath (guid);
                if (IsGameScenePath (path) && !tempList.Contains (path))
                {
                    tempList.Add (path);
                }
            }

            foreach (var path in tempList)
            {
                var name = Path.GetFileNameWithoutExtension (path).Replace ("Scene", "");
                builder.AppendLine (string.Format ("{0},", name).Indent (8));
            }

            Directory.CreateDirectory (Path.GetDirectoryName (PathGameSceneTypeScript));
            File.WriteAllText (PathGameSceneTypeScript, TemplateGameSceneTypeScript.Replace ("${TYPES}", builder.ToString ().RemoveLast ()));
        }

        private static void GenerateHelperScript ()
        {
            var caseBuilder = new StringBuilder ();
            var pathCaseBuilder = new StringBuilder ();

            var tempList = new List<string> ();
            foreach (var guid in AssetDatabase.FindAssets ("t:prefab"))
            {
                var path = AssetDatabase.GUIDToAssetPath (guid);
                if (IsGameScenePath (path) && !tempList.Contains (path))
                {
                    tempList.Add (path);
                }
            }

            foreach (var path in tempList)
            {
                var name = Path.GetFileNameWithoutExtension (path).Replace ("Scene", "");
                pathCaseBuilder.AppendLine (string.Format ("case GameSceneType.{0}:", name).Indent (16));
                pathCaseBuilder.AppendLine (string.Format ("return \"{0}\";", path).Indent (20));
            }

            var helperScript = TemplateGameSceneTypeHelperScript
                .Replace ("${CASES}", caseBuilder.ToString ().RemoveLast ())
                .Replace ("${PATH_CASES}", pathCaseBuilder.ToString ().RemoveLast ());
            Directory.CreateDirectory (Path.GetDirectoryName (PathGameSceneTypeHelperScript));
            File.WriteAllText (PathGameSceneTypeHelperScript, helperScript);
        }

        private static bool IsGameScenePath (string path)
        {
            return Regex.IsMatch (path, @".+/GameScene/(.*?)Scene.prefab");
        }
    }
}