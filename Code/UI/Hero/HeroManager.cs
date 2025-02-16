using System;
using Managers;
using Shared.MessageCore;
using Shared.MessageData.Hero;
using Shared.Scriptables.Hero;
using Shared.Utils.Values;
using UnityEngine;
using Utils;

namespace UI.Hero
{
public class HeroManager : MonoBehaviour
{
    static public Action<HeroSO> OnSelectHero;
    static public Action         OnUnlockHeroSelection;

    static public HeroManager instance;

    private HeroSO _currentHero;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        OnSelectHero          += HeroManager_OnSelectHero;
        OnUnlockHeroSelection += HeroManager_OnUnlockHeroSelection;
    }

    private void OnDestroy()
    {
        OnSelectHero          -= HeroManager_OnSelectHero;
        OnUnlockHeroSelection -= HeroManager_OnUnlockHeroSelection;
    }

    private void HeroManager_OnSelectHero(HeroSO hero)
    {
        _currentHero = hero;

        if (hero == null)
            return;
    }

    private void HeroManager_OnUnlockHeroSelection()
    {
        // check if can afford (allow or send out error message)
        /*foreach (CurrencyData currency in _currentHero.Levels[0].Cost)
    {
        if (playerData.Currencies[currency.Type] < currency.Amount)
        {
            ClientPlayerManager.OnDisplayError?.Invoke(ToastMessages.CannotAfford);
            return;
        }

        Debug.Log($"Can Afford Hero unlocking/ungrading. {currency.Type}: {currency.Amount}");
    }*/

        // check if max hero's is reached - TODO
        //if(playerData.Heroes.GetHeroCount(_currentHero.Id) >= _currentHero.GetMaxHero())
        if (PlayerManager.Heroes.HasHero(_currentHero.Id))
        {
            PlayerManager.OnDisplayError?.Invoke(ToastMessages.HeroAlreadyUnlocked);
            return;
        }

        Debug.Log($"Has not unlocked {_currentHero.Id}");

        // TEMPARAY - For testing purpose's
        PlayerManager.OnDisplayError?.Invoke(ToastMessages.HeroUnlocked);

        //Debug.Log($"GetHeroCount: {playerData.Heroes.GetHeroCount(_currentHero.Id)}");
        //Debug.Log($"GetMaxHero{_currentHero.GetMaxHero()}");

        // TO DO - maybe add hero to building queue

        ClientNetworkUtils.SendServerMessage(MessageNames.UnlockHero, new UnlockHeroMessageData(_currentHero.Id));
    }
}
}