// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using UnityEngine;
using UnityMasterData.Editor;
using UnityMasterData.Editor.Interfaces;

namespace Chsopoly.MasterData.Editor.Exporter
{
    public class IngameExporter : IMasterDataExporter
    {
        public void Export ()
        {
            MasterDataExporter.Export<DTO.Ingame.StageDTO, VO.Ingame.StageVO, uint>("Excels/Ingame.xlsx", "Assets/App/AddressableAssets/MasterData", "Ingame", "Stage");
        }
    }
}
