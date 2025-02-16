using Shared.Enums;
using Shared.Scriptables.Equipment;
using Shared.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Equipment
{
/// <summary>
///     Following script is used in the Equipment Popup to display what equipment BP is unlocked
/// </summary>
public class BlueprintUnlockedUI : MonoBehaviour
{
    [Header("Apperence"), SerializeField]
    private GameObject _icon;

    [SerializeField]
    private GameObject _blur;

    [SerializeField]
    private TMP_Text _name;

    public void Init(ushort equipmentId, Rarity rarity)
    {
        if (!Assets.GetEquipment(equipmentId, out EquipmentSO equipmentSo))
            return;

        _icon.GetComponent<Image>().sprite = equipmentSo.Sprite;
        _blur.GetComponent<Image>().color  = Colours.RarityColours[rarity];

        _name.color = Colours.RarityColours[rarity];
        _name.text  = $"{equipmentSo.Name} Blueprint";
    }
}
}