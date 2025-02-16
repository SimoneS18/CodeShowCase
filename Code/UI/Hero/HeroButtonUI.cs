using System;
using Managers;
using Messages.Hero;
using Shared.MessageCore;
using Shared.MessageData.Hero;
using Shared.Scriptables.Hero;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Hero
{
[RequireComponent(typeof(Button))]
public class HeroButtonUI : MonoBehaviour
{
    static public Action<ushort> OnHeroSelected;

    [SerializeField]
    private Image _wantedSign;

    //         [SerializeField]
    //         private Transform _purchaseCostTransform;

    //         [Header("Buttons")]
    //         [SerializeField]
    //         private Button _select;

    //         [SerializeField]
    //         private Button _unlock;

    //         [Header("Prefabs")]
    //         [SerializeField]
    //         private GameObject _resourceCost;

    private ushort _heroId;

    //         private void Awake()
    //         {
    //             _unlock.onClick.AddListener(() => OnUnlock());
    //             _select.GetComponent<Button>().onClick.AddListener(() => OnSelect());
    //         }

    private void OnEnable()
    {
        UnlockHeroMessage.OnHeroUnlocked += UpgradeHeroMessage_OnHeroUpgraded;
        SelectHeroMessage.OnHeroSelected += UpgradeHeroMessage_OnHeroUpgraded;
    }

    private void OnDisable()
    {
        UnlockHeroMessage.OnHeroUnlocked -= UpgradeHeroMessage_OnHeroUpgraded;
        SelectHeroMessage.OnHeroSelected -= UpgradeHeroMessage_OnHeroUpgraded;
    }

    internal void Init(ushort heroId)
    {
        _heroId = heroId;

        UpdateDisplay();
    }

    private void UpgradeHeroMessage_OnHeroUpgraded() => UpdateDisplay();

    private void UpdateDisplay()
    {
        Button heroInfoButton = GetComponent<Button>();

        if (!Assets.GetHero(_heroId, out HeroSO hero))
            return;

        // if do not nave hero
        if (!PlayerManager.Heroes.HasHero(_heroId))

            //                 _select.gameObject.SetActive(false);
            //                 _unlock.gameObject.SetActive(true);
            // clear cost before spawning them in
            //                 ClearCost();
            // spawn in cost to unlock
            //                 CurrencyData ScrapCost = new CurrencyData(CurrencyType.Scrap,
            //                     Shared.Utils.Values.GlobalSettings.HeroScrapCostPerLevel[0]);
            /*                Instantiate(_resourceCost, _purchaseCostTransform).GetComponent<UpgradeCostUI>().Init(ScrapCost);*/
            // get BaseShipData (HQ)
            /*                PlayerManager.Base.GetBuilding(BuildingIds.HQ, out BuildingsData hq);*/
            // check if hero can be unlocked (HeroHQRequirement)
            //                 if (hq.Positions[0].Level < Shared.Utils.Values.GlobalSettings.HeroHqRequirement[0])
            //                 {
            //                     // Hq level not high enough
            //                     _wantedSign.sprite = hero.WantedPoster;
            // /*                    _unlock.interactable = false;*/
            //                 }
            //                 else
            //                 {
            //                     // Hq level is high enough
            //                     _wantedSign.sprite = hero.WantedPoster;
            // 
            // /*                    _unlock.interactable = true;*/
            //                 }
            //                 // check if can afford to purchase hero
            //                 if (PlayerManager.Currencies[ScrapCost.Type] < ScrapCost.Amount)
            //                 {
            //                     _unlock.interactable = false;
            //                     return;
            //                 }
            /*                _unlock.interactable = true;*/
            return;

        //             _select.gameObject.SetActive(true);
        //             _unlock.gameObject.SetActive(false);

        _wantedSign.sprite          = hero.WantedPoster;
        heroInfoButton.interactable = true;

        // clear old listener
        gameObject.GetComponent<Button>().onClick.AddListener(() => SetupHeroButton(_heroId));

        // set up selected Button
        //             if (PlayerManager.Heroes.Selected == hero.Id)
        //             {
        //                 _select.gameObject.SetActive(false);
        //             }
        //             else
        //             {
        //                 _select.GetComponent<Button>().interactable = true;
        //                 _select.gameObject.SetActive(true);
        //             }
    }

    //         private void ClearCost()
    //         {
    //             foreach (Transform child in _purchaseCostTransform) Destroy(child.gameObject);
    //         }

    static private void SetupHeroButton(ushort heroId)
    {
        HeroSelectionNavigation.OnShowUI?.Invoke(heroId);
        OnHeroSelected?.Invoke(heroId);
    }

    private void OnSelect() => ClientNetworkUtils.SendServerMessage(MessageNames.SelectHero, new SelectHeroMessageData(_heroId));

    private void OnUnlock() => ClientNetworkUtils.SendServerMessage(MessageNames.UnlockHero, new UnlockHeroMessageData(_heroId));
}
}