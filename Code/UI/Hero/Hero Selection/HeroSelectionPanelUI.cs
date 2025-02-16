using System.Collections.Generic;
using System.Linq;
using Shared.Scriptables.Hero;
using UnityEngine;

namespace UI.Hero.HeroSelection
{
/// <summary>
///     Following script spawns in the hero buttons on the hero selection Panel
/// </summary>
public class HeroSelectionPanelUI : MonoBehaviour
{
    [Header("Prefabs"), SerializeField]
    private GameObject _heroButton;

    [Header("SpawnPoint"), SerializeField]
    private Transform _heroSpawnPoint;

    private List<HeroSO> _heroes;

    private void Awake()
    {
        _heroes = Resources.LoadAll<HeroSO>("ScriptableObjects/Hero").Where(x => x.Enabled).ToList();

        ClearData();

        Instantiate(_heroes);
    }

    private void Instantiate <T>(List<T> objects) where T : HeroSO
    {
        foreach (HeroSO hero in _heroes)
        {
            GameObject heroButton = Instantiate(_heroButton, _heroSpawnPoint);
            heroButton.GetComponent<HeroButtonUI>().Init(hero.Id);
        }
    }

    private void ClearData()
    {
        foreach (Transform child in _heroSpawnPoint)
            Destroy(child.gameObject);
    }
}
}