using Shared.Scriptables.Hero;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hero.HeroSelection
{
public class HeroSelectionButton : MonoBehaviour
{
    [SerializeField]
    private Image _heroImage;

    [SerializeField]
    private TMP_Text _heroText;

    // change the following to something like the followin
    //     internal void Init(IconType iconType, ushort buildingId, BasePlaceableData data, int buildingIndex)
    internal void Init(HeroSO data)
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => HeroManager.OnSelectHero?.Invoke(data));

        _heroText.text    = data.name;
        _heroImage.sprite = data.FullBody;
    }
}
}