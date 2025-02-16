using System;
using Managers;
using Shared.Data.Equipment;
using Shared.MessageCore;
using Shared.MessageData.Equipment;
using Shared.Scriptables.Equipment;
using Utils;

namespace Messages.Equipment
{
public class UpgradeEquipmentMessage : IMessage<UpgradeEquipmentMessageData>
{
    static public Action<ushort, int, EquipmentData> OnEquipmentUpgraded;
    static public Action                             EquipmentUpgraded;

    public override string MessageName => MessageNames.UpgradeEquipment.ToString();

    protected override void Handle(ulong senderClientId, UpgradeEquipmentMessageData data)
    {
        ushort equipmentId = data.buildingId;
        int    index       = data.buildingIndex;

        if (!Assets.GetEquipment(equipmentId, out EquipmentSO equipmentSO))
            return;

        PlayerManager.Equipment.Equipments.GetEquipment(equipmentId, out Shared.Data.Equipment.Equipment equipment);
        EquipmentData equipmentData = equipment.Items[index];

        //#SIMONETODO
        // Remove materials
        //             foreach (MaterialCost material in equipmentSO.BaseCost)
        //             {
        //                 uint amount = GlobalSettings.EquipmentMaterialCost(equipmentData.RarityLevel,
        //                     equipmentData.UpgradeLevel,
        //                     material.Amount);
        //                 PlayerManager.Equipment.Materials.RemoveMaterials(material.Id, amount);
        //             }
        // 
        //             PlayerManager.Equipment.Equipments.UpgradeEquipment(equipmentId, buildingIndex);
        // 
        //             OnEquipmentUpgraded?.Invoke(equipmentId, buildingIndex, equipmentData);
        //             EquipmentUpgraded?.Invoke();
        // 
        //             Debug.Log("Equipment Leveled Up");
    }
}
}