using System;
using System.Linq;
using Shared.Scriptables.Basic;
using Shared.Scriptables.Cards;
using Shared.Structs;
using Shared.Utils.Values;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Shared.Scriptables.Hero
{
[CreateAssetMenu(fileName = "New Hero", menuName = "AnchoredAtSea/Heroes/Hero")]
public class HeroSO : BasicScriptable
{
    [BoxGroup("Hero Info"), HorizontalGroup("Hero Info/Split", 55, LabelWidth = 80), HideLabel, PreviewField(55, ObjectFieldAlignment.Left),
     SerializeField]
    private Sprite _icon;

    [VerticalGroup("Hero Info/Split/Meta"), SerializeField]
    private string _firstName;

    [VerticalGroup("Hero Info/Split/Meta"), SerializeField]
    private string _lastName;

    [VerticalGroup("Hero Info/Split/Meta"), SerializeField]
    private string _nickname;

    [VerticalGroup("Hero Info/Split/Meta"), Range(0, 100), SerializeField]
    private uint _age;

    [VerticalGroup("Hero Info/Split/Meta"), HideLabel, TextArea(3, 12), SerializeField]
    private string _description;

    [HorizontalGroup("Hero Info/Split", 400, LabelWidth = 70), EnumToggleButtons, VerticalGroup("Hero Info/Split/Right"), SerializeField]
    private CharacterTitles _titles;

    [Space, HorizontalGroup("Hero Info/Split", 400, LabelWidth = 70), EnumToggleButtons, VerticalGroup("Hero Info/Split/Right"), SerializeField]
    private CharacterPirateTitles _pirate;

    [Space, HorizontalGroup("Hero Info/Split", 400, LabelWidth = 70), EnumToggleButtons, VerticalGroup("Hero Info/Split/Right"), SerializeField]
    private CharacterRelationshipStatus _relationship;

    [TabGroup("tab1", "Visuals", SdfIconType.ImageAlt, TextColor = "green"), SerializeField]
    private Sprite _small;

    [TabGroup("tab1", "Visuals"), SerializeField]
    private Sprite _wantedPoster;

    [TabGroup("tab1", "Visuals"), SerializeField]
    private Sprite _fullBody;

    // Tutorial thingi
    // [SerializeField] private GestureImages[] _gestures = new GestureImages[Enum.GetValues(typeof(Gestures)).Length];

    [TabGroup("tab1", "Objects", SdfIconType.Octagon, TextColor = "blue"), SerializeField]
    private HeroShipCardSO _shipCard;

    [TabGroup("tab1", "Objects"), SerializeField]
    private GameObject _prefab;

    [TabGroup("tab1", "Stats", SdfIconType.Percent, TextColor = "red"), SerializeField]
    private HeroStats[] _heroStats;

    [TabGroup("tab1", "SkillTree", SdfIconType.Coin, TextColor = "purple"), SerializeField]
    private SkillTreeSO[] _skillTrees;

    public string FirstName
    {
        get => _firstName;
        set => _firstName = value;
    }

    public string LastName => _lastName;
    public string Nickname => _nickname;
    public uint Age => _age;
    public CharacterTitles Titles => _titles;
    public CharacterPirateTitles Pirate => _pirate;
    public CharacterRelationshipStatus CharacterRelationshipStatus => _relationship;
    public string Description => _description;

    public Sprite Icon
    {
        get => _icon;
        set => _icon = value;
    }

    public Sprite Small => _small;
    public Sprite WantedPoster => _wantedPoster;
    public Sprite FullBody => _fullBody;
    public HeroShipCardSO ShipCard => _shipCard;
    public GameObject Prefab => _prefab;

    public HeroStats[] HeroStats => _heroStats;
    public SkillTreeSO[] SkillTrees => _skillTrees;

    // public Sprite GetHeroGestureImage(Gestures type)
    // {
    //     foreach (GestureImages item in _gestures)
    //         if (item.gestures == type)
    //             return item.sprite;
    //
    //     return _gestures[0].sprite;
    // }

    public bool HasAbility(uint id) => _skillTrees.Any(skillTree => skillTree.HasAbility(id));

    public float[] GetSimpleStats(int level)
    {
        float[] returnStats = new float[3];

        for (int i = 0; i < returnStats.Length; i++)
            returnStats[i] = 0;

        if (_heroStats == null)
            return returnStats;

        foreach (HeroStats stat in _heroStats)
        {
            int   index      = 0;
            float multiplier = 0;

            if (GlobalSettings.SimpleAttackMultipliers.ContainsKey(stat.StatType))
            {
                index      = 0;
                multiplier = GlobalSettings.SimpleAttackMultipliers[stat.StatType];
            }
            else if (GlobalSettings.SimpleDefenceMultipliers.ContainsKey(stat.StatType))
            {
                index      = 1;
                multiplier = GlobalSettings.SimpleDefenceMultipliers[stat.StatType];
            }
            else if (GlobalSettings.SimpleSpeedMultipliers.ContainsKey(stat.StatType))
            {
                index      = 2;
                multiplier = GlobalSettings.SimpleSpeedMultipliers[stat.StatType];
            }

            if (stat.IsMultiplier)
                returnStats[index] += CalculateStat(level, stat.BaseValue, stat.IncreaseValue) * multiplier * 10;
            else
                returnStats[index] += CalculateStat(level, stat.BaseValue, stat.IncreaseValue) * multiplier;
        }

        return returnStats;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    static public int CalculateStat(int level, float baseStat, float increaseStat) => Mathf.FloorToInt(baseStat + increaseStat * level);

    public Stat[] GetStatsArray(byte level)
    {
        if (_heroStats == null)
        {
            Debug.LogWarning("Hero has no stats");
            return Array.Empty<Stat>();
        }

        Stat[] stats = new Stat[_heroStats.Length];

        for (int i = 0; i < _heroStats.Length; i++)
        {
            HeroStats stat = _heroStats[i];
            stats[i] = new Stat(stat.StatType, CalculateStat(level, stat.BaseValue, stat.IncreaseValue));
        }

        return stats;
    }
}
}