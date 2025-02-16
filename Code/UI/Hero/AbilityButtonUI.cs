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
///     On the ST (the points) and controls its look and if its on
/// </summary>
public class AbilityButtonUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _image;

    private ushort _abilityId;

    private ushort _heroId;
    private ushort _skillTreeId;

    //#STODO - add action to update button 
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

    public void Init(ushort heroId, ushort abilityId, ushort skillTreeId)
    {
        _heroId      = heroId;
        _abilityId   = abilityId;
        _skillTreeId = skillTreeId;

        GetComponent<Button>().onClick.AddListener(() => OnClick());
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        // do this first to see if ability is real
        if (!PlayerManager.Heroes.GetHero(_heroId, out HeroData heroData))
            return;

        if (!Assets.GetHero(_heroId, out HeroSO _))
            return;

        if (!Assets.GetHeroSkillTree(_skillTreeId, out SkillTreeSO skillTreeSO))
            return;

        if (!Assets.GetHeroAbility(_abilityId, out HeroAbilitySO abilitySO))
            return;

        SkillTreeAbilityType type            = abilitySO.AbilityType;
        int                  NeededHeroLevel = GlobalSettings.SkillTreeHeroLevel[type];
        int                  NeededSPSpent   = GlobalSettings.SkillTreeSkillPointsMustSpent[type];

        // Checks - hero Level and Skill points to spend
        // #TODO - maybe change the button if is not on in future
        if (NeededHeroLevel > heroData.Level)
        {
            _image.GetComponent<Image>().color             = Colours.GreyDark32;
            gameObject.GetComponent<Button>().interactable = false;
            return;
        }

        if (NeededSPSpent > GlobalSettings.SkillPointsSpent(heroData, _abilityId))
        {
            _image.GetComponent<Image>().color             = Colours.GreyDark32;
            gameObject.GetComponent<Button>().interactable = false;
            return;
        }

        // change color
        if (!heroData.AbilityData.GetAbility(_abilityId, out AbilityData data))
        {
            gameObject.GetComponent<Button>().interactable = true;
            _image.GetComponent<Image>().color             = Colours.White32;
        }
        else
        {
            if (data.Level > 0)
                _image.GetComponent<Image>().color = skillTreeSO.Colours[data.Level - 1];
            else if (data.Level == 0)
                _image.GetComponent<Image>().color = Colours.White32;

            gameObject.GetComponent<Button>().interactable = true;
        }

        // Button is on
    }

    private void OnClick() => HeroPopupNavigation.OnShowAbilityUI?.Invoke(_heroId, _skillTreeId, _abilityId);
}
}