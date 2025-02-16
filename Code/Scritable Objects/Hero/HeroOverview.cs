using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;

namespace Shared.Scriptables.Hero
{
[GlobalConfig("Resources/ScriptableObjects/Heros/Hero")]
public class HeroOverview : GlobalConfig<HeroOverview>
{
    [ReadOnly, ListDrawerSettings(IsReadOnly = true)]
    public HeroSO[] _allCharacters;

    [Button(ButtonSizes.Medium), PropertyOrder(-1)]
    public void UpdateHeroOverview()
    {
        // t: Is the SO we are filter name, not any file names
        // Finds and assigns all scriptable objects of type Character
        _allCharacters = AssetDatabase.FindAssets("t:HeroSO")
                                     .Select(guid => AssetDatabase.LoadAssetAtPath<HeroSO>(AssetDatabase.GUIDToAssetPath(guid)))
                                     .ToArray();
        
        Array.Sort(_allCharacters, new ScriptableObjectComparer());
    }
    
    private class ScriptableObjectComparer : IComparer<HeroSO> 
    {
        public int Compare(HeroSO x, HeroSO y)
        {
            if (x != null & y != null)
                return string.CompareOrdinal(x.name, y.name);

            return 0;
        }
    }
}
}