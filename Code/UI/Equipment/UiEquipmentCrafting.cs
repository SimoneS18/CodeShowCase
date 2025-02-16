using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Data.PlayerBase;
using Shared.Enums.Assets;
using Shared.Scriptables.Buildings;
using Shared.Scriptables.Equipment;
using Shared.Utils;
using Sirenix.OdinInspector;
using UI.BaseBuilding.Buildings;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Equipment
{
public class UiEquipmentCrafting : BuildingUI
{
    static public Action<ushort, int> OnWorkshopFirstOpenUpdateDisplay;

    [Title("Spawning")]
    [SerializeField, Required] private GameObject[] _craftEquipmentSpawnPoints;

    [Title("Prefabs")]
    [SerializeField, Required] private GameObject _equipmentPrefab;

    private ushort     _buildingId;
    private ushort     _index;
    private int        _queueTotalCount;
    private byte       _level;
    private Button     _selectedButton;
    private ScrollRect _selectedScrollRect;

    public override void Init(ushort placeableId, BasePlaceableData data, int index, AdditionalIcon icon)
    {
        Debugger.print("INITIAL SPAWNED");
        // get equipment SO
        if (!Assets.GetBuilding(placeableId, out PlaceableSO _))
            return;

        List<EquipmentSO> equipmentSos = Resources.LoadAll<EquipmentSO>(Assets.equipmentFile).Where(x => x.Enabled).ToList();

        _buildingId = placeableId;
        _index      = (ushort)index;

        // clear spawned equipment
        int spawnSelectionPoint = _craftEquipmentSpawnPoints.Length;

        for (int i = 0; i < spawnSelectionPoint; i++)
        {
            foreach (Transform child in _craftEquipmentSpawnPoints[i].transform)
                Destroy(child.gameObject);
        }

        // spawn equipment
        foreach (EquipmentSO equipment in equipmentSos)
        {
            foreach (EquipmentRarityDetails equipmentRarityDetails in equipment.RarityDetails)
            {
                GameObject selection = Instantiate(_equipmentPrefab, _craftEquipmentSpawnPoints[(int)equipment.EquipmentType].transform);

                selection.GetComponent<UiEquipmentButton>().Init(equipment.Id, 
                                                                 equipmentRarityDetails.Rarity, 
                                                                 _craftEquipmentSpawnPoints[(int)equipment.EquipmentType].GetComponent<ToggleGroup>());
            }
        }

        OnWorkshopFirstOpenUpdateDisplay?.Invoke(_buildingId, _index);
    }
}
}