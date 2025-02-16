using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Shared.Data.PlayerBase;
using Shared.Enums.Assets;
using Shared.Enums.Equipment;
using Shared.Scriptables.Buildings;
using Shared.Scriptables.Equipment;
using Sirenix.OdinInspector;
using UI.BaseBuilding.Buildings;
using UI.Toggle;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Equipment
{
public class UiEquipmentUpgrading : BuildingUI
{
   // static public Action<EquipmentType> OnTabPressed;
    static public Action<ushort, int>   OnArsenalFirstOpenUpdateDisplay;

    [Title("Appearance")]
    [SerializeField, Required] private GameObject _displayNoEquipment;
    [SerializeField, Required] private GameObject[] _turnOffWhenNoEquipment;

    [Title("Spawning")]
    [SerializeField, Required] private Transform[] _craftEquipmentSpawnPoints;

    [Title("Category Selection")]
    [SerializeField, Required] private GameObject _categoryPanel;
    [SerializeField, Required] private GameObject _categoryTabs;

    [Title("Prefabs")]
    [SerializeField, Required] private GameObject _equipmentPrefab;
    
    private          ushort                                _buildingId;
    private          ushort                                _index;
    private          List<EquipmentSO>                     _equipmentSo      = new List<EquipmentSO>();
    private readonly List<Shared.Data.Equipment.Equipment> _craftedEquipment = new List<Shared.Data.Equipment.Equipment>();


    public override void Init(ushort placeableId, BasePlaceableData data, int index, AdditionalIcon icon)
    {
        // get equipment SO
        if (!Assets.GetBuilding(placeableId, out PlaceableSO placeableSo))
            return;
        
        _buildingId  = placeableId;
        _index       = (ushort)index;

        ClearOldData();

        // make sure list is empty
        _craftedEquipment.Clear();

        // get all equipment
        List<EquipmentSO> equipmentSos = Resources.LoadAll<EquipmentSO>(Assets.equipmentFile).Where(x => x.Enabled).ToList();

        // go through each equipment, and determine if one with that ID is found in the players DB
        foreach (EquipmentSO equipment in equipmentSos)
            if (PlayerManager.Equipment.Equipments.GetEquipment(equipment.Id, out Shared.Data.Equipment.Equipment equipmentData))
                _craftedEquipment.Add(equipmentData);
 
        // either spawn the available equipments to upgrade or update the UI to display no equipment ot upgrade
        if (_craftedEquipment.Count > 0)
            InstantiateEquipments();
        else
            ToggleValueChanged(true);
        
        OnArsenalFirstOpenUpdateDisplay?.Invoke(_buildingId, _index);
    }

    private void ClearOldData()
    {
        int spawnSelectionPoint = _craftEquipmentSpawnPoints.Length;

        for (int i = 0; i < spawnSelectionPoint; i++)
        {
            foreach (Transform child in _craftEquipmentSpawnPoints[i])
                Destroy(child.gameObject);
        }
    }

    private void InstantiateEquipments()
    {
        List<int> equipmentsList = new List<int>();

        foreach (Shared.Data.Equipment.Equipment craftingEquipment in _craftedEquipment)
        {
            for (int j = 0; j < craftingEquipment.Items.Count; j++)
            {
                Assets.GetEquipment(craftingEquipment.Id, out EquipmentSO equipmentSo);
                int        categoryIndex = (int)equipmentSo.EquipmentType;
                GameObject heroButton    = Instantiate(_equipmentPrefab, _craftEquipmentSpawnPoints[categoryIndex]);

                heroButton.GetComponent<UiEquipmentUpgradeButton>()
                          .Init(craftingEquipment.Id, j, craftingEquipment.Items[j],
                                _craftEquipmentSpawnPoints[categoryIndex].GetComponent<ToggleGroup>());
                
                equipmentsList.Add(categoryIndex);
            }
        }

        for (int i = 0; i < Enum.GetValues(typeof(EquipmentType)).Length; i++)
        {
            UnityEngine.UI.Toggle toggle = _categoryTabs.transform.GetChild(i).GetComponent<UnityEngine.UI.Toggle>();

            if (!equipmentsList.Contains(i))
            {
                toggle.onValueChanged.AddListener(_ => ToggleValueChanged(true));

                if (toggle.isOn)
                    ToggleValueChanged(true);
            }
            else
            {
                toggle.onValueChanged.AddListener(_ => ToggleValueChanged(false));

                if (toggle.isOn)
                    ToggleValueChanged(false);
            }
        }
    }

    private void ToggleValueChanged(bool showing)
    {
        _displayNoEquipment.SetActive(showing);

        foreach (GameObject variable in _turnOffWhenNoEquipment)
            variable.SetActive(!showing);
    }
}
}