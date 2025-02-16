using System;
using Shared.Enums.Equipment;
using UI.Admin___Testing;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Equipment
{
public class EquipmentIconUI : MonoBehaviour
{
    static public Action<WeaponSlot> OnShow;

    [Header("Apperence"), SerializeField]
    private WeaponSlot _slot;

    [SerializeField]
    private Image _icon;

    private void Awake()
    {
        _icon.sprite = Assets.GetEquipmentStandardImages(_slot);

        // If Active at start, set starting rarity
        if (gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn)
            DummyCrafting.OnButtonPressed?.Invoke(_slot);

        gameObject.GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener(isOn => ToggleValueChanged(isOn));

        ////Add listener for when the state of the Toggle changes, to take action
        //gameObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate
        //{
        //    ToggleValueChanged(gameObject.GetComponent<Toggle>());
        //});
    }

    private void ToggleValueChanged(bool isOn)
    {
        if (isOn)
            DummyCrafting.OnButtonPressed?.Invoke(_slot);
    }
}
}