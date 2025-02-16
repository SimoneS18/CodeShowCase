using Shared.Scriptables.Hero;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hero.HeroSelection
{
public class HeroSelectionConfirmationUI : MonoBehaviour
{
    [SerializeField]
    private Button _cancelButton;

    [SerializeField]
    private Button _closeButton;

    [SerializeField]
    private Button _unlockButton;

    [SerializeField]
    private GameObject _mainHeroSelectionPanel;

    [SerializeField]
    private GameObject _selectionPanel;

    public void Awake()
    {
        _cancelButton.onClick.AddListener(() => HeroManager.OnSelectHero?.Invoke(null));
        _cancelButton.onClick.AddListener(() => OnCancelButtonPressed());

        _closeButton.onClick.AddListener(() => OnCancelButtonPressed());

        _unlockButton.onClick.AddListener(() => HeroManager.OnUnlockHeroSelection?.Invoke());

        HeroManager.OnSelectHero += OnHeroButtonPressed;
    }

    private void OnDestroy() => HeroManager.OnSelectHero -= OnHeroButtonPressed;

    private void OnHeroButtonPressed(HeroSO hero) => _selectionPanel.SetActive(hero != null);

    private void OnCancelButtonPressed() => _mainHeroSelectionPanel.SetActive(false);
}
}