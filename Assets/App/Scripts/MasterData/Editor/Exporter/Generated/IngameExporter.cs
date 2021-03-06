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
            MasterDataExporter.Export<DTO.Ingame.CharacterDTO, VO.Ingame.CharacterVO, uint>("Excels/Ingame.xlsx", "Assets/App/AddressableAssets/MasterData", "Ingame", "Character");
            MasterDataExporter.Export<DTO.Ingame.GimmickDTO, VO.Ingame.GimmickVO, uint>("Excels/Ingame.xlsx", "Assets/App/AddressableAssets/MasterData", "Ingame", "Gimmick");
        }
    }
}
