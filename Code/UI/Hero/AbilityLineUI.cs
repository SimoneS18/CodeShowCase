using Managers;
using Messages.Hero;
using Shared.Data.Hero;
using Shared.Scriptables.Hero;
using Shared.Utils;
using Shared.Utils.Values;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Hero
{
/// <summary>
///     On the ST Lines and controls its look
/// </summary>
public class AbilityLineUI : MonoBehaviour
{
    private HeroAbilitySO _abilityOne;
    private HeroAbilitySO _abilityTwo;
    private ushort        _heroId;
    private int           _one;
    private ushort        _skId;
    private int           _two;

    private void OnEnable()
    {
        LevelUpAbilityMessage.OnHeroAbilityLeveled += UpgradeHeroMessage_OnHeroUpgraded;
        ResetAbilitiesMessage.OnResetPressed       += ResetAbilitiesMessage_OnResetPressed;
    }

    private void OnDisable()
    {
        LevelUpAbilityMessage.OnHeroAbilityLeveled -= UpgradeHeroMessage_OnHeroUpgraded;
        ResetAbilitiesMessage.OnResetPressed       -= ResetAbilitiesMessage_OnResetPressed;
    }

    private void UpgradeHeroMessage_OnHeroUpgraded(int level) => UpdateDisplay();

    private void ResetAbilitiesMessage_OnResetPressed() => UpdateDisplay();

    public void Init(ushort heroId, ushort stId, int one, int two)
    {
        _heroId = heroId;
        _skId   = stId;
        _one    = one;
        _two    = two;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (!PlayerManager.Heroes.GetHero(_heroId, out HeroData heroData))
            return;

        if (!Assets.GetHero(_heroId, out HeroSO _))
            return;

        if (!Assets.GetHeroSkillTree(_skId, out SkillTreeSO skillTreeSO))
            return;

        _abilityOne = skillTreeSO.Abilities[_one];
        _abilityTwo = skillTreeSO.Abilities[_two];

        Material mat = gameObject.GetComponent<Image>().material;

        // AbilityOne
        SkillTreeAbilityType OneType            = _abilityOne.AbilityType;
        int                  OneNeededHeroLevel = GlobalSettings.SkillTreeHeroLevel[OneType];
        int                  OneNeededSPSpent   = GlobalSettings.SkillTreeSkillPointsMustSpent[OneType];

        if (OneNeededHeroLevel > heroData.Level || OneNeededSPSpent >
            GlobalSettings.SkillPointsSpent(heroData, _abilityOne.Id))
        {
            mat.SetColor("Colour_1", Colours.GreyDark32);
        }
        else
        {
            if (!heroData.AbilityData.GetAbility(_abilityOne.Id, out AbilityData data))
            {
                mat.SetColor("Colour_1", Colours.White32);
            }
            else
            {
                if (data.Level > 0)
                    mat.SetColor("Colour_1", skillTreeSO.Colours[data.Level - 1]);
                else
                    mat.SetColor("Colour_1", Colours.White32);
            }
        }

        // AbilityTwo - ingame
        SkillTreeAbilityType TwoType            = _abilityTwo.AbilityType;
        int                  TwoNeededHeroLevel = GlobalSettings.SkillTreeHeroLevel[TwoType];
        int                  TwoNeededSPSpent   = GlobalSettings.SkillTreeSkillPointsMustSpent[TwoType];

        if (TwoNeededHeroLevel > heroData.Level || TwoNeededSPSpent >
            GlobalSettings.SkillPointsSpent(heroData, _abilityTwo.Id))
        {
            mat.SetColor("Colour_2", Colours.GreyDark32);
        }
        else
        {
            if (!heroData.AbilityData.GetAbility(_abilityTwo.Id, out AbilityData data))
            {
                mat.SetColor("Colour_2", Colours.White32);
            }
            else
            {
                if (data.Level > 0)
                    mat.SetColor("Colour_2", skillTreeSO.Colours[data.Level - 1]);
                else
                    mat.SetColor("Colour_2", Colours.White32);
            }
        }
    }
}
}