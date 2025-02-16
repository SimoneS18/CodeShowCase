using Shared.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Shared.Scriptables.Items
{
[CreateAssetMenu(fileName = "Fragments", menuName = "AnchoredAtSea/Items/Fragments")]
public class FragmentsSO : ItemSO
{
    [BoxGroup("Fragment Setup"), EnableIf("@this.UnlockUI"), InfoBox("This is the time added if this fragment level is selected"), SerializeField]
    protected double _craftTime;

    [BoxGroup("Fragment Setup"), EnableIf("@this.UnlockUI"), InfoBox("This is the weighted amount for this rarity"), SerializeField]
    protected uint _value;

    public FragmentsSO() => ItemType = ItemType.Fragment;

    public double CraftTime
    {
        get => _craftTime;
        set => _craftTime = value;
    }

    public uint Value
    {
        get => _value;
        set => _value = value;
    }
}
}