using System;
using Shared.Enums.Equipment;
using UnityEngine;

namespace UI.Equipment
{
public class WeaponCraftingnavigation : MonoBehaviour
{
    static public Action<WeaponSlot, ushort, int>           OnEquipmentDisplay;
    static public Action<GameObject, EquipmentType, ushort> OnShowUI;
}
}