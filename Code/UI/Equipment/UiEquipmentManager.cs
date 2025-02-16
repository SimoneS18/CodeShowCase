using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Equipment
{
public class UiEquipmentManager : MonoBehaviour
{
    [Header("Appearance"), SerializeField]
    private Image _icon;

    [SerializeField]
    private TMP_Text _upgrade;

    [SerializeField]
    private Image _setImage;

    [SerializeField]
    private Image _toggledImage;

    public Image    Icon        => _icon;
    public TMP_Text Upgrade     => _upgrade;
    public Image    SetImage    => _setImage;
    public Image    ToggleImage => _toggledImage;
}
}