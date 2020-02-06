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
        private const string kPathGameSceneTypeScript = "Assets/App/Scripts/GameScene/Generated/GameSceneType.cs";
        private const string kPathGameSceneTypeHelperScript = "Assets/App/Scripts/GameScene/Generated/GameSceneTypeHelper.cs";

        private const string kTemplateGameSceneTypeScript = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.

namespace Chsopoly.GameScene
{
    public enum GameSceneType
    {
        None,
${TYPES}
    }
}";
        private const string kTemplateGameSceneTypeHelperScript = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;

namespace Chsopoly.GameScene
{
    public static class GameSceneTypeHelper
    {
        public static string GetAssetPath (GameSceneType type)
        {
            switch (type)
            {
${PATH_CASES}
            }
            throw new ArgumentOutOfRangeException (""Undefined GameSceneType was found. "" + type.ToString ());
        }
    }
}";

        [MenuItem ("Project/Update GameScene Types")]
        public static void Generate ()
        {
            var typeBuilder = new StringBuilder ();
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
                typeBuilder.AppendLine (string.Format ("{0},", name).Indent (8));
                pathCaseBuilder.AppendLine (string.Format ("case GameSceneType.{0}:", name).Indent (16));
                pathCaseBuilder.AppendLine (string.Format ("return \"{0}\";", path).Indent (20));
            }

            File.WriteAllText (kPathGameSceneTypeScript, kTemplateGameSceneTypeScript.Replace ("${TYPES}", typeBuilder.ToString ().RemoveLast ()));

            var helperScript = kTemplateGameSceneTypeHelperScript
                .Replace ("${CASES}", caseBuilder.ToString ().RemoveLast ())
                .Replace ("${PATH_CASES}", pathCaseBuilder.ToString ().RemoveLast ());
            File.WriteAllText (kPathGameSceneTypeHelperScript, helperScript);

            AssetDatabase.Refresh ();
        }

        private static bool IsGameScenePath (string path)
        {
            return Regex.IsMatch (path, @".+/GameScene/(.*?)Scene.prefab");
        }
    }
}