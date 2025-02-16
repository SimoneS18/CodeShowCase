using Managers;
using Shared.Data.Hero;
using Shared.MessageCore;
using Shared.MessageData.Hero;
using UI.Hero;
using UnityEngine;

namespace Messages.Hero
{
public class SelectHeroSkillTreeMessage : IMessage<SelectHeroSkillTreeMessageData>
{
    //public static Action OnSkillTreeSelected;
    public override string MessageName => MessageNames.HeroSkillTreeSelected.ToString();

    protected override void Handle(ulong senderClientId, SelectHeroSkillTreeMessageData data)
    {
        ushort _STId   = data.skillTreeId;
        ushort _heroId = data.heroId;

        PlayerManager.Heroes.GetHero(_heroId, out HeroData herodata);

        herodata.AddSelected(_STId);

        Debug.Log($"Added ST id: {_STId} to {_heroId} DB");

        //OnSkillTreeSelected?.Invoke();
        HeroSelectionNavigation.OnShowSTAfterSelection?.Invoke(_heroId, _STId);
    }
}
}