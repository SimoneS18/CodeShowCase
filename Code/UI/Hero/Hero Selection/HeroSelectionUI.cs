using System.Collections.Generic;
using System.Linq;
using Shared.Scriptables.Hero;
using UnityEngine;

namespace UI.Hero.HeroSelection
{
public class HeroSelectionUI : MonoBehaviour
{
    [Header("Prefabs"), SerializeField]
    private GameObject _heroSelectionButton;

    [Header("Transform"), SerializeField]
    private Transform _heroSelectionSpawnPoint;

    //[Header("Buttons")]
    //[SerializeField] private GameObject _selectedButton;

    private List<HeroSO> _heros;

    private void Awake()
    {
        _heros = Resources.LoadAll<HeroSO>("ScriptableObjects/Hero").ToList();

        Instantiate(_heros);
    }

    private void Instantiate <T>(List<T> objects) where T : HeroSO
    {
        foreach (HeroSO hero in objects)
        {
            GameObject heroButton = Instantiate(_heroSelectionButton, _heroSelectionSpawnPoint);
            heroButton.GetComponent<HeroSelectionButton>().Init(hero);
        }
    }
}
}