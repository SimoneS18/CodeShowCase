using System;
using Shared.Data.Equipment;
using Shared.Enums;
using Shared.Scriptables.Equipment;
using UI.Inventory;
using UnityEngine;

namespace UI.Hero
{
public class InventoryPopupNavigation : MonoBehaviour
{
    // ButtonPressed
    static public Action<ItemType, ushort>      OnButtonPressed;
    static public Action<EquipmentSO, Rarity>   OnBlueprintPressed;
    static public Action<ushort, EquipmentData> OnEquipmentPressed;

    [SerializeField]
    private GameObject _itemInfo;

    [SerializeField]
    private GameObject _equipmentInfo;

    private void Awake()
    {
        OnButtonPressed    += InventoryNavigation_ItemPressedUI;
        OnBlueprintPressed += InventoryNavigation_ItemPressedUI;
        OnEquipmentPressed += InventoryNavigation_ItemPressedUI;
    }

    private void OnDestroy()
    {
        OnButtonPressed    -= InventoryNavigation_ItemPressedUI;
        OnBlueprintPressed -= InventoryNavigation_ItemPressedUI;
        OnEquipmentPressed -= InventoryNavigation_ItemPressedUI;
    }

    private void InventoryNavigation_ItemPressedUI(ItemType type, ushort id)
    {
        _itemInfo.SetActive(true);
        _itemInfo.GetComponent<InventoryInformationUI>().Init(type, id);
    }

    private void InventoryNavigation_ItemPressedUI(EquipmentSO equipmentSO, Rarity rarity)
    {
        _itemInfo.SetActive(true);
        _itemInfo.GetComponent<InventoryInformationUI>().Init(equipmentSO, rarity);
    }

    private void InventoryNavigation_ItemPressedUI(ushort id, EquipmentData data)
    {
        _equipmentInfo.SetActive(true);
        _equipmentInfo.GetComponent<InventoryEquipmentInformationUI>().Init(id, data);
    }
}
}