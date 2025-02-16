using System;
using Shared.MessageCore;
using Shared.MessageData.Equipment;

namespace Messages.Equipment
{
public class AddBlueprintMessage : IMessage<AddBlueprintMessageData>
{
    static public Action OnBlueprintAdd;

    public override string MessageName => MessageNames.AddBlueprint.ToString();

    protected override void Handle(ulong senderClientId, AddBlueprintMessageData data) =>

        //             ushort _equipmentId = data.equipmentId;
        //             Rarity _rarity = data.rarity;
        // 
        //             if (!Assets.GetEquipment(_equipmentId, out EquipmentSO _))
        //             {
        //                 Debug.Log("Cant find Equipment SO");
        //                 return;
        //             }
        /*            PlayerManager.Equipment.Blueprints.AddEquipmentBlueprint(_equipmentId, _rarity);*/
        OnBlueprintAdd?.Invoke();
}
}