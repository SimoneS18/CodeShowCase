using System;
using Managers;
using Shared.Data;
using Shared.Data.Equipment;
using Shared.Data.Hero;
using Shared.Enums.Equipment;
using Shared.MessageCore;
using Shared.MessageData.Equipment;
using Shared.Scriptables.Equipment;
using Shared.Scriptables.Hero;
using Utils;

namespace Messages.Equipment
{
public class EquipEquipmentMessage : IMessage<EquipEquipmentMessageData>
{
    static public Action OnEquipmentEquiped;

    public override string MessageName => MessageNames.EquipEquipment.ToString();

    // #SimoneTODO 21/05/2023 Needs redoing
    protected override void Handle(ulong senderClientId, EquipEquipmentMessageData data)
    {
        ushort        heroId        = data.heroId;
        ushort        equipmentId   = data.equipmentId;
        int           index         = data.index;
        EquipmentType equipmentSlot = data.equipmentType;
        int           slotEnum      = data.slotEnum;

        if (!Assets.GetEquipment(equipmentId, out EquipmentSO _))
            return;

        if (!Assets.GetHero(heroId, out HeroSO _))
            return;

        if (!PlayerManager.Heroes.GetHero(heroId, out HeroData heroData))
            return;

        if (!PlayerManager.Equipment.Equipments.GetEquipment(equipmentId, out Shared.Data.Equipment.Equipment EB))
            return;

        EquipmentData equipment = EB.Items[index];
        string        hero      = Convert.ToString((int)heroId);

        // if (heroData.GetEquipmentEquiped(weaponSlot) != null && heroData.GetEquipmentEquiped(weaponSlot) != "")
        // {
        // un-equip the equipment already on
        // PlayerManager.Equipment.Equipments.GetEquipment(heroData.GetEquipmentId(weaponSlot),
        // out Shared.Data.Equipment.Equipment eqquipedEquipment);
        // eqquipedEquipment.Items[heroData.GetEquipmentIndex(weaponSlot)].EquipmentUnequiped();
        // }

        // now equip new hero Equipment
        // AddHero(heroId, equipmentId, index, weaponSlot, PlayerManager.Data, equipment, hero);
    }

    static private void AddHero(ushort        heroId,
                                ushort        equipmentId,
                                int           index,
                                WeaponSlot    weaponSlot,
                                PlayerData    playerData,
                                EquipmentData equipment,
                                string        hero)
    {
        string adding = $"{equipmentId} {index}";
        playerData.Heroes.AddEquipment(heroId, weaponSlot, adding);

        if (Assets.GetEquipment(equipmentId, out EquipmentSO equipmentSO))
            ByteBrewManager.HeroEquipmentEquipped(equipmentSO.Name);

        equipment.EquipmentEquiped(hero);

        // OnEquipmentEquiped?.Invoke();
    }
}
}