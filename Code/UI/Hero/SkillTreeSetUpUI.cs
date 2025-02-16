using Shared.Scriptables.Hero;
using UnityEngine;
using Utils;

namespace UI.Hero
{
public class SkillTreeSetUpUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _point;

    [SerializeField]
    private GameObject[] _lines;

    private ushort _heroId;
    private ushort _stId;

    public GameObject Point => _point;

    public void Init(ushort heroId, ushort stId)
    {
        _heroId = heroId;
        _stId   = stId;

        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        Assets.GetHeroSkillTree(_stId, out SkillTreeSO skillTreeSO);

        for (int i = 0; i < Point.GetComponent<Transform>().childCount; i++)
        {
            // #STODO - redo do this for new st
            ushort abilityId = skillTreeSO.Abilities[i].Id;
            Point.GetComponent<Transform>().GetChild(i).GetComponent<AbilityButtonUI>().Init(_heroId, abilityId, _stId);
        }

        _lines[0].GetComponent<AbilityLineUI>().Init(_heroId, _stId, 0, 1);
        _lines[1].GetComponent<AbilityLineUI>().Init(_heroId, _stId, 1, 2);
        _lines[2].GetComponent<AbilityLineUI>().Init(_heroId, _stId, 2, 3);
        _lines[3].GetComponent<AbilityLineUI>().Init(_heroId, _stId, 3, 4);
        _lines[4].GetComponent<AbilityLineUI>().Init(_heroId, _stId, 4, 9);
        _lines[5].GetComponent<AbilityLineUI>().Init(_heroId, _stId, 0, 5);
        _lines[6].GetComponent<AbilityLineUI>().Init(_heroId, _stId, 5, 6);
        _lines[7].GetComponent<AbilityLineUI>().Init(_heroId, _stId, 6, 7);
        _lines[8].GetComponent<AbilityLineUI>().Init(_heroId, _stId, 7, 8);
        _lines[9].GetComponent<AbilityLineUI>().Init(_heroId, _stId, 8, 9);
    }
}
}