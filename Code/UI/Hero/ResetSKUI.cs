using Managers;
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
public class ResetSKUI : MonoBehaviour
{
    private void Awake() => _resetButton.GetComponent<Button>().onClick.AddListener(() => OnReset());

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

    private void SpawnInCost()
    {
        cost = GlobalSettings.HeroResetCost(resetCount);

        Instantiate(_upgradeCostPrefab, _upgradeCostParent)
           .GetComponent<UpgradeCostUI>()
           .Init(CurrencyType.Dubloon, cost);
    }

    private void UpdateDisplay()
    {
        if (!PlayerManager.Heroes.GetHero(_heroId, out HeroData heroData))
            return;

        if (!Assets.GetHero(_heroId, out HeroSO _))
            return;

        _heroData = heroData;

        // get number of spent SP & Reset Count
        spentPoints = GlobalSettings.SkillPointsSpent(heroData);
        resetCount  = _heroData.ResetCount;

        ClearOldData();
        SpawnInCost();

        #region Do the checks
        // check if theres any sk to reset
        if (spentPoints == 0)
        {
            _resetButton.GetComponent<Button>().interactable = false;
            _errorText.text                                  = "Nothing to reset";
            return;
        }

        _resetButton.GetComponent<Button>().interactable = true;
        _errorText.text                                  = "";

        // check if can afford it
        if (PlayerManager.Currencies[CurrencyType.Dubloon] < cost)
        {
            _resetButton.GetComponent<Button>().interactable = false;
            _SPRecieve.text                                  = "";
            _errorText.text                                  = "Can't afford :(";
            return;
        }

        _resetButton.GetComponent<Button>().interactable = true;
        _SPRecieve.text                                  = $"Receive {spentPoints} Skill Points";
        _errorText.text                                  = "";
        #endregion
    }

    private void OnReset()
    {
        // close Panel on hero unlock
        _resetSKPanel.SetActive(false);

        ClientNetworkUtils.SendServerMessage(MessageNames.ResetAbility,
                                             new ResetAbilitiesMessageData(_heroId, spentPoints));
    }

    #region SerializeField
    [SerializeField]
    private TMP_Text _SPRecieve;

    [SerializeField]
    private Transform _upgradeCostParent;

    [SerializeField]
    private GameObject _upgradeCostPrefab;

    [Header("Other"), SerializeField]
    private GameObject _resetSKPanel;

    [SerializeField]
    private TMP_Text _errorText;

    [SerializeField]
    private GameObject _costSpawnPoint;

    [Header("Buttons"), SerializeField]
    private GameObject _resetButton;
    #endregion

    #region Other Variables
    private ushort   _heroId;
    private HeroData _heroData;

    private int resetCount;
    private int cost;
    private int spentPoints;
    #endregion
}
}