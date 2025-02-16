using System.Collections.Generic;
using System.Linq;
using Managers;
using Messages.BaseBuilding;
using Messages.BaseBuilding.CardCrafting;
using Messages.Cheats;
using Shared.Data;
using Shared.Data.Equipment;
using Shared.Data.PlayerBase.BuildingData;
using Shared.Enums;
using Shared.MessageCore;
using Shared.MessageData.BaseBuilding.CardCrafting;
using Shared.Scriptables.Equipment;
using Shared.Structs;
using Shared.Utils.Values;
using Sirenix.OdinInspector;
using TMPro;
using UI.BaseBuilding;
using UI.BaseBuilding.EquipmentCrafting;
using UI.Stats;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Equipment
{
public class UiCraftSelectionEquipment : MonoBehaviour
{
    [Title("Appearance")] 
    [SerializeField, Required] private TMP_Text            _name;
    [SerializeField, Required] private TMP_Text            _setName;
    [SerializeField, Required] private TMP_Text            _errorText;
    [SerializeField, Required] private Image               _setIcon;
    [SerializeField, Required] private Image               _equipmentImage;
    [SerializeField, Required] private Button              _forgeButton;

    [Title("Spawning")] 
    [SerializeField, Required] private Transform _statSpawnPoint;
    [SerializeField, Required] private Transform _costSpawnPoint;

    [Title("Prefabs")] 
    [SerializeField, Required] private GameObject _statPrefab;
    [SerializeField, Required] private GameObject _itemCost;
    [SerializeField, Required] private GameObject _currencyPrefab;
    [SerializeField, Required] private Transform  _currencyParent;

    [Title("Time")]
    [SerializeField, Required] private TMP_Text _timeToUpgrade;
    
    [Title("Queue Panel")]
    [SerializeField, Required] private TMP_Text _queueSlotText;
    [SerializeField, Required] private GameObject _queueItemPrefab;
    [SerializeField, Required] private Transform  _queueParent;
    
    private ushort      _buildingId;
    private int         _buildingIndex;
    private ushort      _equipmentId;
    private ushort      _rarityIndex;
    private float       _finishTime;
    private bool        _inactiveButton;
    private EquipmentSO _equipmentSo;
    private Rarity      _rarity;

    private void OnEnable()
    {
        _inactiveButton = true;
        
        BuildingCraftAddMessage.OnCraftAddWorkshop           += BuildingCraftAddMessage_OnCraftAddWorkshop;
        BuildingCraftCompleteMessage.OnCraftCompleteWorkshop += BuildingCraftCompleteMessage_OnCraftCompleteWorkshop;
        BuildingCraftCancelMessage.OnCraftCancelWorkshop     += BuildingCraftCancelMessage_OnEquipmentCraftingCancelled;
        UiEquipmentCrafting.OnWorkshopFirstOpenUpdateDisplay += UiEquipmentCrafting_OnWorkshopFirstOpenUpdateDisplay;
        UiEquipmentButton.OnTogglePressed                    += UiEquipmentButton_OnEquipmentTogglePressed;
        AddSubMaterialMessage.OnMaterialAddSub               += AddSubMaterialMessage_OnMaterialAddSub;
        UpdateCurrencyMessage.OnCurrencyUpdated              += UpdateCurrencyMessage_OnCurrencyUpdated;
        
        //EquipmentsData.OnStaticEquipmentAdded += EquipmentData_OnEquipmentAdded;
    }

    private void OnDisable()
    {
        BuildingCraftAddMessage.OnCraftAddWorkshop           -= BuildingCraftAddMessage_OnCraftAddWorkshop;
        BuildingCraftCompleteMessage.OnCraftCompleteWorkshop -= BuildingCraftCompleteMessage_OnCraftCompleteWorkshop;
        BuildingCraftCancelMessage.OnCraftCancelWorkshop     -= BuildingCraftCancelMessage_OnEquipmentCraftingCancelled;
        UiEquipmentCrafting.OnWorkshopFirstOpenUpdateDisplay -= UiEquipmentCrafting_OnWorkshopFirstOpenUpdateDisplay;
        UiEquipmentButton.OnTogglePressed           -= UiEquipmentButton_OnEquipmentTogglePressed;
        AddSubMaterialMessage.OnMaterialAddSub               -= AddSubMaterialMessage_OnMaterialAddSub;
        UpdateCurrencyMessage.OnCurrencyUpdated              -= UpdateCurrencyMessage_OnCurrencyUpdated;
        
       // EquipmentsData.OnStaticEquipmentAdded -= EquipmentData_OnEquipmentAdded;
    }
    
    /// <summary>
    /// When the Workshop is first opened, gets the building id and building Index
    /// </summary>
    /// <param name="buildingId"> Buildings ID </param>
    /// <param name="index"> Index of the building </param>
    private void UiEquipmentCrafting_OnWorkshopFirstOpenUpdateDisplay(ushort buildingId, int index)
    {
        _buildingId = buildingId;
        _buildingIndex      = index;

        // make sure everything is off the craft the equipment button.
        // Assign function to button
        _forgeButton.onClick.RemoveAllListeners();
        _forgeButton.onClick.AddListener(OnForgeButtonPressed);
    }
    
    /// <summary>
    /// When the button is pressed, this handles the next steps of the logic
    /// </summary>
    private void OnForgeButtonPressed()
    {
        // get time when button was pressed
        double time = _finishTime + Shared.Utils.Utils.GetTimeValue();

        // send message to server 
        ClientNetworkUtils.SendServerMessage(MessageNames.BuildingCraftAdd,
                                             new BuildingCraftAddMessageData(_buildingId, (byte)_buildingIndex, _equipmentId,
                                                                             0, new ushort[1], _rarityIndex, time));
    }
    
    /// <summary>
    /// This is executed when the toggle of a equipment is pressed.
    /// </summary>
    /// <param name="equipmentId"> Equipment ID selected</param>
    /// <param name="rarity"> Rarity of the selected equipment </param>
    private void UiEquipmentButton_OnEquipmentTogglePressed(ushort equipmentId, Rarity rarity)
    {
        if (!Assets.GetEquipment(equipmentId, out EquipmentSO equipmentSo))
            return;

        _equipmentId    = equipmentId;
        _rarity         = rarity;
        _equipmentSo    = equipmentSo;

        UpdateDisplay();
    }

    /// <summary>
    /// When material is added/subtracted from player, update Display
    /// </summary>
    private void AddSubMaterialMessage_OnMaterialAddSub() => DetermineIfInPlayerDBandUpdateButtonsDisplay();

    /// <summary>
    /// When currency is added/subtracted from player, update Display
    /// </summary>
    private void UpdateCurrencyMessage_OnCurrencyUpdated() => DetermineIfInPlayerDBandUpdateButtonsDisplay();

    /// <summary>
    /// When equipment crafting is CANCELLED, update the Queue Display
    /// </summary>
    private void BuildingCraftCancelMessage_OnEquipmentCraftingCancelled()
    {
        _inactiveButton           = true;
        _forgeButton.interactable = true;
        _errorText.text           = "";

        UpdateQueueDisplay();
    }

    /// <summary>
    /// When equipment Craft is complete, display a toast and update Queue Display
    /// </summary>
    private void BuildingCraftCompleteMessage_OnCraftCompleteWorkshop()
    {
        // reset button
        _inactiveButton = true;

        // #TODO Display Toast
        // PlayerManager.OnDisplayError(ToastMessages.EquipmentCrafted(_equipmentSo.Name));

        UpdateDisplay();
    }

    private void EquipmentData_OnEquipmentAdded(ushort id)
    {
        // get all equipment
        List<EquipmentSO> equipmentSos = Resources.LoadAll<EquipmentSO>(Assets.equipmentFile).Where(x => x.Enabled).ToList();
        
        Debug.Log($"Added: {id} new Count: {equipmentSos.Count}");
    }

    /// <summary>
    /// Check if the currently selected equipment and rarity is in the player DB, if so update the buttons
    /// </summary>
    private void DetermineIfInPlayerDBandUpdateButtonsDisplay()
    {
        // Do we have the equipment blueprint saved in the players DB?
        if (PlayerManager.Equipment.Blueprints.GetEquipmentBlueprints(_equipmentId, out BlueprintData blueprintData))
            EquipmentIdInDB(blueprintData);
        else
        {
            // if button is already false, return
            if (!_inactiveButton)
                return;

            UpdateButtonStatus(ToastMessages.NoBlueprint);
        }
    }

    /// <summary>
    /// Check if we have the blueprint for this equipment and Rarity unlocked 
    /// </summary>
    /// <param name="blueprintData"> the equipment blueprint data in the players DB</param>
    private void EquipmentIdInDB(BlueprintData blueprintData)
    {
        // Do we have it in the selected Rarity?
        if (blueprintData.HasRarity(_rarity))
        {
            bool currency = UpdateCurrency_CheckIfCanAfford();

            // Do we have enough currency ? & Active button
            if (!currency && _inactiveButton)
            {
                Debug.LogError($"Can not afford!  currency: {currency} button: {_inactiveButton}");
                UpdateButtonStatus(ToastMessages.CannotAfford);
            }
            else
                Debug.Log("Can AFFORD!");
        }

        // Don't have of this rarity
        else
            UpdateButtonStatus(ToastMessages.NoBlueprint);
    }
    
    /// <summary>
    /// Update Main UI on Craft Added
    /// </summary>
    private void BuildingCraftAddMessage_OnCraftAddWorkshop() => UpdateDisplay();

    /// <summary>
    /// The main place the overall UI is updated
    /// This updates the currency, stats and material costs of the equipment.
    /// Also updates where the player can craft the selected equipment and any other information.
    /// </summary>
    private void UpdateDisplay()
    {
        // clear spawned Prefabs
        foreach (Transform child in _statSpawnPoint)
            Destroy(child.gameObject);

        foreach (Transform child in _costSpawnPoint)
            Destroy(child.gameObject);

        foreach (Transform child in _currencyParent)
            Destroy(child.gameObject);

        // Update Equipment General UI Display
        _forgeButton.interactable = true;
        _errorText.text           = "";

        // Update Visuals of the equipment Details on the right
        _name.text             = _equipmentSo.Name;
        _equipmentImage.sprite = _equipmentSo.Sprite;

        DetermineIfInPlayerDBandUpdateButtonsDisplay();

        // Update Display Stats Material Currency Time
        _inactiveButton = true;

        if (_equipmentSo.RarityDetails == null)
            return;

        for (int i = 0; i < _equipmentSo.RarityDetails.Length; i++)
        {
            if (_equipmentSo.RarityDetails[i].Rarity != _rarity)
                continue;

            _finishTime  = _equipmentSo.RarityDetails[i].CraftTime.Value;
            _rarityIndex = (ushort)i;

            // display stats
            foreach (EquipmentStat stat in _equipmentSo.RarityDetails[i].DefaultStats)
                Instantiate(_statPrefab, _statSpawnPoint).GetComponent<UIStatsText>().
                                                          Init(stat);

            // display Material Cost
            foreach (MaterialCost materialCost in _equipmentSo.RarityDetails[i].Items)

                Instantiate(_itemCost, _costSpawnPoint).GetComponent<UiItemCost>().
                                                        Init(materialCost.Id, materialCost.Amount);

            // display Currency
            foreach (CurrencyData cost in _equipmentSo.RarityDetails[i].Cost)
                Instantiate(_currencyPrefab, _currencyParent).GetComponent<UpgradeCostUI>().
                                                              Init(cost);

            // display time to craft
            _timeToUpgrade.text = Shared.Utils.Utils.GetTimeString(_equipmentSo.RarityDetails[i].CraftTime.Value);
        }

        UpdateQueueDisplay();
    }

    /// <summary>
    /// Checks if the player can afford the current craft currency Wise
    /// </summary>
    /// <returns></returns>
    private bool UpdateCurrency_CheckIfCanAfford()
    {
        foreach (EquipmentRarityDetails rarityDetails in _equipmentSo.RarityDetails)
        {
            if (rarityDetails.Rarity != _rarity)
                continue;

            // check currency
            foreach (CurrencyData currency in rarityDetails.Cost)
                if (PlayerManager.Currencies[currency.Type] < currency.Amount)
                    return false;

            // check material
            foreach (MaterialCost materialCost in rarityDetails.Items)
                if (!PlayerManager.Items.HasItemAmount(materialCost.Id, materialCost.Amount))
                    return false;
        }

        return true;
    }

    /// <summary>
    /// Updates ONLY the Queue display.
    /// This includes the number in the queue and the toggles in the queue.
    /// Each item in the queue has its own button.
    /// </summary>
    private void UpdateQueueDisplay()
    {
        int inQueue         = 0;
        int queueTotalCount = 0; 
        
        foreach (Transform child in _queueParent)
            Destroy(child.gameObject);

        // Get Workshop count
        if (!PlayerManager.Base.GetBuilding(_buildingId, out BuildingsData placeableData))
        {
            Debug.LogError("Could not find Building data", gameObject);
            return;
        }

        queueTotalCount = placeableData.Positions.Count;
        
        for (int i = 0; i < queueTotalCount; i++)
        {
            CraftBuildingData buildingData = (CraftBuildingData)placeableData.Positions[i];

            if (buildingData.CurrentCraft == 0)
                continue;

            inQueue++;

            Instantiate(_queueItemPrefab, _queueParent).GetComponent<EquipmentCraftingSlot>()
                                                       .Init(i, _buildingId, _buildingIndex, 
                                                             (CraftBuildingData)placeableData.Positions[i]);
        }

        _queueSlotText.text = $"{inQueue}/{queueTotalCount}";

        if (inQueue == queueTotalCount && _inactiveButton)
            UpdateButtonStatus(ToastMessages.QueueFull);
    }
    
    /// <summary>
    /// Update the research button, text and check to false
    /// </summary>
    /// <param name="errorMessage"> What is the cause of the button be disabled? </param>
    private void UpdateButtonStatus(string errorMessage)
    {
        _forgeButton.interactable = false;
        _errorText.text           = errorMessage;
        _inactiveButton           = false;
    }
}
}