using System;
using Shared.Scriptables.Basic;
using UnityEngine;

namespace Shared.Scriptables.Hero
{
[CreateAssetMenu(fileName = "New Skill Tree", menuName = "AnchoredAtSea/Heroes/SkillTree")]
public class SkillTreeSO : BasicScriptable
{
    [Header("Appearance"), SerializeField]
    private string _name;

    [TextArea(3, 2), SerializeField]
    private string _description;

    [SerializeField]
    private Sprite _image;

    [SerializeField]
    private SkillTreeType _type;

    [SerializeField]
    private HeroAbilitySO[] _abilities;
    /*    [SerializeField] private SkillLevelUnlock[] _levels;*/

    [Header("Colours"), SerializeField]
    private Color32[] _colors;

    [Header("Prefabs"), SerializeField]
    private GameObject _SkillTreePrefab;

    #region Public's
    public string Name => _name;
    public string Description => _description;
    public Sprite Image => _image;
    public SkillTreeType Type => _type;
    public HeroAbilitySO[] Abilities => _abilities;
    public Color32[] Colours => _colors;

    public GameObject SkillTreePrefab => _SkillTreePrefab;
    /*    public SkillLevelUnlock[] Levels => _levels;*/

    public bool HasAbility(uint id) => Array.Exists(_abilities, x => x.Id == id);
    #endregion
}

//#STODO - check if we need this with chase
// [Serializable]
// public struct SkillLevelUnlock
// {
//     [SerializeField] private STAbilityType _type;
//     [SerializeField] private byte _heroLevelRequired;
//     [SerializeField] private int _totalSpSpent;
//     [SerializeField] private List<AchievementRequirement> _achievementsRequired;
// 
//     public STAbilityType Type => _type;
//     public byte HeroLevelRequired => _heroLevelRequired;
//     public int TotalSpSpent => _totalSpSpent;
// 
//     public bool Achieved(PlayerData data)
//     {
//         if (!data.Achievements.HasHitAchievements(_achievementsRequired)) return false;
//         return true;
//     }
// }
}