using System.Collections.Generic;
using System.Linq;
using Managers;
using Shared.Enums.Equipment;
using Shared.Scriptables.Equipment;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Equipment
{
/// <summary>
///     following script is used when a hero equipment button is pressed to equip equipment
/// </summary>
public class EquipEquipmentUI : MonoBehaviour
{
    [Header("Spawning Points"), SerializeField]
    private GameObject _spawnPoint;

    [Header("Prefab"), SerializeField]
    private GameObject EquipmentToggle;

    private List<EquipmentSO> _equipmentSO = new List<EquipmentSO>();

    private ushort _heroId;

    private WeaponSlot _weaponSlot;

    internal void Init(ushort heroId, WeaponSlot weaponSlot)
    {
        _weaponSlot = weaponSlot;
        _heroId     = heroId;

        _equipmentSO = Assets.Equipment.Values.Where(x => x.WeaponSlot == _weaponSlot).ToList();

        UpdateDisplay();
    }

    private void ClearEquipment()
    {
        foreach (Transform child in _spawnPoint.transform)
            Destroy(child.gameObject);
    }

    private void UpdateDisplay()
    {
        ClearEquipment();

        for (int i = _equipmentSO.Count - 1; i >= 0; i--)
        {
            if (!PlayerManager.Equipment.Equipments.GetEquipment(_equipmentSO[i].Id, out Shared.Data.Equipment.Equipment _))
            {
                _equipmentSO.RemoveAt(i);
                return;
            }

            // now determine if has something equipped already (change button appearance
            PlayerManager.Equipment.Equipments.GetEquipment(_equipmentSO[i].Id, out Shared.Data.Equipment.Equipment equipmentData);

            for (int j = 0; j < equipmentData.Items.Count; j++)
            {
                if (equipmentData.Items[j].Equiped == $"{_heroId}" || equipmentData.Items[j].Equiped == null ||
                    equipmentData.Items[j].Equiped == "")
                {
                    // spawn in button as is equipped on hero
                    GameObject toggleButton = Instantiate(EquipmentToggle, _spawnPoint.GetComponent<Transform>());

                    toggleButton.GetComponent<UiEquipmentEquipButton>()
                                .Init(equipmentData.Items[j],
                                      j,
                                      equipmentData.Id,
                                      _heroId,
                                      _weaponSlot,
                                      _spawnPoint.GetComponent<ToggleGroup>());
                }
            }
        }
    }
}
}