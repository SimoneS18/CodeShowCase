using Messages.BaseBuilding.CardCrafting;
using Shared.Enums;
using Shared.Scriptables.Items;
using Shared.Utils;
using Shared.Utils.Identifiers;
using Shared.Utils.Values;
using TMPro;
using UnityEngine;
using Utils;

namespace UI.Equipment
{
public class RarityChanceUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _rarityName;

    [SerializeField]
    private TMP_Text _rarityChance;

    [SerializeField]
    private Rarity _rarity;

    private ushort _fragmentId;
    private int    _totalForRarity;
    private int    _totalFragments;

    private void Start()
    {
        _rarityName.color   = Colours.RarityColours[_rarity];
        _rarityChance.color = Colours.RarityColours[_rarity];
        _rarityName.text    = RarityNames.GetRarityName(_rarity);

        if (!Assets.GetItem(ItemsIds.FragmentIds[_rarity], out FragmentsSO fragment))
            return;

        _fragmentId = fragment.Id;

        // working on
        UiCraftSelectionBlueprint.OnResetButtonPressed += UiCraftSelectionBlueprint_ResetButtonPressed;
        UiFragmentSelection.OnPlusButtonPress          += UiFragmentSelection_OnPlusButtonPressed;
        UiFragmentSelection.OnSubButtonPress           += UiFragmentSelection_OnSubButtonPressed;
        UiFragmentSelection.OnAddMultipleButtonPress   += UiFragmentSelection_OnAddMultipleButtonPressed;

        BuildingCraftAddMessage.OnCraftAddLaboratory += BuildingCraftAddMessage_OnCraftAddLaboratory;
    }

    private void OnDisable()
    {
        // working on
        UiCraftSelectionBlueprint.OnResetButtonPressed -= UiCraftSelectionBlueprint_ResetButtonPressed;
        UiFragmentSelection.OnPlusButtonPress          -= UiFragmentSelection_OnPlusButtonPressed;
        UiFragmentSelection.OnSubButtonPress           -= UiFragmentSelection_OnSubButtonPressed;
        UiFragmentSelection.OnAddMultipleButtonPress   -= UiFragmentSelection_OnAddMultipleButtonPressed;

        BuildingCraftAddMessage.OnCraftAddLaboratory -= BuildingCraftAddMessage_OnCraftAddLaboratory;
    }

    private void UpdateDisplay()
    {
        int multiplier = GlobalSettings.Percentage;

        if (_totalForRarity == 0)
            _rarityChance.text = "0%";
        else
            _rarityChance.text = $"{_totalForRarity * multiplier}%";
    }

    private void UiCraftSelectionBlueprint_ResetButtonPressed()
    {
        _totalForRarity = 0;
        _totalFragments = 0;

        UpdateDisplay();
    }

    private void UiFragmentSelection_OnPlusButtonPressed(ushort fragmentId)
    {
        if (fragmentId == _fragmentId)
        {
            _totalForRarity++;
            _totalFragments++;
        }
        else
        {
            _totalFragments++;
        }

        UpdateDisplay();
    }

    private void UiFragmentSelection_OnSubButtonPressed(ushort fragmentId)
    {
        if (fragmentId == _fragmentId)
        {
            _totalForRarity--;
            _totalFragments--;
        }
        else
        {
            _totalFragments--;
        }

        UpdateDisplay();
    }

    private void UiFragmentSelection_OnAddMultipleButtonPressed(ushort fragmentId, int adding)
    {
        if (fragmentId == _fragmentId)
        {
            _totalForRarity = _totalForRarity + adding;
            _totalFragments = _totalFragments + adding;
        }
        else
        {
            _totalFragments = _totalFragments + adding;
        }

        UpdateDisplay();
    }

    private void BuildingCraftAddMessage_OnCraftAddLaboratory()
    {
        _totalForRarity = 0;
        _totalFragments = 0;

        UpdateDisplay();
    }
}
}