using System;
using UnityEngine;

namespace UI.Hero
{
[Serializable]
public class HeroCountController
{
    [SerializeField]
    private byte _heroCount;

    public byte HeroCount => _heroCount;
}
}