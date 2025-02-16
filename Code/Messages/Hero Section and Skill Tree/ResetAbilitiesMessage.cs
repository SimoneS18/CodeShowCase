using System;
using Managers;
using Shared.Data.Hero;
using Shared.MessageCore;
using Shared.MessageData.Hero;
using Shared.Scriptables.Hero;
using UI.Hero;
using UnityEngine;
using Utils;

namespace Messages.Hero
{
public class ResetAbilitiesMessage : IMessage<ResetAbilitiesMessageData>
{
    static public   Action OnResetPressed;
    public override string MessageName => MessageNames.ResetAbility.ToString();

    protected override void Handle(ulong senderClientId, ResetAbilitiesMessageData data)
    {
        ushort _heroId  = data.heroId;
        int    addingSP = data.adding;

        if (!PlayerManager.Heroes.GetHero(_heroId, out HeroData heroData))
            return;

        if (!Assets.GetHeroSkillTree(heroData.SelectedST, out SkillTreeSO skillTreeSO))
            return;

        for (int i = 0; i < skillTreeSO.Abilities.Length; i++)
        {
            Assets.GetHeroAbility(skillTreeSO.Abilities[i].Id, out HeroAbilitySO heroAbilitySO);

            if (heroData.AbilityData.GetAbility(heroAbilitySO.Id, out AbilityData abilityData))
                abilityData.ResetLevel();
        }

        heroData.AddSkillPoints(addingSP);
        heroData.AddResetCount();
        heroData.ResetSelectedST();

        //OnResetPressed?.Invoke();
        HeroSelectionNavigation.OnSkillTreeReset?.Invoke(_heroId);

        Debug.Log("ST Reset");
    }
}
}