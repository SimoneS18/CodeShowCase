using Shared.Enums;
using Shared.Scriptables.Basic;
using Shared.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Shared.Scriptables.Items
{
[CreateAssetMenu(fileName = "Item", menuName = "AnchoredAtSea/Items/Item")]
public class ItemSO : BasicScriptable
{
    protected const string ITEM             = "Item";
    protected const string ITEM_SPLIT       = ITEM + "/Split";
    protected const string ITEM_SPLIT_LEFT  = ITEM_SPLIT + "/Left";
    protected const string ITEM_SPLIT_RIGHT = ITEM_SPLIT + "/Right";

    [BoxGroup(ITEM), HorizontalGroup(ITEM_SPLIT, 55f, LabelWidth = 75), HideLabel, EnableIf("@this.UnlockUI"), PreviewField(55, ObjectFieldAlignment.Left),
     SerializeField]
    protected Sprite _sprite;

    [VerticalGroup(ITEM_SPLIT_LEFT), SerializeField, EnableIf("@this.UnlockUI")]
    protected string _name;

    [VerticalGroup(ITEM_SPLIT_LEFT), EnumPaging, GUIColor("GetButtonColor"), SerializeField, EnableIf("@this.UnlockUI")]
    private Rarity _rarity;

    [VerticalGroup(ITEM_SPLIT_LEFT), EnumPaging, ReadOnly, SerializeField, EnableIf("@this.UnlockUI")]
    protected ItemType _itemType;

    [VerticalGroup(ITEM_SPLIT_RIGHT), HideLabel, TextArea, SerializeField, EnableIf("@this.UnlockUI")]
    protected string _description;

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public string Description
    {
        get => _description;
        set => _description = value;
    }

    public Sprite Sprite
    {
        get => _sprite;
        set => _sprite = value;
    }

    public Rarity Rarity
    {
        get => _rarity;
        set => _rarity = value;
    }

    public ItemType ItemType
    {
        get => _itemType;
        set => _itemType = value;
    }

    // Sets UI color depending on rarity selected
    private Color GetButtonColor() => Colours.RarityColours[Rarity];
}
}