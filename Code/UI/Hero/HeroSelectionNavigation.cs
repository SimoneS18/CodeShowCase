using System;
using UI.Hero.SkillTree;
using UnityEngine;

namespace UI.Hero
{
public class HeroSelectionNavigation : MonoBehaviour
{
    static public Action<ushort> OnShowUI;

    static public Action<ushort> OnShowSTSelection;

    static public Action<ushort, ushort> OnShowSTAfterSelection;
    static public Action<ushort, ushort> OnShowSTAlreadySelected;

    static public Action<ushort> OnSkillTreeReset;

    [SerializeField]
    private GameObject _heroSelectionUI;

    [SerializeField]
    private GameObject _heroPanel;

    [SerializeField]
    private GameObject _STSelectionPanel;

    [SerializeField]
    private GameObject _STPanel;

    [Header("Appearance"), SerializeField]
    private GameObject _background;

    private void Awake()
    {
        OnShowUI                += HeroNavigation_OnShowUI;
        OnShowSTSelection       += HeroNavigation_OnShowSTSelection;
        OnShowSTAfterSelection  += HeroNavigation_OnShowSTAfterSelection;
        OnShowSTAlreadySelected += HeroNavigation_OnShowSTAlreadySelected;
        OnSkillTreeReset        += HeroNavigation_OnSkillTreeReset;
    }

    private void OnDestroy()
    {
        OnShowUI                -= HeroNavigation_OnShowUI;
        OnShowSTSelection       -= HeroNavigation_OnShowSTSelection;
        OnShowSTAfterSelection  -= HeroNavigation_OnShowSTAfterSelection;
        OnShowSTAlreadySelected -= HeroNavigation_OnShowSTAlreadySelected;
        OnSkillTreeReset        -= HeroNavigation_OnSkillTreeReset;
    }

    private void HeroNavigation_OnShowUI(ushort id)
    {
        _background.SetActive(true);
        _heroSelectionUI.SetActive(false);
        _heroPanel.SetActive(true);
        _heroPanel.GetComponent<HeroUI>().Init(id);
    }

    private void HeroNavigation_OnShowSTSelection(ushort id)
    {
        _background.SetActive(true);
        _heroPanel.SetActive(false);
        _STSelectionPanel.SetActive(true);
        _STSelectionPanel.GetComponent<HeroSkillTreePanalUI>().Init(id);
    }

    private void HeroNavigation_OnShowSTAfterSelection(ushort heroID, ushort STID)
    {
        _background.SetActive(true);
        _STSelectionPanel.SetActive(false);
        _STPanel.SetActive(true);
        _STPanel.GetComponent<HeroSkillTreeUI>().Init(heroID, STID);
    }

    private void HeroNavigation_OnShowSTAlreadySelected(ushort heroID, ushort STID)
    {
        _background.SetActive(true);
        _heroPanel.SetActive(false);
        _STPanel.SetActive(true);
        _STPanel.GetComponent<HeroSkillTreeUI>().Init(heroID, STID);
    }

    private void HeroNavigation_OnSkillTreeReset(ushort heroID)
    {
        _STPanel.SetActive(false);
        _STSelectionPanel.SetActive(true);
        _STSelectionPanel.GetComponent<HeroSkillTreePanalUI>().Init(heroID);
    }
}
}