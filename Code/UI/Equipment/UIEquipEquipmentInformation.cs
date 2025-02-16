using System;
using Managers;
using Shared.Data.Equipment;
using Shared.Data.Hero;
using Shared.Enums.Equipment;
using Shared.Scriptables.Equipment;
using Shared.Utils.Identifiers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Equipment
{
/// <summary>
///     Following Script is used when a hero equipment Button is pressed and displays the equipments Information
/// </summary>
public class UIEquipEquipmentInformation : MonoBehaviour
{
    static public Action<EquipmentData, int, ushort, ushort, WeaponSlot> OnShow;

    [Header("Appearance"), SerializeField]
    private TMP_Text _equipmentName;

    [SerializeField]
    private Image _equipmentImage;

    [SerializeField]
    private Image _equipmentSetImage;

    [SerializeField]
    private GameObject _equipButton;

    [SerializeField]
    private GameObject _extraText;

    [SerializeField]
    private GameObject _equipingGameObject;

    [Header("Spawning Points"), SerializeField]
    private Transform _StatSpawnPoint;

    [Header("Prefab"), SerializeField]
    private GameObject _statPrefab;

    private EquipmentData _equipment;
    private ushort        _equipmentId;
    private ushort        _heroId;
    private int           _index;
    private WeaponSlot    _weaponSlot;

    private void EquipmentSelected(EquipmentData equ,
                                   int           index,
                                   ushort        equipmentId,
                                   ushort        heroId,
                                   WeaponSlot    weaponSlot)
    {
        _equipment   = equ;
        _equipmentId = equipmentId;
        _index       = index;
        _heroId      = heroId;
        _weaponSlot  = weaponSlot;

        UpdateDisplay();
    }

    private void DisplayEquipmentStats()
    {
        foreach (Transform child in _StatSpawnPoint)
            Destroy(child.gameObject);

        if (ClientSettings.SimpleStats)
        {
            int rarityLevel  = (int)_equipment.RarityLevel;
            int upgradeLevel = _equipment.UpgradeLevel;

            // #SimoneTODO
            //                 float[] current = _equipment.GetSimpleStats(rarityLevel, upgradeLevel);
            // float[] next = _equipment.GetSimpleStats(rarityLevel+1, upgradeLevel + 1);
            /*                float[] max = _equipment.GetSimpleStats(4, 10);*/

            GameObject obj = Instantiate(_statPrefab, _StatSpawnPoint);

            //                 Debug.Log($"Current[0]: {current[0]}");
            //                 Debug.Log($"Max[0]: {max[0]}");
            //                 obj.GetComponent<UIStatsUpgrade>().InitSimple("Attack", Assets.GetStatIconImage(StatType.Damage), current[0], next[0], max[0]);
            // 
            //                 obj = Instantiate(_statPrefab, _StatSpawnPoint);
            //                 obj.GetComponent<UIStatsUpgrade>().InitSimple("Defence", Assets.GetStatIconImage(StatType.Defence), current[1], next[1], max[1]);
            //                 //    obj = Instantiate(_statPrefab, _StatSpawnPoint);
            //                 obj.GetComponent<UIStatsUpgrade>().InitSimple("Speed", Assets.GetStatIconImage(StatType.MovementSpeed), current[2], next[2], max[2]);
        }

        // #SimoneTODO
        //                 foreach (EquipmentStatsData statData in _equipment.Stats.Values.OrderBy(x => x.StatType.ToString()))
        //                 {
        //                     GameObject obj = Instantiate(_statPrefab, _StatSpawnPoint);
        //                     obj.GetComponent<UIStatsCurrent>().InitEquipmentData(_equipment, statData);
        //                 }
    }

    private void UpdateDisplay()
    {
        Debug.Log("AAAAAAAAA'");

        if (!Assets.GetEquipment(_equipmentId, out EquipmentSO equipmentSO))
        {
            Debug.Log("Can't Find EquipmentSO");
            return;
        }

        Assets.GetEquipmentSet(ItemsIds.EquipmentSetIds[equipmentSO.Set], out EquipmentSetSO equipmentSetSO);
        PlayerManager.Heroes.GetHero(_heroId, out HeroData heroData);

        DisplayEquipmentStats();

        if (heroData.GetEquipmentEquiped(_weaponSlot) == null || heroData.GetEquipmentEquiped(_weaponSlot) == "")
        {
            _equipButton.GetComponent<Button>().interactable = true;
            _extraText.SetActive(false);
        }
        else
        {
            ushort id    = heroData.GetEquipmentId(_weaponSlot);
            int    index = heroData.GetEquipmentIndex(_weaponSlot);

            if (id == _equipmentId && index == _index)
            {
                _equipButton.GetComponent<Button>().interactable = false;
                _extraText.SetActive(true);
            }
            else
            {
                _equipButton.GetComponent<Button>().interactable = true;
                _extraText.SetActive(false);
            }
        }

        _equipmentSetImage.GetComponent<Image>().sprite = equipmentSetSO.Image;
        _equipmentName.text                             = equipmentSO.Name;
        _equipmentImage.sprite                          = equipmentSO.Sprite;
    }

    private void OnEquipPressed() => Debug.Log("Equip Button Pressed TODO SIMONE");

    // ClientNetworkUtils.SendServerMessage(MessageNames.EquipEquipment,
    // new EquipEquipmentMessageData(_heroId, _equipmentId, _index, _weaponSlot));
    // _equipingGameObject.SetActive(false);

    #region Awake, OnEnable, OnDisable
    private void Awake() =>

        // OnShow += EquipmentSelected;
        _equipButton.GetComponent<Button>().onClick.AddListener(() => OnEquipPressed());

    private void OnEnable()
    {
        UiEquipmentEquipButton.OnToggleEnabled += EquipmentSelected;
        OnShow                                 += EquipmentSelected;
    }

    private void OnDisable()
    {
        UiEquipmentEquipButton.OnToggleEnabled -= EquipmentSelected;
        OnShow                                 -= EquipmentSelected;
    }
    #endregion
}
}