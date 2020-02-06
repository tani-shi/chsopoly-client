using UnityEditor;
using UnityMasterData.Editor;

namespace Chsopoly.BaseSystem.MasterData.Editor
{
    static class MasterDataBuilder
    {
        [MenuItem ("Project/Master Data/Generate Master Data Scripts")]
        static void GenerateMasterDataScripts ()
        {
            MasterDataClassGenerator.GenerateAllDataScripts (
                "Excels", "Assets/App/Scripts", "Assets/App/AddressableAssets", "Chsopoly");
        }

        [MenuItem ("Project/Master Data/Export Master Data Assets")]    
        static void ExportMasterDataAssets ()
        {
            MasterDataExporter.Export ();
        }
    }
}