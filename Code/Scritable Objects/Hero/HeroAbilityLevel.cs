using System;
using Shared.Data;
using Shared.Enums;
using Shared.Enums.Units;
using Shared.Managers;
using Shared.Structs;
using Shared.Structs.Battles;
using UnityEngine;

namespace Shared.Scriptables.Hero
{
[Serializable]
public class HeroAbilityLevel
{
    [SerializeField]
    protected Stat[] _stats;

    [SerializeField]
    private StatusEffectDetails[] _statusEffects;

    public Stat[] Stats => _stats;
    public StatusEffectDetails[] StatusEffects => _statusEffects;

    public Stat GetStat(StatType stat)
    {
        for (int i = 0; i < Stats.Length; i++)
        {
            if (Stats[i].StatType == stat)
                return Stats[i];
        }

        return new Stat();
    }

    public float GetModifiedStat(StatType statType, ushort id, PlayerData playerData)
    {
        if (HasStat(statType))
            return StatManager.GetModifiedStat(StatOwnerType.Hero, GetStat(statType), id, playerData);

        return 0;
    }

    public bool HasStat(StatType stat)
    {
        for (int i = 0; i < Stats.Length; i++)
        {
            if (Stats[i].StatType == stat)
                return true;
        }

        return false;
    }
}
}