using System;
using Managers;
using Shared.MessageCore;
using Shared.MessageData.Items;

namespace Messages.Items
{
public class ItemsRemovedMessage : IMessage<ItemsRemovedMessageData>
{
    static public Action OnItemRemoved;

    public override string MessageName => MessageNames.ItemsRemoved.ToString();

    protected override void Handle(ulong senderClientId, ItemsRemovedMessageData data)
    {
        PlayerManager.Items.RemoveItems(data.id, data.amount);
        OnItemRemoved?.Invoke();
    }
}
}