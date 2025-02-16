using System;
using Shared.Enums.Units;
using UnityEngine;

namespace Shared.Scriptables.Hero
{
[Serializable]
public class HeroStats
{
    [SerializeField]
    private StatType _statType;

    [SerializeField]
    private float _baseValue;

    [SerializeField]
    private float _increaseValue;

    [SerializeField]
    private bool _multiplier;

    public StatType StatType => _statType;
    public float BaseValue => _baseValue;
    public float IncreaseValue => _increaseValue;
    public bool IsMultiplier => _multiplier;
}
}