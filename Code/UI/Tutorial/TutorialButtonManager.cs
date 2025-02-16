using Managers;
using Messages.Tutorial;
using Shared.Data;
using UI.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tutorial
{
public class TutorialButtonManager : MonoBehaviour
{
    [Header("Tutorial  Controller"), SerializeField]
    private int _number;

    [SerializeField]
    private Button _button;

    [SerializeField]
    private bool _hideButton;

    private int _mainStoryStatus;

    private PlayerData playerData;

    private void Awake()
    {
        OnMainUpdated();

        TutorialMessage.OnMainUpdated                   += OnMainUpdated;
        SettingsTutorialStatus.HideTutorial             += OnMainUpdated;
        TutorialSettingsMessage.OnSettingsButtonPressed += OnMainUpdated;
    }

    private void OnDisable()
    {
        TutorialMessage.OnMainUpdated                   -= OnMainUpdated;
        SettingsTutorialStatus.HideTutorial             -= OnMainUpdated;
        TutorialSettingsMessage.OnSettingsButtonPressed -= OnMainUpdated;
    }

    private void OnMainUpdated()
    {
        if (PlayerPrefs.GetInt("HideTutorial") == 1)
        {
            if (_button != null)
            {
                gameObject.SetActive(!_hideButton);
                _button.interactable = !_hideButton;
            }
            else
            {
                foreach (Transform child in gameObject.transform)
                    child.gameObject.SetActive(true);
            }
        }
        else
        {
            _mainStoryStatus = PlayerManager.TutorialData.MainTutorialStory;
            MainStoryStatus();
        }
    }

    private void MainStoryStatus()
    {
        if (_mainStoryStatus < _number)
        {
            if (_button != null)
            {
                if (_hideButton)
                {
                    gameObject.SetActive(false);
                    _button.interactable = false;
                }
                else
                {
                    gameObject.SetActive(true);
                    _button.interactable = true;
                }
            }
            else
            {
                foreach (Transform child in gameObject.transform)
                    child.gameObject.SetActive(false);
            }
        }
        else
        {
            if (_button != null)
            {
                gameObject.SetActive(true);
                _button.interactable = true;
            }
            else
            {
                foreach (Transform child in gameObject.transform)
                    child.gameObject.SetActive(true);
            }
        }
    }
}
}