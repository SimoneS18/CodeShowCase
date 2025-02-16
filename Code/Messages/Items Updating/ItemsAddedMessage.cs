using System;
using Managers;
using Shared.MessageCore;
using Shared.MessageData.Items;

namespace Messages.Items
{
public class ItemsAddedMessage : IMessage<ItemsAddedMessageData>
{
    static public Action OnItemAdded;

    public override string MessageName => MessageNames.ItemsAdded.ToString();

    protected override void Handle(ulong senderClientId, ItemsAddedMessageData data)
    {
        PlayerManager.Items.AddItem(data.id, data.amount);
        OnItemAdded?.Invoke();
    }
}
}