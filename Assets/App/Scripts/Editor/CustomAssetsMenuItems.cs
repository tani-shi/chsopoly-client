using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chsopoly.Editor
{
    static class CustomAssetsMenuItems
    {
        const int CreatePriority = 10000;

        [MenuItem ("Assets/Create/GameScene", false, CreatePriority)]
        static void GenerateVariantPrefab ()
        {
            EditorUtility.FocusProjectWindow ();

            var path = AssetDatabase.GenerateUniqueAssetPath (GetCurrentFolder () + "/NewScene.prefab");
            var source = Resources.Load ("BasePrefabs/GameScene");
            var prefab = (GameObject) PrefabUtility.InstantiatePrefab (source);
            var newObj = PrefabUtility.SaveAsPrefabAsset (prefab, path);
            GameObject.DestroyImmediate (prefab);

            Selection.activeObject = newObj;
        }

        [MenuItem ("Assets/Create/Empty uGUI Prefab", false, CreatePriority)]
        static void GenerateEmptyPrefab ()
        {
            EditorUtility.FocusProjectWindow ();

            var path = AssetDatabase.GenerateUniqueAssetPath (GetCurrentFolder () + "/New Prefab.prefab");
            var source = new GameObject ();
            source.AddComponent<RectTransform> ();
            source.AddComponent<CanvasRenderer> ();
            var newObj = PrefabUtility.SaveAsPrefabAsset (source, path);
            GameObject.DestroyImmediate (source);

            Selection.activeObject = newObj;
        }

        static string GetCurrentFolder ()
        {
            string filePath;
            if (Selection.assetGUIDs.Length == 0)
            {
                // No asset selected.
                filePath = "Assets";
            }
            else
            {
                // Get the path of the selected folder or asset.
                filePath = AssetDatabase.GUIDToAssetPath (Selection.assetGUIDs[0]);

                // Get the file extension of the selected asset as it might need to be removed.
                string fileExtension = Path.GetExtension (filePath);
                if (fileExtension != "")
                {
                    filePath = Path.GetDirectoryName (filePath);
                }
            }

            return filePath;
        }
    }
}