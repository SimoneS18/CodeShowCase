using System;
using Managers;
using Shared.Data.Equipment;
using Shared.Enums;
using Shared.Scriptables.Equipment;
using Shared.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Equipment
{
public class UiEquipmentButton : MonoBehaviour
{
    static public Action<ushort, Rarity> OnTogglePressed;

    [Title("Equipment Button Appearance")]
    [SerializeField, Required] private Image                 _setIcon;
    [SerializeField, Required] private UnityEngine.UI.Toggle _toggle;

    private ushort                _equipmentId;
    private Rarity                _rarity;
    private bool                  _set;

    private void OnEnable()
    {
        if (!_set)
            return;
        
        IfToggleEnabled();
    }

    public void Init(ushort equipmentId, Rarity rarity, ToggleGroup toggleGroup)
    {
        IfToggleEnabled();

        UiItemManager itemManager = gameObject.GetComponent<UiItemManager>();

        _equipmentId  = equipmentId;
        _rarity       = rarity;
        _toggle.group = toggleGroup;

        if (!Assets.GetEquipment(_equipmentId, out EquipmentSO equipmentSo))
            return;
        
        itemManager.Icon.sprite       = equipmentSo.Sprite;
        itemManager.Background.color  = Colours.RarityColours[_rarity];
        itemManager.ToggleImage.color = Colours.RarityColoursDarkened[_rarity];

        // determine if blueprint is in the players DB
        if (PlayerManager.Equipment.Blueprints.GetEquipmentBlueprints(_equipmentId, out BlueprintData blueprintData))
        {
            // if so, check the rarity is saved
            if (blueprintData.HasRarity(_rarity))
                itemManager.Icon.color = Colours.Green32;
            else
                itemManager.Icon.color = Colours.Black;
        }
        else
            itemManager.Icon.color = Colours.Black;

        _set = true;
    }

    private void ToggleValueChanged(UnityEngine.UI.Toggle change)
    {
        UiItemManager itemManager = gameObject.GetComponent<UiItemManager>();

        if (!change.isOn)
        {
            itemManager.ToggleImage.gameObject.SetActive(false);
            return;
        }

        itemManager.ToggleImage.gameObject.SetActive(true);
        OnTogglePressed?.Invoke(_equipmentId, _rarity);
        UiTabToggle.OnShow?.Invoke(_equipmentId);
    }

    private void IfToggleEnabled()
    {
        UiItemManager itemManager = gameObject.GetComponent<UiItemManager>();

        // Add listener for when the state of the Toggle changes, to take action
        _toggle.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged(_toggle);
        });

        // Following Determines if the toggle is activated when enabled, if so, change the panel display on the right
        if (_toggle.isOn)
        {
            Debug.Log("TOggle IS ON");
            itemManager.ToggleImage.gameObject.SetActive(true);
            OnTogglePressed?.Invoke(_equipmentId, _rarity);
        }
        else
        {
            Debug.Log("TOggle IS OFF");
            itemManager.ToggleImage.gameObject.SetActive(false);
            
        }
    }
}
}