using Managers;
using Shared.Data;
using Shared.Data.Hero;
using Shared.Enums;
using Shared.MessageCore;
using Shared.MessageData.Hero;
using Shared.Scriptables.Hero;
using Shared.Utils.Values;
using TMPro;
using UI.BaseBuilding;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Hero
{
/// <summary>
///     Following Script is used when a hero is pressed, and the hero is not brought yet
/// </summary>
public class UnlockHeroUI : MonoBehaviour
{
    [Header("Appearance"), SerializeField]
    private TMP_Text _heroName;

    [SerializeField]
    private TMP_Text _heroDescription;

    [SerializeField]
    private Image _heroImage;

    [Header("Other"), SerializeField]
    private GameObject _heroUnlockPanel;

    [SerializeField]
    private TMP_Text _errorText;

    [SerializeField]
    private GameObject _costPrefab;

    [SerializeField]
    private GameObject _costSpawnPoint;

    [Header("Buttons"), SerializeField]
    private GameObject _unlockButton;

    private HeroData _heroData;

    private ushort       _heroId;
    private int          _heroLevel;
    private CurrencyData escudoCurrency;
    private CurrencyData scrapCurrency;

    private void Awake() => _unlockButton.GetComponent<Button>().onClick.AddListener(() => OnUnlock());

    internal void Init(ushort HeroId)
    {
        _heroId = HeroId;
        UpdateDisplay();
    }

    private void ClearOldData()
    {
        foreach (Transform child in _costSpawnPoint.GetComponent<Transform>())
            Destroy(child.gameObject);
    }

    private void SpawnInCost(HeroSO hero, int level)
    {
        if (level < GlobalSettings.HeroMaxLevel &&
            PlayerManager.Base.HasHqLevel(GlobalSettings.HeroHqRequirement[level]))
            Instantiate(_costPrefab, _costSpawnPoint.GetComponent<Transform>())
               .GetComponent<UpgradeCostUI>()
               .Init(scrapCurrency);
    }

    private void UpdateDisplay()
    {
        if (!Assets.GetHero(_heroId, out HeroSO hero))
            return;

        _heroLevel = 0;

        scrapCurrency = new CurrencyData(CurrencyType.Scrap,
                                         GlobalSettings.HeroScrapCostPerLevel[_heroLevel]);

        escudoCurrency = new CurrencyData(CurrencyType.Escudo,
                                          GlobalSettings.HeroEscudoCostPerLevel[_heroLevel]);

        ClearOldData();
        SpawnInCost(hero, _heroLevel);

        _heroName.text        = hero.name;
        _heroDescription.text = hero.Description;
        _heroImage.sprite     = hero.FullBody;

        #region Check can afford
        if (PlayerManager.Currencies[scrapCurrency.Type] < scrapCurrency.Amount)
        {
            _unlockButton.GetComponent<Button>().interactable = false;
            _errorText.text                                   = ToastMessages.CannotAfford;
            return;
        }

        if (PlayerManager.Currencies[escudoCurrency.Type] < escudoCurrency.Amount)
        {
            _unlockButton.GetComponent<Button>().interactable = false;
            _errorText.text                                   = ToastMessages.CannotAfford;
            return;
        }
        #endregion

        _errorText.text = "";
    }

    private void OnUnlock()
    {
        _heroUnlockPanel.SetActive(false);

        ClientNetworkUtils.SendServerMessage(MessageNames.UnlockHero, new UnlockHeroMessageData(_heroId));
    }
}
}