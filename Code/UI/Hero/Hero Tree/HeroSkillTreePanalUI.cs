using System.Collections.Generic;
using Shared.Scriptables.Hero;
using UnityEngine;
using Utils;

namespace UI.Hero.SkillTree
{
/// <summary>
///     Following script spawns in Skill Tree buttons for the player to Select - only if selectedSt is 0
/// </summary>
public class HeroSkillTreePanalUI : MonoBehaviour
{
    [Header("Prefabs"), SerializeField]
    private GameObject _Button;

    [Header("SpawnPoint"), SerializeField]
    private GameObject _SpawnPoint;

    [Header("Close Button"), SerializeField]
    private GameObject _closeButton_Info;

    [SerializeField]
    private GameObject _closeButton_STSelection;

    private ushort _heroId;

    private List<SkillTreeSO> _skillTrees = new List<SkillTreeSO>();

    private void OnEnable()
    {
        _closeButton_Info.SetActive(false);
        _closeButton_STSelection.SetActive(true);
    }

    public void Init(ushort id)
    {
        _heroId = id;

        UpgradeDisplay();
    }

    private void ClearData()
    {
        foreach (Transform child in _SpawnPoint.GetComponent<Transform>())
            Destroy(child.gameObject);
    }

    private void UpgradeDisplay()
    {
        Assets.GetHero(_heroId, out HeroSO heroSO);

        ClearData();

        foreach (SkillTreeSO STSO in heroSO.SkillTrees)
        {
            GameObject heroButton = Instantiate(_Button, _SpawnPoint.GetComponent<Transform>());
            heroButton.GetComponent<StSelectionButton>().Init(_heroId, STSO);
        }
    }
}
}