using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Chsopoly.Libs.Extensions;
using UnityEditor;
using UnityEngine;

namespace Chsopoly.BaseSystem.Popup.Editor
{
    public class PopupTypeGenerator : AssetPostprocessor
    {
        private const string PathPopupTypeScript = "Assets/App/Scripts/Popup/Generated/PopupType.cs";
        private const string PathPopupTypeHelperScript = "Assets/App/Scripts/BaseSystem/Popup/Generated/PopupTypeHelper.cs";

        private const string TemplatePopupTypeScript = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.

namespace Chsopoly.Popup
{
    public enum PopupType
    {
        None,
${TYPES}
    }
}";
        private const string TemplatePopupTypeHelperScript = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using Chsopoly.Popup;

namespace Chsopoly.BaseSystem.Popup
{
    public static class PopupTypeHelper
    {
        public static string GetAssetPath (PopupType type)
        {
            switch (type)
            {
${PATH_CASES}
                default:
                    throw new ArgumentOutOfRangeException (""Undefined PopupType was specified. "" + type.ToString ());
            }
        }
    }
}";

        [MenuItem ("Project/Popup/Update Popup Types")]
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
                if (IsPopupPath (path) && !tempList.Contains (path))
                {
                    tempList.Add (path);
                }
            }

            foreach (var path in tempList)
            {
                var name = Path.GetFileNameWithoutExtension (path).Replace ("Popup", "");;
                builder.AppendLine (string.Format ("{0},", name).Indent (8));
            }

            Directory.CreateDirectory (Path.GetDirectoryName (PathPopupTypeScript));
            File.WriteAllText (PathPopupTypeScript, TemplatePopupTypeScript.Replace ("${TYPES}", builder.ToString ().RemoveLast ()));
        }

        private static void GenerateHelperScript ()
        {
            var caseBuilder = new StringBuilder ();
            var pathCaseBuilder = new StringBuilder ();
            var loaderCaseBuilder = new StringBuilder ();
            var loaderPathCaseBuilder = new StringBuilder ();

            foreach (var guid in AssetDatabase.FindAssets ("t:prefab"))
            {
                var path = AssetDatabase.GUIDToAssetPath (guid);
                if (IsPopupPath (path))
                {
                    var name = Path.GetFileNameWithoutExtension (path).Replace ("Popup", "");
                    pathCaseBuilder.AppendLine (string.Format ("case PopupType.{0}:", name).Indent (16));
                    pathCaseBuilder.AppendLine (string.Format ("return \"{0}\";", path).Indent (20));
                }
            }

            var helperScript = TemplatePopupTypeHelperScript
                .Replace ("${CASES}", caseBuilder.ToString ().RemoveLast ())
                .Replace ("${PATH_CASES}", pathCaseBuilder.ToString ().RemoveLast ())
                .Replace ("${LOADER_CASES}", loaderCaseBuilder.ToString ().RemoveLast ());
            Directory.CreateDirectory (Path.GetDirectoryName (PathPopupTypeHelperScript));
            File.WriteAllText (PathPopupTypeHelperScript, helperScript);
        }

        private static bool IsPopupPath (string path)
        {
            return Regex.IsMatch (path, @".+/Popup/(.*?)Popup.prefab");
        }
    }
}