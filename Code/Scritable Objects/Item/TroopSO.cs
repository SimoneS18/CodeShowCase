using Shared.Enums.Units;
using Shared.Scriptables.Basic;
using UnityEngine;

namespace Shared.Scriptables.Items
{
[CreateAssetMenu(fileName = "New Troop", menuName = "AnchoredAtSea/Troops/Troop")]
public class TroopSO : BasicScriptable
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private string _description;

    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    private TroopType _troopType;

    [SerializeField]
    private TroopLevel[] _levels;

    public string Name => _name;
    public string Description => _description;
    public Sprite Sprite => _sprite;
    public TroopType TroopType => _troopType;
    public TroopLevel[] Levels => _levels;
}
}