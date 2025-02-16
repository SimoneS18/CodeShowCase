using Shared.Enums;
using Shared.Enums.Material;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Shared.Scriptables.Items
{
[CreateAssetMenu(fileName = "Materials", menuName = "AnchoredAtSea/Items/Materials")]
public class MaterialSO : ItemSO
{
    [VerticalGroup(ITEM_SPLIT_LEFT), EnumPaging, SerializeField, EnableIf("@this.UnlockUI")]
    protected MaterialTypes _type;

    public MaterialSO() => ItemType = ItemType.Material;

    public MaterialTypes Type
    {
        get => _type;
        set => _type = value;
    }
}
}