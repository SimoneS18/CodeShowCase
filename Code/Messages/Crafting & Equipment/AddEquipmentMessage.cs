using Shared.MessageCore;
using Shared.MessageData.Equipment;

namespace Messages.Equipment
{
public class AddEquipmentMessage : IMessage<AddEquipmentMessageData>
{
    /*        public static Action OnEquipmentAdded;*/

    public override string MessageName => MessageNames.AddEquipment.ToString();

    protected override void Handle(ulong senderClientId, AddEquipmentMessageData data)
    {
        //             if (!PlayerManager.Equipment.Blueprints.GetEquipmentBlueprints(data.id, out BlueprintData blueprintData))
        //             {
        //                 Debug.Log("No Blueprint data found");
        //                 return;
        //             }
        //             if (!blueprintData.HasRarity(data.rarityIndex))
        //             {
        //                 Debug.Log("No Blueprint rarity found");
        //                 return;
        //             }
        //             if (!Assets.GetEquipment(data.id, out EquipmentSO equipmentSO)) return;
        // 
        //             EquipmentRarityDetails RarityDetails = equipmentSO.RarityDetails[data.rarityIndex];
        //             ushort _equipmentId = data.id;
        //             Rarity rarity = equipmentSO.RarityDetails[data.rarityIndex].Rarity;
        // 
        //             EquipmentData equipmentData = new EquipmentData(rarity, 0, "");
        //             EquipmentData equipData = new EquipmentData(rarity, 0, null);
        // 
        //             PlayerManager.Equipment.Equipments.AddEquipment(data.id, equipmentData);
        // 
        //             OnEquipmentAdded?.Invoke();
    }
}
}