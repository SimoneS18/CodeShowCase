using System;
using Shared.Enums.Equipment;
using UI.Equipment;
using UI.Hero.Ability;
using UnityEngine;

namespace UI.Hero
{
public class HeroPopupNavigation : MonoBehaviour
{
    // Ability
    static public Action<ushort, ushort, ushort> OnShowAbilityUI;

    // reset ST
    static public Action<ushort> OnShowResetSTUI;

    // equip Equipment
    static public Action<ushort, WeaponSlot> OnShowEquipmentToEquipUI;

    [SerializeField]
    private GameObject _abilityInfo;

    [SerializeField]
    private GameObject _restInfo;

    [SerializeField]
    private GameObject _equipEquipmentInfo;

    private void Awake()
    {
        OnShowAbilityUI          += AbilityNavigation_OnShowAbilityUI;
        OnShowResetSTUI          += ResetNavigation_OnShowResetSTUI;
        OnShowEquipmentToEquipUI += EquipmentNavigation_OnShowEquipingUI;
    }

    private void OnDestroy()
    {
        OnShowAbilityUI          -= AbilityNavigation_OnShowAbilityUI;
        OnShowResetSTUI          -= ResetNavigation_OnShowResetSTUI;
        OnShowEquipmentToEquipUI -= EquipmentNavigation_OnShowEquipingUI;
    }

    private void AbilityNavigation_OnShowAbilityUI(ushort heroId, ushort skId, ushort abilityId)
    {
        _abilityInfo.SetActive(true);
        _abilityInfo.GetComponent<AbilitySelectedUI>().Init(heroId, skId, abilityId);
    }

    private void ResetNavigation_OnShowResetSTUI(ushort heroId)
    {
        _restInfo.SetActive(true);
        _restInfo.GetComponent<ResetSKUI>().Init(heroId);
    }

    private void EquipmentNavigation_OnShowEquipingUI(ushort heroId, WeaponSlot weaponSlot)
    {
        _equipEquipmentInfo.SetActive(true);
        _equipEquipmentInfo.GetComponent<EquipEquipmentUI>().Init(heroId, weaponSlot);
    }
}
}