using Managers;
using Messages.Hero;
using Shared.Data.Hero;
using Shared.Scriptables.Hero;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Hero
{
public class HeroSkillTreeUI : MonoBehaviour
{
    [Header("Appearance"), SerializeField]
    private TMP_Text _header;

    [SerializeField]
    private TMP_Text _spAvailable;

    [SerializeField]
    private GameObject _resetButton;

    [Header("Spawn Point"), SerializeField]
    private GameObject _spawnPoint;

    [Header("Close Button"), SerializeField]
    private GameObject _closeButton_info;

    [SerializeField]
    private GameObject _closeButton_ST;

    private ushort _heroId;
    private ushort _stId;

    public void Init(ushort heroID, ushort STID)
    {
        _heroId = heroID;
        _stId   = STID;

        UpgradeDisplay();
        UpdateText();
    }

    private void ClearData()
    {
        foreach (Transform child in _spawnPoint.GetComponent<Transform>())
            Destroy(child.gameObject);
    }

    private void UpgradeDisplay()
    {
        ClearData();

        Assets.GetHero(_heroId, out HeroSO _);
        Assets.GetHeroSkillTree(_stId, out SkillTreeSO skillTreeSO);

        _header.text = skillTreeSO.Name;

        Instantiate(skillTreeSO.SkillTreePrefab, _spawnPoint.GetComponent<Transform>())
           .GetComponent<SkillTreeSetUpUI>()
           .Init(_heroId, _stId);
    }

    private void UpdateText()
    {
        PlayerManager.Heroes.GetHero(_heroId, out HeroData heroData);
        _spAvailable.text = $"{heroData.SkillPoints}";
    }

    private void OnResetClick() => HeroPopupNavigation.OnShowResetSTUI?.Invoke(_heroId);

    #region OnEnable | OnDisable | Other
    private void Awake() => _resetButton.GetComponent<Button>().onClick.AddListener(() => OnResetClick());

    private void OnEnable()
    {
        LevelUpAbilityMessage.OnHeroAbilityLeveled += LevelUpAbilityMessage_OnHeroUpgraded;
        UpgradeHeroMessage.OnHeroUpgraded          += LevelUpAbilityMessage_OnHeroUpgraded;
        ResetAbilitiesMessage.OnResetPressed       += ResetAbilitiesMessage_OnResetPressed;

        _closeButton_info.SetActive(false);
        _closeButton_ST.SetActive(true);
    }

    private void OnDisable()
    {
        LevelUpAbilityMessage.OnHeroAbilityLeveled -= LevelUpAbilityMessage_OnHeroUpgraded;
        UpgradeHeroMessage.OnHeroUpgraded          -= LevelUpAbilityMessage_OnHeroUpgraded;
        ResetAbilitiesMessage.OnResetPressed       -= ResetAbilitiesMessage_OnResetPressed;
    }

    private void LevelUpAbilityMessage_OnHeroUpgraded(int level) => UpdateText();

    private void ResetAbilitiesMessage_OnResetPressed() => UpdateText();
    #endregion
}
}