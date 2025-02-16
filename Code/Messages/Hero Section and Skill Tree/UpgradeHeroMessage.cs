using System;
using Managers;
using Shared.Data.Hero;
using Shared.MessageCore;
using Shared.MessageData.Hero;
using Shared.Scriptables.Hero;
using Shared.Utils.Values;
using Utils;

namespace Messages.Hero
{
public class UpgradeHeroMessage : IMessage<UpgradeHeroMessageData>
{
    static public   Action<int> OnHeroUpgraded;
    public override string      MessageName => MessageNames.UpgradeHero.ToString();

    protected override void Handle(ulong senderClientId, UpgradeHeroMessageData data)
    {
        if (!Assets.GetHero(data.heroId, out HeroSO _))
            return;

        if (!PlayerManager.Heroes.GetHero(data.heroId, out HeroData heroData))
            return;

        heroData.AddLevel();

        // heroData.AddSkillPoints(hero.Levels[heroData.Level - 1].SkillPointReceive);
        heroData.AddSkillPoints(GlobalSettings.HeroSkillPointReceived[heroData.Level - 1]);

        OnHeroUpgraded?.Invoke(heroData.Level);
    }
}
}