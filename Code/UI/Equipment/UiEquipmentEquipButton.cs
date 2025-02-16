using System;
using Managers;
using Shared.Data.Equipment;
using Shared.Data.Hero;
using Shared.Enums.Equipment;
using Shared.Scriptables.Equipment;
using Shared.Utils;
using Shared.Utils.Identifiers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

//#STODO
namespace UI.Equipment
{
public class UiEquipmentEquipButton : MonoBehaviour
{
    static public Action<EquipmentData, int, ushort, ushort, WeaponSlot> OnToggleEnabled;

    [SerializeField]
    private TMP_Text _upgrade;

    [SerializeField]
    private GameObject _setImage;

    [SerializeField]
    private GameObject _onHeroText;

    private EquipmentData _equipment;
    private ushort        _equipmentId;
    private ushort        _heroId;
    private int           _index;
    private WeaponSlot    _weaponSlot;

    public void Init(EquipmentData equipmentData,
                     int           index,
                     ushort        equipmentId,
                     ushort        heroId,
                     WeaponSlot    weaponSlot,
                     ToggleGroup   toggleGroup)
    {
        Assets.GetEquipment(equipmentId, out EquipmentSO _);
        gameObject.GetComponent<UiItemManager>();
        GetComponent<UnityEngine.UI.Toggle>().group = toggleGroup;

        _equipment   = equipmentData;
        _equipmentId = equipmentId;
        _heroId      = heroId;
        _index       = index;
        _weaponSlot  = weaponSlot;

        gameObject.GetComponent<UnityEngine.UI.Toggle>()
                  .onValueChanged.AddListener(onValueChanged => ToggleValueChanged(gameObject.GetComponent<UnityEngine.UI.Toggle>()));

        if (gameObject.GetComponent<UnityEngine.UI.Toggle>().isActiveAndEnabled)
            OnToggleEnabled?.Invoke(_equipment, _index, _equipmentId, _heroId, _weaponSlot);

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        UiItemManager itemManager = gameObject.GetComponent<UiItemManager>();

        if (!Assets.GetEquipment(_equipmentId, out EquipmentSO equipmentSO))
            return;

        Assets.GetEquipmentSet(ItemsIds.EquipmentSetIds[equipmentSO.Set], out EquipmentSetSO equipmentSetSO);

        PlayerManager.Heroes.GetHero(_heroId, out HeroData heroData);
        itemManager.Icon.sprite = equipmentSO.Sprite;

        int upgradeLevel = _equipment.UpgradeLevel;

        _setImage.GetComponent<Image>().sprite = equipmentSetSO.Image;

        if (heroData.GetEquipmentEquiped(_weaponSlot) == null || heroData.GetEquipmentEquiped(_weaponSlot) == "")
        {
            _onHeroText.SetActive(false);
        }
        else
        {
            ushort id    = heroData.GetEquipmentId(_weaponSlot);
            int    index = heroData.GetEquipmentIndex(_weaponSlot);

            if (id == _equipmentId && index == _index)
                _onHeroText.SetActive(true);
            else
                _onHeroText.SetActive(false);
        }

        if (upgradeLevel == 0)
            _upgrade.text = "";
        else
            _upgrade.text = $" {upgradeLevel}";

        // #STODO - set up equipment sprites
        //         _setImage.sprite = 
        gameObject.GetComponent<Image>().color = Colours.RarityColours[_equipment.RarityLevel];
        itemManager.ToggleImage.color          = Colours.RarityColoursDarkened[_equipment.RarityLevel];
    }

    private void ToggleValueChanged(UnityEngine.UI.Toggle change) =>
        UIEquipEquipmentInformation.OnShow?.Invoke(_equipment, _index, _equipmentId, _heroId, _weaponSlot);
}
}