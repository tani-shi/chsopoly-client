using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;

namespace Chsopoly.BaseSystem.AddressableAssets.Editor
{
    public class AddressableAssetsNameBuilder : AssetPostprocessor
    {
        const string AssetRootPath = "Assets/App/AddressableAssets";

        [MenuItem ("Project/AddressableAssets/Refresh All AddressableAssets Entries")]
        static void BuildAllAddressableAssetsNames ()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = settings.DefaultGroup;
            foreach (var entry in group.entries.ToArray ())
            {
                settings.RemoveAssetEntry (entry.guid);
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
            foreach (var asset in importedAssets.Union (movedAssets))
            {
                if (!asset.StartsWith (AssetRootPath))
                {
                    continue;
                }
                SetAddressableAssetsName (asset);
            }

            AssetDatabase.SaveAssets ();
        }

        static void SetAddressableAssetsName (string path)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = settings.DefaultGroup;
            var guid = AssetDatabase.AssetPathToGUID (path);
            var entry = group.GetAssetEntry (guid);
            if (entry != null)
            {
                entry.SetAddress (path);
            }
            else
            {
                settings.CreateOrMoveEntry (guid, group, true);
            }
        }
    }
}