using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Chsopoly.BaseSystem.AddressableAssets.Editor
{
    public class AddressableAssetsNameBuilder : AssetPostprocessor
    {
        const string AssetRootPath = "Assets/App/AddressableAssets";

        private static AddressableAssetSettings _defaultSettings = null;

        static AddressableAssetsNameBuilder ()
        {
            _defaultSettings = AddressableAssetSettingsDefaultObject.Settings;
        }

        [MenuItem ("Project/AddressableAssets/Refresh All AddressableAssets Entries")]
        static void BuildAllAddressableAssetsNames ()
        {
            if (_defaultSettings == null)
            {
                Debug.LogError ("Not found a addressable assets default settings file.");
                return;
            }
            var group = _defaultSettings.DefaultGroup;
            foreach (var entry in group.entries.ToArray ())
            {
                _defaultSettings.RemoveAssetEntry (entry.guid);
            }
            foreach (var path in Directory.GetFiles (AssetRootPath, "*.*", SearchOption.AllDirectories))
            {
                if (path.EndsWith (".meta"))
                {
                    continue;
                }

                SetAddressableAssetsName (path);
            }

            AssetDatabase.SaveAssets ();
        }

        static void OnPostprocessAllAssets (string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (_defaultSettings == null)
            {
                return;
            }

            foreach (var asset in importedAssets.Union (movedAssets))
            {
                if (!asset.StartsWith (AssetRootPath) || Directory.Exists (asset))
                {
                    continue;
                }
                SetAddressableAssetsName (asset);
            }

            AssetDatabase.SaveAssets ();
        }

        static void SetAddressableAssetsName (string path)
        {
            var group = _defaultSettings.DefaultGroup;
            var guid = AssetDatabase.AssetPathToGUID (path);
            var entry = group.GetAssetEntry (guid);
            if (entry != null)
            {
                entry.SetAddress (path);
            }
            else
            {
                _defaultSettings.CreateOrMoveEntry (guid, group, true);
            }
        }
    }
}