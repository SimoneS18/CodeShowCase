using Messages.Tutorial;
using Shared.Data;
using UI.Settings;
using UnityEngine;

namespace UI.Tutorial
{
public class TutorialToggleManager : MonoBehaviour
{
    [Header("Tutorial  Controller"), SerializeField]
    private int _number;

    [SerializeField]
    private UnityEngine.UI.Toggle _toggle;

    private int _mainStoryStatus;

    private PlayerData playerData;

    private void Awake()
    {
        OnMainUpdated();

        TutorialSettingsMessage.OnSettingsButtonPressed += OnMainUpdated;
        TutorialMessage.OnMainUpdated                   += OnMainUpdated;
        SettingsTutorialStatus.HideTutorial             += OnMainUpdated;
    }

    private void OnDisable()
    {
        TutorialSettingsMessage.OnSettingsButtonPressed -= OnMainUpdated;
        TutorialMessage.OnMainUpdated                   -= OnMainUpdated;
        SettingsTutorialStatus.HideTutorial             -= OnMainUpdated;
    }

    private void OnMainUpdated() =>

        //if (PlayerPrefs.GetInt("HideTutorial") == 1)
        //{
        _toggle.enabled = true;

    // foreach (Transform child in gameObject.transform) child.gameObject.SetActive(true);
    //}
    //else
    //{
    //    _mainStoryStatus = PlayerManager.TutorialData.MainTutorialStory;
    //    MainStoryStatus();
    //}        
    private void MainStoryStatus()
    {
        if (_mainStoryStatus < _number)
        {
            _toggle.enabled = false;

            foreach (Transform child in gameObject.transform)
                child.gameObject.SetActive(false);
        }
        else
        {
            _toggle.enabled = true;

            foreach (Transform child in gameObject.transform)
                child.gameObject.SetActive(true);
        }
    }
}
}