using Shared.Scriptables.Basic;
using Shared.Scriptables.Cards;
using UnityEngine;

namespace Shared.Scriptables.Hero
{
[CreateAssetMenu(fileName = "New Hero Ability", menuName = "AnchoredAtSea/Heroes/Ability")]
public class HeroAbilitySO : BasicScriptable
{
    [Header("Information"), SerializeField]
    private string _name;

    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    private SkillTreeAbilityType _abilityType;

    [TextArea(3, 2), SerializeField]
    private string _description;

    [SerializeField]
    private HeroAbilityLevel[] _levels;

    [Header("Ability Use"), Tooltip("Only used for core abilities"), SerializeField]
    private AbilityCardSO _abilityCard;

    public string Name => _name;
    public Sprite Sprite => _sprite;
    public SkillTreeAbilityType AbilityType => _abilityType;
    public string Description => _description;
    public HeroAbilityLevel[] Levels => _levels;
    public AbilityCardSO AbilityCard => _abilityCard;
}
}