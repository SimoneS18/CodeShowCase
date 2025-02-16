using System;
using System.Collections.Generic;
using Managers;
using Messages.BaseBuilding;
using Messages.BaseBuilding.CardCrafting;
using Messages.BaseBuilding.Update;
using Shared.Data.PlayerBase.BuildingData;
using Shared.Enums;
using Shared.MessageCore;
using Shared.MessageData.BaseBuilding.CardCrafting;
using Shared.Scriptables.Buildings;
using Shared.Scriptables.Equipment;
using Shared.Scriptables.Items;
using Shared.Utils;
using Shared.Utils.Identifiers;
using TMPro;
using UI.BaseBuilding.EquipmentCrafting;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Equipment
{
public class UiCraftSelectionBlueprint : MonoBehaviour
{
    static public Action OnResetButtonPressed;

    [Header("Appearance")]
    [SerializeField] private TMP_Text _headerText;
    [SerializeField] private TMP_Text _fragsAdded;
    [SerializeField] private Button   _researchButton;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private Button   _resetButton;

    [Header("Spawning")]
    [SerializeField] private GameObject _fragCardSpawnpoint;

    [Header("Prefabs")]
    [SerializeField] private GameObject _fragCardPrefab;

    [Header("Queue Panel")]
    [SerializeField] private TMP_Text _queueSlotText;
    [SerializeField] private GameObject _queueItemPrefab;
    [SerializeField] private Transform  _queueParent;

    [Header("Collected Blueprint UI")]
    [SerializeField] private GameObject _blackBackground;
    [SerializeField] private GameObject _collectedBlueprintUI;
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private TMP_Text   _equipmentName;
    [SerializeField] private Image      _equipmentImage;
    [SerializeField] private Image      _eqipmentRarity;

    [Header("Debugging")]
    [SerializeField] private bool _debubggingEnabled;
    [SerializeField] private GameObject _parent;
    [SerializeField] private TMP_Text   _activeButton;
    
    private          bool              _inactiveButton;
    private          bool              _blueprintCollected;
    private          int               _buildingIndex;
    private          int               _selectedFragments;
    private          ushort            _buildingId;
    private readonly List<int>         _index = new List<int>();
    private          List<FragmentsSO> _selectedFrags;
    private          List<ushort>      _strFrags;
    private          PlaceableSO       _placeableSo;

    private void OnEnable()
    {
        _inactiveButton     = true;
        _blueprintCollected = false;
        _selectedFrags      = new List<FragmentsSO>();
        _strFrags           = new List<ushort>();

        UiBlueprintCrafting.OnFirstOpenUpdateDisplay           += UIBlueprintCrafting_OnFirstOpenUpdateDisplay;
        UiFragmentSelection.OnPlusButtonPress                  += UiFragmentSelection_OnPlusButtonPressed;
        UiFragmentSelection.OnSubButtonPress                   += UiFragmentSelection_OnSubButtonPressed;
        UiFragmentSelection.OnAddMultipleButtonPress           += UiFragmentSelection_OnAddMultipleButtonPressed;
        BuildingCraftCancelMessage.OnCraftCancelLaboratory     += BuildingCraftCancelMessage_OnCraftCancelLaboratory;
        BuildingCraftAddMessage.OnCraftAddLaboratory           += BlueprintCraftAddMessage_OnCraftAddLaboratory;
        BuildingCraftCompleteMessage.OnCraftCompleteLaboratory += BuildingCraftCompleteMessage_OnCraftCompleteLaboratory;
        UpdateBlueprintMessage.OnBlueprintUpdated              += UpdateBlueprintMessage_OnBlueprintUpdated;
    }

    private void OnDisable()
    {
        UiBlueprintCrafting.OnFirstOpenUpdateDisplay           -= UIBlueprintCrafting_OnFirstOpenUpdateDisplay;
        UiFragmentSelection.OnPlusButtonPress                  -= UiFragmentSelection_OnPlusButtonPressed;
        UiFragmentSelection.OnSubButtonPress                   -= UiFragmentSelection_OnSubButtonPressed;
        UiFragmentSelection.OnAddMultipleButtonPress           -= UiFragmentSelection_OnAddMultipleButtonPressed;
        BuildingCraftCancelMessage.OnCraftCancelLaboratory     -= BuildingCraftCancelMessage_OnCraftCancelLaboratory;
        BuildingCraftAddMessage.OnCraftAddLaboratory           -= BlueprintCraftAddMessage_OnCraftAddLaboratory;
        BuildingCraftCompleteMessage.OnCraftCompleteLaboratory -= BuildingCraftCompleteMessage_OnCraftCompleteLaboratory;
        UpdateBlueprintMessage.OnBlueprintUpdated              -= UpdateBlueprintMessage_OnBlueprintUpdated;
    }

    /// <summary>
    /// Executed when the Workshop is first Opened
    /// </summary>
    /// <param name="buildingId"></param>
    /// <param name="buildingIndex"></param>
    private void UIBlueprintCrafting_OnFirstOpenUpdateDisplay(ushort buildingId, int buildingIndex)
    {
        _buildingId = buildingId;

        if (!Assets.GetBuilding(buildingId, out PlaceableSO placeable))
        {
            Debug.LogError("Couldn't get building");
            return;
        }

        _placeableSo   = placeable;
        _buildingIndex = buildingIndex;

        _researchButton.onClick.RemoveAllListeners();
        _researchButton.GetComponent<Button>().onClick.AddListener(OnResearchPressed);
        _resetButton.onClick.RemoveAllListeners();
        _resetButton.GetComponent<Button>().onClick.AddListener(OnResetButtonPress);
        _closeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _closeButton.GetComponent<Button>().onClick.AddListener(OnCloseButtonPressed);

        UpdateDisplay();
        UpdateQueueDisplay();
    }

    private void UiFragmentSelection_OnPlusButtonPressed(ushort fragmentId)
    {
        if (!Assets.GetItem(fragmentId, out FragmentsSO fragment))
            return;

        SpawnVisualFragmentAndIncreaseData(fragment);
        UpdateDisplay();
    }

    private void UiFragmentSelection_OnSubButtonPressed(ushort fragmentId)
    {
        if (!Assets.GetItem(fragmentId, out FragmentsSO fragment))
            return;

        _index.Clear();

        for (int i = 0; i < _selectedFrags.Count; i++)
        {
            if (_selectedFrags[i] == fragment)
                _index.Add(i);
        }

        Destroy(_fragCardSpawnpoint.GetComponent<Transform>().GetChild(_index[_index.Count - 1]).gameObject);
        _selectedFrags.RemoveAt(_index[_index.Count - 1]);

        UpdateDisplay();
    }

    private void UiFragmentSelection_OnAddMultipleButtonPressed(ushort fragmentId, int numberAdding)
    {
        if (!Assets.GetItem(fragmentId, out FragmentsSO fragment))
            return;

        for (int i = 0; i < numberAdding; i++)
            SpawnVisualFragmentAndIncreaseData(fragment);

        UpdateDisplay();
    }

    private void BuildingCraftCancelMessage_OnCraftCancelLaboratory()
    {
        ClearOldData();
        UpdateDisplay();
        UpdateQueueDisplay();
    }

    private void BlueprintCraftAddMessage_OnCraftAddLaboratory()
    {
        _selectedFrags.Clear();

        ClearOldData();
        UpdateDisplay();
        UpdateQueueDisplay();
    }

    private void BuildingCraftCompleteMessage_OnCraftCompleteLaboratory()
    {
        _blackBackground.SetActive(true);
        _collectedBlueprintUI.SetActive(true);

        _blueprintCollected = true;
    }

    private void UpdateBlueprintMessage_OnBlueprintUpdated(ushort id, Rarity rarity)
    {
        if (!_blueprintCollected)
            return;

        Assets.GetEquipment(id, out EquipmentSO equipmentSo);

        // set Display
        _equipmentImage.sprite = equipmentSo.Sprite;
        _eqipmentRarity.color  = Colours.RarityColours[rarity];
        _equipmentName.text    = $"{equipmentSo.Name}";

        // Enable Display
        _equipmentImage.gameObject.SetActive(true);
        _eqipmentRarity.gameObject.SetActive(true);
        _equipmentName.gameObject.SetActive(true);
        _closeButton.gameObject.SetActive(true);

        UpdateDisplay();
        UpdateQueueDisplay();
    }

    private void SpawnVisualFragmentAndIncreaseData(FragmentsSO fragment)
    {
        GameObject fragCard = Instantiate(_fragCardPrefab, _fragCardSpawnpoint.GetComponent<Transform>());
        fragCard.GetComponent<UiItemCost>().FragmentSetup(fragment);
        _selectedFrags.Add(fragment);
    }

    private void UpdateDisplay()
    {
        _headerText.text = _placeableSo.Name;

        // Get and display total selected frags
        _selectedFragments = _selectedFrags.Count;
        _fragsAdded.text   = $"{_selectedFragments}/5";

        _errorText.text = "";

        //If all frags are selected

        if (_selectedFragments > 0)
        {
            _resetButton.GetComponent<Button>().interactable = true;
            _activeButton.text                               = "_selectedFragments > 0";
        }

        if (_selectedFragments == 5 && _inactiveButton)
        {
            _researchButton.GetComponent<Button>().interactable = true;
            _resetButton.GetComponent<Button>().interactable    = true;
            _inactiveButton                                     = false;
            _activeButton.text                                  = "_selectedFragments == 5 && _inactiveButton";
        }
        else if (_selectedFragments == 0)
        {
            _resetButton.GetComponent<Button>().interactable = false;
            _activeButton.text                               = "_selectedFragments == 0";
        }
        else if (_selectedFragments != 0 && _selectedFragments < 5 && !_inactiveButton)
        {
            _researchButton.GetComponent<Button>().interactable = false;
            _resetButton.GetComponent<Button>().interactable    = true;
            _inactiveButton                                     = true;
            _activeButton.text                                  = "_selectedFragments != 0 && _selectedFragments < 5 && !_inactiveButton";
        }

        // debug UI if enabled or Admin
        if (_debubggingEnabled || PlayerManager.Admin)
            _parent.SetActive(true);
        else
            _parent.SetActive(false);
    }

    private void ClearOldData()
    {
        foreach (Transform child in _fragCardSpawnpoint.GetComponent<Transform>())
            Destroy(child.gameObject);
    }

    private void OnResearchPressed()
    {
        double craftTime = 0;
        _strFrags.Clear();

        foreach (FragmentsSO frag in _selectedFrags)
        {
            if (!Assets.GetItem(frag.Id, out FragmentsSO fragment))
                return;

            double fragmentTime = fragment.CraftTime;
            craftTime = craftTime + fragmentTime;
            _strFrags.Add(frag.Id);
        }

        double finishTime = Shared.Utils.Utils.GetTimeValue() + craftTime;

        // new ? get working 
        ClientNetworkUtils.SendServerMessage(MessageNames.BuildingCraftAdd,
                                             new BuildingCraftAddMessageData(_buildingId, (byte)_buildingIndex, 0, 0, _strFrags.ToArray(),
                                                                             10, finishTime));

        _inactiveButton = true;
    }

    private void OnResetButtonPress()
    {
        _selectedFrags.Clear();
        ClearOldData();
        UpdateDisplay();

        _researchButton.GetComponent<Button>().interactable = false;
        OnResetButtonPressed?.Invoke();
    }

    private void OnCloseButtonPressed() => _blueprintCollected = false;

    private void UpdateQueueDisplay()
    {
        // clear queue display
        foreach (Transform child in _queueParent)
            Destroy(child.gameObject);

        ushort id = BuildingIds.Laboratory;

        PlayerManager.Base.GetBuilding(id, out BuildingsData laboratoryData);

        int queueMaxCount = laboratoryData.Positions.Count;
        int inQueueCount  = 0;

        if (Assets.GetQueueCount(id, laboratoryData) > 0)
            for (int i = 0; i < laboratoryData.Positions.Count; i++)
            {
                CraftBuildingData craftBuildingData = (CraftBuildingData)laboratoryData.Positions[i];

                if (craftBuildingData == null)
                    throw new ArgumentNullException(nameof(craftBuildingData));

                if (craftBuildingData.Fragments == null)
                    continue;

                Debug.Log($"Stored Length: {craftBuildingData.Fragments.Length}");

                if (craftBuildingData.Fragments.Length == 5)
                    inQueueCount = inQueueCount + 1;

                // Spawn UI
                GameObject obj = Instantiate(_queueItemPrefab, _queueParent);
                obj.GetComponent<BlueprintCraftingSlot>().Init(inQueueCount, _buildingId, _buildingIndex, craftBuildingData);
            }

        // update Text
        _queueSlotText.text = $"{inQueueCount}/{queueMaxCount}";

        // if full, update display and lock buttons if needed
        if (inQueueCount == queueMaxCount)
        {
            _errorText.text              = "Queue Full";
            _researchButton.interactable = false;
            _inactiveButton              = false;
        }
    }
}
}