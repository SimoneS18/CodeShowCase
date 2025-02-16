using System;
using Managers;
using Shared.MessageCore;
using Shared.MessageData.Hero;

namespace Messages.Hero
{
public class SelectHeroMessage : IMessage<SelectHeroMessageData>
{
    static public   Action OnHeroSelected;
    public override string MessageName => MessageNames.SelectHero.ToString();

    protected override void Handle(ulong senderClientId, SelectHeroMessageData data)
    {
        PlayerManager.Heroes.SetSelected(data.id);
        OnHeroSelected?.Invoke();

        // check to see if this tutorial is in the thingi
        if (PlayerManager.TutorialData.HeroGuide == 0)
        {
            PlayerManager.TutorialData.SetGuideHero(data.id);
            PlayerManager.Heroes.AddHero(data.id);
            TutorialMain.OnHeroGuideSelected?.Invoke(data.id);
        }
        else
        {
            TutorialMain.OnHeroGuideSelected?.Invoke(PlayerManager.TutorialData.HeroGuide);
        }
    }
}
}