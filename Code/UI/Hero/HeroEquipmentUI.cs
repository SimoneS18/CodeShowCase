using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Messages.Equipment;
using Messages.Hero;
using Shared.Data.Equipment;
using Shared.Data.Hero;
using Shared.Enums;
using Shared.Enums.Assets;
using Shared.Enums.Equipment;
using Shared.MessageData.BaseBuilding.EquipmentCrafting;
using Shared.Scriptables.Equipment;
using Shared.Scriptables.Hero;
using Shared.Utils;
using Shared.Utils.Identifiers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace UI.Hero
{
/// <summary>
///     on the button on hero panel that show the player if there is equipment equipped
/// </summary>
public class HeroEquipmentUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _upgrade;

    [SerializeField]
    private GameObject _setImage;

    [SerializeField]
    private GameObject _secondIcon;

    [SerializeField]
    private GameObject _thirdIcon;

    [SerializeField]
    private GameObject _noEquipmentText;

    [FormerlySerializedAs("_equipmentSlot"), Header("Equipment Slot"), SerializeField]
    private WeaponSlot weaponSlot;

    private List<EquipmentSO> _equipmentSO = new List<EquipmentSO>();

    private HeroData _heroData;
    private ushort   _heroId;

    private int _heroLevel;
    private int _levelToUnlock;

    internal void Init(ushort heroId)
    {
        _heroId = heroId;

        if (!PlayerManager.Heroes.GetHero(_heroId, out HeroData heroData))
            return;

        _heroData = heroData;

        Upgrade();
    }

    internal void Upgrade()
    {
        UiItemManager itemManager = gameObject.GetComponent<UiItemManager>();

        #region Check if we have any equipment to equip - put the SO's in list if we do
        _equipmentSO = Assets.GetEquipment().Where(x => x.WeaponSlot == weaponSlot).ToList();

        List<EquipmentSO> removing = new List<EquipmentSO>();

        for (int i = 0; i < _equipmentSO.Count; i++)
        {
            if (!PlayerManager.Equipment.Equipments.GetEquipment(_equipmentSO[i].Id, out Shared.Data.Equipment.Equipment _))
                removing.Add(_equipmentSO[i]);
        }

        // remove SO's that you do not have in DB
        if (removing.Count > 0)
            for (int i = 0; i < removing.Count; i++)
                _equipmentSO.Remove(removing[i]);
        #endregion

        Assets.GetHero(_heroId, out HeroSO _);
        _heroLevel = _heroData.Level;

        // _levelToUnlock = Shared.Utils.Values.GlobalSettings.HeroLevelUnlock[weaponSlot];
        _levelToUnlock = 0;

        // #SIMONETODO

        itemManager.Icon.sprite = Assets.GetEquipmentStandardImages(weaponSlot);

        #region Is not locked
        if (_heroLevel >= _levelToUnlock)
        {
            #region Have Equipment
            if (_equipmentSO.Count != 0)
            {
                // there is equipment to equip
                itemManager.Icon.gameObject.SetActive(false);
                _setImage.SetActive(true);
                _noEquipmentText.SetActive(true);

                gameObject.GetComponent<Button>().interactable = true;
                gameObject.GetComponent<Button>().onClick.AddListener(() => OpenEquipingPanel());

                // determine if it has equipment already equipped
                // has no equipment equipped for that slot
                if (_heroData.GetEquipmentEquiped(weaponSlot) == null ||
                    _heroData.GetEquipmentEquiped(weaponSlot) == "")
                {
                    _secondIcon.SetActive(false);
                    _upgrade.gameObject.SetActive(false);
                    _setImage.SetActive(false);

                    gameObject.GetComponent<Image>().color = Colours.White32;
                    _thirdIcon.SetActive(true);
                    _thirdIcon.GetComponent<Image>().sprite = Assets.GetEquipmentStandardImages(weaponSlot);

                    // todo - check if can equip these equipment
                    List<string> thingi       = new List<string>();
                    string       heroIdString = Convert.ToString((int)_heroId);

                    for (int i = 0; i < _equipmentSO.Count; i++)
                    {
                        PlayerManager.Equipment.Equipments.GetEquipment(_equipmentSO[i].Id,
                                                                        out Shared.Data.Equipment.Equipment equipmentData);

                        for (int j = 0; j < equipmentData.Items.Count; j++)

                            // check to make sure it inst equipped to a hero already
                        {
                            if (equipmentData.Items[j].Equiped == null || equipmentData.Items[j].Equiped == "" ||
                                equipmentData.Items[j].Equiped == heroIdString)
                                thingi.Add(equipmentData.Items[j].Equiped);
                        }
                    }

                    // if count is 0, then deactivate button - else player can press button
                    if (thingi.Count == 0)
                    {
                        _noEquipmentText.SetActive(true);
                        gameObject.GetComponent<Button>().interactable = false;
                    }
                    else if (thingi.Count > 0)
                    {
                        _noEquipmentText.SetActive(false);
                        gameObject.GetComponent<Button>().interactable = true;
                    }
                }

                // does have equipment
                else
                {
                    ushort id    = ushort.MaxValue;
                    int    index = 0;

                    if (_heroData.GetEquipmentEquiped(weaponSlot) != "")
                    {
                        id    = _heroData.GetEquipmentId(weaponSlot);
                        index = _heroData.GetEquipmentIndex(weaponSlot);
                    }

                    PlayerManager.Equipment.Equipments.GetEquipment(id, out Shared.Data.Equipment.Equipment EB);

                    if (!Assets.GetEquipment(id, out EquipmentSO equipmentSO))
                        return;

                    Assets.GetEquipmentSet(ItemsIds.EquipmentSetIds[equipmentSO.Set],
                                           out EquipmentSetSO equipmentSetSO);

                    EquipmentData EquipmentData = EB.Items[index];

                    Rarity rarity       = EquipmentData.RarityLevel;
                    int    upgradeLevel = EquipmentData.UpgradeLevel;

                    // now edit the buttons appearance (text and images)
                    gameObject.GetComponent<Image>().color = Colours.RarityColours[rarity];
                    _noEquipmentText.SetActive(false);
                    itemManager.Icon.gameObject.SetActive(false);
                    _setImage.SetActive(true);
                    _setImage.GetComponent<Image>().sprite = equipmentSetSO.Image;
                    _thirdIcon.SetActive(true);
                    _thirdIcon.GetComponent<Image>().sprite = equipmentSO.Sprite;
                    _secondIcon.SetActive(false);

                    // make sure Upgrade Level 
                    if (upgradeLevel != 0)
                    {
                        _upgrade.gameObject.SetActive(true);
                        _upgrade.gameObject.GetComponent<TMP_Text>().text = $"+{upgradeLevel}";
                    }
                    else
                    {
                        _upgrade.gameObject.SetActive(false);
                    }
                }
            }
            #endregion

            #region No equipment
            else
            {
                // no Equipment
                _upgrade.gameObject.SetActive(false);
                _setImage.SetActive(false);
                _setImage.GetComponent<Image>().sprite = Assets.GetIconImage(AdditionalIcon.Unknown);

                _noEquipmentText.SetActive(true);

                _secondIcon.SetActive(false);
                _thirdIcon.SetActive(true);
                _thirdIcon.GetComponent<Image>().sprite        = Assets.GetEquipmentStandardImages(weaponSlot);
                gameObject.GetComponent<Button>().interactable = false;
            }
            #endregion
        }
        #endregion

        #region Is Locked
        // is locked
        else if (_heroLevel < _levelToUnlock)
        {
            _noEquipmentText.SetActive(false);
            itemManager.Icon.gameObject.SetActive(false);
            itemManager.Icon.gameObject.GetComponent<Image>().color = Colours.White32;
            _thirdIcon.SetActive(false);
            _secondIcon.SetActive(true);
            gameObject.GetComponent<Button>().interactable = false;

            _upgrade.gameObject.SetActive(false);
            _setImage.SetActive(false);
            _setImage.GetComponent<Image>().sprite = Assets.GetIconImage(AdditionalIcon.Unknown);
        }
        #endregion
    }

    private void OpenEquipingPanel() => HeroPopupNavigation.OnShowEquipmentToEquipUI?.Invoke(_heroId, weaponSlot);

    #region Unity
    private void OnEnable()
    {
        UpgradeHeroMessage.OnHeroUpgraded                  += UpgradeHeroMessage_OnHeroUpgraded;
        EquipmentCraftCompleteMessage.OnEquipmentCollected += Upgrade;
        EquipEquipmentMessage.OnEquipmentEquiped           += Upgrade;
    }

    private void OnDisable()
    {
        UpgradeHeroMessage.OnHeroUpgraded                  -= UpgradeHeroMessage_OnHeroUpgraded;
        EquipmentCraftCompleteMessage.OnEquipmentCollected -= Upgrade;
        EquipEquipmentMessage.OnEquipmentEquiped           -= Upgrade;
    }

    private void UpgradeHeroMessage_OnHeroUpgraded(int level) => Upgrade();
    #endregion
}
}