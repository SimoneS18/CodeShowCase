using Managers;
using Messages.Equipment;
using Messages.Hero;
using Shared.Data;
using Shared.Data.Hero;
using Shared.Data.PlayerBase.BaseShip;
using Shared.Data.PlayerBase.BuildingData;
using Shared.Enums;
using Shared.Enums.Units;
using Shared.Managers;
using Shared.MessageCore;
using Shared.MessageData.BaseBuilding.EquipmentCrafting;
using Shared.MessageData.Hero;
using Shared.Scriptables.Hero;
using Shared.Structs;
using Shared.Utils.Identifiers;
using Shared.Utils.Values;
using TMPro;
using UI.BaseBuilding;
using UI.Stats;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Hero
{
public class HeroUI : MonoBehaviour
{
    #region Serialize's
    [Header("Appearance"), SerializeField]
    private TMP_Text _heroName;

    [SerializeField]
    private TMP_Text _skillPointsAvailable;

    [SerializeField]
    private Image _heroFullBody;

    [Header("Cost Info"), SerializeField]
    private GameObject _costPanel;

    [SerializeField]
    private Transform _upgradeCostParent;

    [SerializeField]
    private GameObject _upgradeCostPrefab;

    [Header("Stats"), SerializeField]
    private Transform _statSpawnPoint;

    [SerializeField]
    private GameObject _statPrefab;

    [Header("Buttons"), SerializeField]
    private GameObject _upgradeButton;

    [SerializeField]
    private GameObject _resetButton;

    [SerializeField]
    private GameObject _STButton;

    [Header("Additional Features"), SerializeField]
    private TMP_Text _errorText;

    [SerializeField]
    private TMP_Text _skillPointReceiveAfterUpdate;

    [SerializeField]
    private GameObject[] EquipmentSlots;

    [Header("Close Button"), SerializeField]
    private GameObject _closeButton_Selection;

    [SerializeField]
    private GameObject _closeButton_Info;
    #endregion

    #region Private
    private ushort _heroId;
    private ushort _SKId;
    private int    _heroLevel;
    private int    _skillPoints;
    private int    SkillTreeLevelNumber;
    private int    currentLevelIndex;

    private HeroSO        _currentHero;
    private BaseShipsData hq;
    private HeroData      _heroData;
    #endregion

    #region Awake, OnEnable, OnDisable
    private void Awake()
    {
        PlayerManager.Base.GetBuilding(BuildingIds.HQ, out BuildingsData hq);

        _upgradeButton.GetComponent<Button>().onClick.AddListener(() => OnUpgrade());
        _resetButton.GetComponent<Button>().onClick.AddListener(() => OnReset(_heroId));
        _STButton.GetComponent<Button>().onClick.AddListener(() => OnSTButtonPressed());
    }

    private void OnEnable()
    {
        UpgradeHeroMessage.OnHeroUpgraded          += UpgradeHeroMessage_OnHeroUpgraded;
        LevelUpAbilityMessage.OnHeroAbilityLeveled += LevelUpAbilityMessage_OnHeroLeveled;
        ResetAbilitiesMessage.OnResetPressed       += ResetAbilitiesMessage_OnResetPressed;

        EquipEquipmentMessage.OnEquipmentEquiped           += EquipEquipmentMessage_OnEquip;
        EquipmentCraftCompleteMessage.OnEquipmentCollected += EquipEquipmentMessage_OnEquip;

        _closeButton_Selection.SetActive(false);
        _closeButton_Info.SetActive(true);
    }

    private void OnDisable()
    {
        UpgradeHeroMessage.OnHeroUpgraded          -= UpgradeHeroMessage_OnHeroUpgraded;
        LevelUpAbilityMessage.OnHeroAbilityLeveled -= LevelUpAbilityMessage_OnHeroLeveled;
        ResetAbilitiesMessage.OnResetPressed       -= ResetAbilitiesMessage_OnResetPressed;

        EquipEquipmentMessage.OnEquipmentEquiped           -= EquipEquipmentMessage_OnEquip;
        EquipmentCraftCompleteMessage.OnEquipmentCollected -= EquipEquipmentMessage_OnEquip;
    }
    #endregion

    #region Functions
    internal void Init(ushort id)
    {
        _heroId = id;

        // #STODO - see if need 
        foreach (GameObject slot in EquipmentSlots)
            slot.GetComponent<HeroEquipmentUI>().Init(_heroId);

        UpdateDisplay();
    }

    #region Actions from other scripts
    private void UpgradeHeroMessage_OnHeroUpgraded(int level) => UpgradeDisplayOnUpgrade();

    private void ResetAbilitiesMessage_OnResetPressed() => UpdateDisplay();

    private void EquipEquipmentMessage_OnEquip() => UpdateDisplay();

    private void LevelUpAbilityMessage_OnHeroLeveled(int level) => UpdateDisplay();
    #endregion

    private void ClearOldData()
    {
        foreach (Transform child in _upgradeCostParent)
            Destroy(child.gameObject);

        foreach (Transform child in _statSpawnPoint)
            Destroy(child.gameObject);
    }

    private void ClearCost()
    {
        foreach (Transform child in _upgradeCostParent)
            Destroy(child.gameObject);
    }

    /// <summary>
    ///     Panel Opened
    /// </summary>
    private void UpdateDisplay()
    {
        if (!PlayerManager.Heroes.GetHero(_heroId, out HeroData heroData))
            return;

        if (!Assets.GetHero(_heroId, out HeroSO hero))
            return;

        _heroData    = heroData;
        _currentHero = hero;
        _heroLevel   = heroData.Level;

        ClearOldData();
        DisplayCost(_heroLevel);
        DisplayStats();

        // set up text
        _heroName.text                     = $"{hero.FirstName} (lvl {_heroLevel})";
        _heroFullBody.sprite               = hero.FullBody;
        _skillPointsAvailable.text         = $"{_heroData.SkillPoints}";
        _skillPointReceiveAfterUpdate.text = $"Skill Points: {GlobalSettings.HeroSkillPointReceived[heroData.Level]}";
    }

    /// <summary>
    ///     Upgrade Display On Upgrade
    /// </summary>
    private void UpgradeDisplayOnUpgrade()
    {
        if (!PlayerManager.Heroes.GetHero(_heroId, out HeroData heroData))
            return;

        if (!Assets.GetHero(_heroId, out HeroSO hero))
            return;

        foreach (GameObject slot in EquipmentSlots)
            slot.GetComponent<HeroEquipmentUI>().Init(_heroId);

        _heroData    = heroData;
        _currentHero = hero;
        _heroLevel   = heroData.Level;

        ClearCost();
        DisplayCost(_heroLevel);

        // set up text
        _heroName.text                     = $"{_currentHero.FirstName} (lvl {_heroLevel})";
        _skillPointsAvailable.text         = $"{_heroData.SkillPoints}";
        _skillPointReceiveAfterUpdate.text = $"Skill Points: {GlobalSettings.HeroSkillPointReceived[_heroData.Level]}";
    }

    /// <summary>
    ///     Display Cost and do all the checks
    /// </summary>
    private void DisplayCost(int level)
    {
        CurrencyData ScrapCost = new CurrencyData(CurrencyType.Scrap,
                                                  GlobalSettings.HeroScrapCostPerLevel[level]);

        CurrencyData EscudoCost = new CurrencyData(CurrencyType.Escudo,
                                                   GlobalSettings.HeroEscudoCostPerLevel[level]);

        Instantiate(_upgradeCostPrefab, _upgradeCostParent.GetComponent<Transform>())
           .GetComponent<UpgradeCostUI>()
           .Init(ScrapCost);

        // only spawn in Escudo only when its greater then 0
        if (GlobalSettings.HeroEscudoCostPerLevel[level] > 0)
            Instantiate(_upgradeCostPrefab, _upgradeCostParent.GetComponent<Transform>())
               .GetComponent<UpgradeCostUI>()
               .Init(EscudoCost);

        if (PlayerManager.Base.HasHqLevel(GlobalSettings.HeroHqRequirement[level]) == false)
        {
            _upgradeButton.GetComponent<Button>().interactable = false;
            _errorText.text                                    = ToastMessages.HQRequired(GlobalSettings.HeroHqRequirement[level]);

            return;
        }

        if (level == GlobalSettings.HeroMaxLevel)
        {
            _upgradeButton.GetComponent<Button>().interactable = false;
            _errorText.text                                    = ToastMessages.MaxHeroLevel;

            return;
        }

        if (hq.Positions[0].Level < GlobalSettings.HeroHqRequirement[currentLevelIndex])
        {
            _upgradeButton.GetComponent<Button>().interactable = false;
            _errorText.text                                    = $"{GlobalSettings.HeroHqRequirement[currentLevelIndex]} Hq Lvl needed";

            return;
        }

        if (PlayerManager.Currencies[ScrapCost.Type] < ScrapCost.Amount)
        {
            _upgradeButton.GetComponent<Button>().interactable = false;
            _errorText.text                                    = ToastMessages.NotEnoughScrap;

            return;
        }

        if (PlayerManager.Currencies[EscudoCost.Type] < EscudoCost.Amount)
        {
            _upgradeButton.GetComponent<Button>().interactable = false;
            _errorText.text                                    = ToastMessages.NotEnoughEscudo;

            return;
        }

        _upgradeButton.GetComponent<Button>().interactable = true;
        _errorText.text                                    = "";
    }

    private void DisplayStats()
    {
        if (ClientSettings.SimpleStats)
            ShowSimpleStats();
        else
            foreach (HeroStats stat in _currentHero.HeroStats)
            {
                float current = HeroSO.CalculateStat(_heroLevel, stat.BaseValue, stat.IncreaseValue);

                float bonus = StatManager.GetModifiedStat(StatOwnerType.Hero,
                                                          new Stat(stat.StatType, current, stat.IsMultiplier),
                                                          PlayerManager.Data) - current;

                GameObject obj = Instantiate(_statPrefab, _statSpawnPoint);
                obj.GetComponent<UIStatsBonus>().Init_Hero(_currentHero, stat, bonus);
            }
    }

    private void ShowSimpleStats()
    {
        float[] currentStats = _currentHero.GetSimpleStats(_heroLevel - 1);
        float[] nextStats    = _currentHero.GetSimpleStats(_heroLevel);
        float[] maxStats     = _currentHero.GetSimpleStats(GlobalSettings.HeroMaxLevel);

        GameObject obj = Instantiate(_statPrefab, _statSpawnPoint);

        obj.GetComponent<UIStatsBonus>()
           .InitValues("Attack",
                       Assets.GetStatIconImage(StatType.Damage),
                       currentStats[0],
                       nextStats[0],
                       0,
                       maxStats[0]);

        obj = Instantiate(_statPrefab, _statSpawnPoint);

        obj.GetComponent<UIStatsBonus>()
           .InitValues("Defence",
                       Assets.GetStatIconImage(StatType.Defence),
                       currentStats[1],
                       nextStats[1],
                       0,
                       maxStats[1]);

        obj = Instantiate(_statPrefab, _statSpawnPoint);

        obj.GetComponent<UIStatsBonus>()
           .InitValues("Speed",
                       Assets.GetStatIconImage(StatType.MovementSpeed),
                       currentStats[2],
                       nextStats[2],
                       0,
                       maxStats[2]);
    }

    private void OnUpgrade() => ClientNetworkUtils.SendServerMessage(MessageNames.UpgradeHero, new UpgradeHeroMessageData(_heroId));

    private void OnReset(ushort heroId) => HeroPopupNavigation.OnShowResetSTUI?.Invoke(heroId);

    // following functions determines if a SkillTree has already been selected or not and opens the correct panel
    private void OnSTButtonPressed()
    {
        if (_heroData.SelectedST == 0)
            HeroSelectionNavigation.OnShowSTSelection?.Invoke(_heroId);
        else
            HeroSelectionNavigation.OnShowSTAlreadySelected?.Invoke(_heroId, _heroData.SelectedST);
    }
    #endregion
}
}