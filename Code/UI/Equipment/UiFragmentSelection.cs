using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Messages.BaseBuilding;
using Messages.BaseBuilding.CardCrafting;
using Shared.Data.Equipment;
using Shared.Data.PlayerBase.BuildingData;
using Shared.Enums;
using Shared.Scriptables.Equipment;
using Shared.Scriptables.Items;
using Shared.Utils;
using Shared.Utils.Identifiers;
using Shared.Utils.Values;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Equipment
{
public class UiFragmentSelection : MonoBehaviour
{
    static public Action<ushort>      OnPlusButtonPress;
    static public Action<ushort>      OnSubButtonPress;
    static public Action<ushort, int> OnAddMultipleButtonPress;

    [SerializeField] private Image    _imageColor;
    [SerializeField] private TMP_Text _numberOfFrags;
    [SerializeField] private Rarity   _rarity;

    [Header("Increase/Decrease")] 
    [SerializeField] private TMP_Text _numberOfFragsSelected;
    [SerializeField] private Button   _subtractFragButton;
    [SerializeField] private Button   _addFragButton;
    [SerializeField] private Button   _plusToMaxButton;
    [SerializeField] private TMP_Text _plusMultipleText;

    [Header("Debugging")]
    [SerializeField] private bool _debuggingEnabled;
    [SerializeField] private GameObject _parent;
    [SerializeField] private TMP_Text   _canDo;
    [SerializeField] private TMP_Text   _have;
    
    private bool   _inactiveButton;
    private int    _fragmentSlotsLeft;
    private int    _maxToAdd;
    private int    _queueTotalCount;
    private int    _numberOfFragmentsForThatRarity;
    private int    _rarityNumberMinusSelected;
    private int    _requiredToResearch;
    private int    _selectedFragmentsRarity;
    private int    _selectedFragmentsTotal;
    private ushort _buildingId;
    private ushort _fragmentId;

    private void OnEnable()
    {
        _inactiveButton                                        =  true;
        UiBlueprintCrafting.OnFirstOpenUpdateDisplay           += UiBlueprintCrafting_OnFirstOpenUpdateDisplay;
        UiCraftSelectionBlueprint.OnResetButtonPressed         += UiCraftSelectionBlueprint_ResetButtonPressed;
        UpdateItemsMessage.OnItemUpdated                       += UpdateItemsMessage_OnItemUpdate;
        BuildingCraftAddMessage.OnCraftAddLaboratory           += BlueprintCraftAddMessage_OnCraftAddLaboratory;
        BuildingCraftCancelMessage.OnCraftCancelLaboratory     += BuildingCraftCancelMessage_OnCraftCancelLaboratory;
        BuildingCraftCompleteMessage.OnCraftCompleteLaboratory += BuildingCraftCompleteMessage_OnCraftCompleteLaboratory;

        OnPlusButtonPress        += OnPlusButtonPressed;
        OnSubButtonPress         += OnSubButtonPressed;
        OnAddMultipleButtonPress += OnAddMultipleButtonPressed;
    }

    private void OnDisable()
    {
        UiBlueprintCrafting.OnFirstOpenUpdateDisplay           -= UiBlueprintCrafting_OnFirstOpenUpdateDisplay;
        UiCraftSelectionBlueprint.OnResetButtonPressed         -= UiCraftSelectionBlueprint_ResetButtonPressed;
        UpdateItemsMessage.OnItemUpdated                       -= UpdateItemsMessage_OnItemUpdate;
        BuildingCraftAddMessage.OnCraftAddLaboratory           -= BlueprintCraftAddMessage_OnCraftAddLaboratory;
        BuildingCraftCancelMessage.OnCraftCancelLaboratory     -= BuildingCraftCancelMessage_OnCraftCancelLaboratory;
        BuildingCraftCompleteMessage.OnCraftCompleteLaboratory -= BuildingCraftCompleteMessage_OnCraftCompleteLaboratory;

        OnPlusButtonPress        -= OnPlusButtonPressed;
        OnSubButtonPress         -= OnSubButtonPressed;
        OnAddMultipleButtonPress -= OnAddMultipleButtonPressed;
    }

    private void UiBlueprintCrafting_OnFirstOpenUpdateDisplay(ushort buildingId, int buildingIndex)
    {
        // check if we can get that rarity 
        if (!Assets.GetItem(ItemsIds.FragmentIds[_rarity], out FragmentsSO fragment))
            return;
        
        // Set variables
        _fragmentId = fragment.Id;
        GetRarityFragmentCount();
        _requiredToResearch             = (int)GlobalSettings.FragmentsToResearch;
        _buildingId                     = buildingId;
        
        // remove all listeners from buttons
        _subtractFragButton.onClick.RemoveAllListeners();
        _addFragButton.onClick.RemoveAllListeners();
        _plusToMaxButton.onClick.RemoveAllListeners();

        // assigned listeners to buttons
        _subtractFragButton.onClick.AddListener(SubButtonAction);
        _addFragButton.onClick.AddListener(AddButtonPressed);
        _plusToMaxButton.onClick.AddListener(AddMultipleButtonAction);

        _imageColor.color        = Colours.RarityColours[_rarity];
        _selectedFragmentsRarity = 0;

        UpdateDisplayAndQueue(false);
    }

    private void GetRarityFragmentCount()
    {
        _numberOfFragmentsForThatRarity = (int)PlayerManager.Items.GetItemCount(_fragmentId);
        Debug.Log($"Fragment Count: {_numberOfFragmentsForThatRarity}");
    }

    private void UiCraftSelectionBlueprint_ResetButtonPressed() => UpdateDisplayAndQueue(true);

    private void BlueprintCraftAddMessage_OnCraftAddLaboratory() => UpdateDisplayAndQueue(true);

    private void BuildingCraftCancelMessage_OnCraftCancelLaboratory() => UpdateDisplayAndQueue(false);

    private void BuildingCraftCompleteMessage_OnCraftCompleteLaboratory() => UpdateDisplayAndQueue(false);

    private void UpdateItemsMessage_OnItemUpdate()
    {
        GetRarityFragmentCount();
        UpdateDisplayAndQueue(false);
    }

    /// <summary>
    ///     Update Both Display and Update Queue Information
    /// </summary>
    /// <param name="resetFragments">TRUE: Reset || FALSE: Do not reset</param>
    private void UpdateDisplayAndQueue(bool resetFragments)
    {
        if (resetFragments)
        {
            _selectedFragmentsRarity = 0;
            _selectedFragmentsTotal  = 0;
        }

        UpdateDisplay();
        UpdateDisplayWithQueue();
    }

    private void UpdateDisplayWithQueue()
    {
        if (!PlayerManager.Base.GetBuilding(_buildingId, out BuildingsData placeableData))
            return;

        bool hitQueue = Assets.HitMaxQueueCount(_buildingId, placeableData);

        if (hitQueue)
            SetButtonStates(false, false);
    }

    private void OnPlusButtonPressed(ushort addedFragmentId)
    {
        if (addedFragmentId == _fragmentId)
        {
            _selectedFragmentsRarity++;
            _selectedFragmentsTotal++;
        }
        else
            _selectedFragmentsTotal++;
        
        UpdateDisplay();
    }

    private void OnSubButtonPressed(ushort addedFragmentId)
    {
        if (addedFragmentId == _fragmentId)
        {
            _selectedFragmentsRarity--;
            _selectedFragmentsTotal--;
        }
        else
        {
            _selectedFragmentsTotal--;
        }

        UpdateDisplay();
    }

    private void OnAddMultipleButtonPressed(ushort addedFragmentId, int totalAdding)
    {
        if (addedFragmentId == _fragmentId)
        {
            _selectedFragmentsRarity += totalAdding;
            _selectedFragmentsTotal  += totalAdding;
        }
        else
        {
            _selectedFragmentsTotal += totalAdding;
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        _rarityNumberMinusSelected = _numberOfFragmentsForThatRarity - _selectedFragmentsRarity;
        _fragmentSlotsLeft         = _requiredToResearch - _selectedFragmentsTotal;

        // determine if button is clickable
        bool enableAddButtons   = _numberOfFragmentsForThatRarity > 0 && _fragmentSlotsLeft > 0;
        bool enableMinusButtons = _selectedFragmentsRarity > 0;

        SetButtonStates(enableMinusButtons, enableAddButtons);

        if (_numberOfFragmentsForThatRarity < _requiredToResearch)
        {
            if (_selectedFragmentsTotal < _requiredToResearch - _numberOfFragmentsForThatRarity)
                _maxToAdd = _numberOfFragmentsForThatRarity - _selectedFragmentsRarity;

            if (_selectedFragmentsRarity == _numberOfFragmentsForThatRarity)
            {
                _maxToAdd                     = 0;
                _plusToMaxButton.interactable = false;
                _addFragButton.interactable   = false;
                _inactiveButton               = false;
            }
            else
            {
                _maxToAdd = (int)Convert.ToUInt32(Mathf.Clamp(_numberOfFragmentsForThatRarity, 0, _fragmentSlotsLeft));
            }
        }
        else
        {
            _maxToAdd = _requiredToResearch - _selectedFragmentsTotal;
        }

        // Set text
        _numberOfFragsSelected.text = $"{_selectedFragmentsRarity}";
        _numberOfFrags.text         = $"{_rarityNumberMinusSelected}";
        _plusMultipleText.text      = $"{_maxToAdd}";

        List<EquipmentSO> equipmentSo = Resources.LoadAll<EquipmentSO>(Assets.equipmentFile).Where(x => x.Enabled).ToList();

        int totalCreatableEquipment = 0;

        foreach (EquipmentSO equipment in from equipment in equipmentSo
                                          from rarityDetails in equipment.RarityDetails
                                          where rarityDetails.Rarity == _rarity
                                          select equipment)
        {
            if (!PlayerManager.Equipment.Blueprints.GetEquipmentBlueprints(equipment.Id, out BlueprintData data))
                totalCreatableEquipment++;
            else if (!data.HasRarity(_rarity))
                totalCreatableEquipment++;
        }

        if (totalCreatableEquipment == 0 && _inactiveButton)
        {
            _subtractFragButton.interactable = false;
            _addFragButton.interactable      = false;
            _plusToMaxButton.interactable    = false;
            _inactiveButton                  = false;
        }

        // debug UI if enabled or Admin
        if (_debuggingEnabled || PlayerManager.Admin)
        {
            _parent.SetActive(true);
            _canDo.text = $"Can do: {Assets.GetEquipmentCountForThatRarity(_rarity)}";
            _have.text  = $"Crafted: {PlayerManager.Data.Equipment.Blueprints.BlueprintsAlreadyCrafted(_rarity)}";
        }
        else
            _parent.SetActive(false);
    }

    private void SetButtonStates(bool minus, bool plus)
    {
        _subtractFragButton.interactable = minus;
        _addFragButton.interactable      = plus;
        _plusToMaxButton.interactable    = plus;
    }

    private void SubButtonAction() => OnSubButtonPress?.Invoke(_fragmentId);

    private void AddButtonPressed() => OnPlusButtonPress?.Invoke(_fragmentId);

    private void AddMultipleButtonAction() => OnAddMultipleButtonPress?.Invoke(_fragmentId, _maxToAdd);
}
}