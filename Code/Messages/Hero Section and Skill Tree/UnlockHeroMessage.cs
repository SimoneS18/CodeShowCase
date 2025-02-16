using System;
using Managers;
using Shared.MessageCore;
using Shared.MessageData.Hero;

namespace Messages.Hero
{
public class UnlockHeroMessage : IMessage<UnlockHeroMessageData>
{
    static public   Action OnHeroUnlocked;
    public override string MessageName => MessageNames.UnlockHero.ToString();

    protected override void Handle(ulong senderClientId, UnlockHeroMessageData data)
    {
        PlayerManager.Heroes.AddHero(data.id);

        OnHeroUnlocked?.Invoke();
    }
}
}