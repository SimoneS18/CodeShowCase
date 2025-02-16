using System;
using Managers;
using Shared.Data.Hero;
using Shared.MessageCore;
using Shared.MessageData.Hero;
using Shared.Scriptables.Hero;
using Shared.Utils.Values;
using Utils;


//#STODO
namespace Messages.Hero
{
public class LevelUpAbilityMessage : IMessage<LevelUpAbilityMessageData>
{
    static public   Action<int> OnHeroAbilityLeveled;
    public override string      MessageName => MessageNames.LevelAbility.ToString();

    protected override void Handle(ulong senderClientId, LevelUpAbilityMessageData data)
    {
        // Check that hero exists and player owns it
        if (!Assets.GetHero(data.heroId, out HeroSO heroSo))
            return;

        if (!PlayerManager.Heroes.GetHero(data.heroId, out HeroData heroData))
            return;

        // Check that ability exists and it belongs to this hero
        if (!Assets.GetHeroAbility(data.abilityId, out HeroAbilitySO abilitySo))
            return;

        if (heroSo.HasAbility(data.abilityId))
            return;

        // Get current level
        int level = 0;

        if (heroData.AbilityData.GetAbility(data.abilityId, out AbilityData abilityData))
            level = abilityData.Level - 1;

        // Get skill point cost
        int skillPointCost = abilitySo.AbilityType switch
        {
            SkillTreeAbilityType.Core       => GlobalSettings.SkillPointsCostCore[level],
            SkillTreeAbilityType.LevelOne   => GlobalSettings.SkillPointsCostOne[level],
            SkillTreeAbilityType.LevelTwo   => GlobalSettings.SkillPointsCostTwo[level],
            SkillTreeAbilityType.LevelThree => GlobalSettings.SkillPointsCostThree[level],
            SkillTreeAbilityType.LevelFour  => GlobalSettings.SkillPointsCostFour[level],
            _                               => GlobalSettings.SkillPointsCostShared[level]
        };

        //Add level
        if (level == 0)
            heroData.AbilityData.AddAbility(data.abilityId);
        else
            abilityData.AddLevel();

        //Remove skill points
        heroData.SubSkillPoints(skillPointCost);

        //Tell things this is done
        OnHeroAbilityLeveled?.Invoke(heroData.AbilityData.Abilities[data.abilityId].Level);
        ByteBrewManager.SkilltreeSkillUnlocked(abilitySo.Name);
    }
}
}