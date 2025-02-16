using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Messages.BaseBuilding.Queue;
using Messages.Tutorial;
using Shared.Class;
using Shared.Data.PlayerBase.BuildingData;
using Shared.Data.Quests;
using Shared.Data.Queue;
using Shared.Enums;
using Shared.Enums.Buildings;
using Shared.MessageCore;
using Shared.MessageData.BaseBuilding.Quest;
using Shared.MessageData.Tutorial;
using Shared.Scriptables.Buildings;
using Shared.Scriptables.Hero;
using Shared.Scriptables.Quests;
using Shared.Scriptables.Tutorial;
using Shared.Structs;
using UnityEngine;
using Utils;
public class TutorialMain : MonoBehaviour
{
    // ushort being Quest Id
    static public Action<ushort> OnHeroGuideSelected;
    static public Action         NextMainTutorialStep;
    static public Action<bool>   SkipTutorial; // from the confirmation panel ( true - skip all, false - skip current)
    static public Action         OnTutorialAim;

    [Header("Pooling Setup"), SerializeField]
    private TutorialPoolObject _objects;

    [SerializeField]
    private Jun_TweenRuntime _cameraManager;

    [SerializeField]
    private Camera _camera;

    [Header("Spawning Targeting"), SerializeField]
    private GameObject _targetObject;

    [SerializeField]
    private GameObject _target3DReference;

    [Header("Tutorial Setup"), SerializeField]
    private List<Tutorials> _mainTutorial;

    private float  _arrowDistance       = 20;
    private float  _arrowMovingDistance = 30;
    private HeroSO _heroSO;

    private int        _mainStoryStatus;
    private GameObject _spawnedObject;

    private TutorialStep _tutorial;
    private int          steps;

    private void Awake()
    {
        _mainStoryStatus = PlayerManager.TutorialData.MainTutorialStory;
        steps            = 0;

        // check to see if tutorials all skipped
        if (_mainStoryStatus == 100)
        {
            Debug.Log("Main Tutorial Skipped");
            return;
        }

        // check the main Tutorials Requirement
        if (steps == 0)
            if (_mainTutorial[_mainStoryStatus].AddionalRequirnment.Length > 0)
                foreach (AddionalRequirnments requi in _mainTutorial[_mainStoryStatus].AddionalRequirnment)
                {
                    if (!QuestRequirnmentChecking(requi))
                        return;
                }

        _tutorial = _mainTutorial[_mainStoryStatus].TutorialStep[steps];

        // Check the additional Requirements on Step
        foreach (AddionalRequirnments requi in _tutorial.AdditionalRequirnment)
        {
            if (!QuestRequirnmentChecking(requi))
                return;
        }

        UpdateDisplay();

        NextMainTutorialStep += NextStepUpdate;
        SkipTutorial         += SkipTutorialClicked;
        OnHeroGuideSelected  += HeroGuidePressed;

        TutorialMessage.OnMainUpdated                      += TutorialCheck_OnQuestAdded;
        TutorialSettingsMessage.OnSettingsButtonPressed    += TutorialCheck_OnQuestAdded;
        PlayerQueue.OnQuestAdded                           += TutorialCheck_OnQuestAdded;
        QueueCompletionMessage.OnCompleteQueueItemTutorail += TutorialCheck_OnQuestAdded;
    }

    private void OnDestroy()
    {
        NextMainTutorialStep                            -= NextStepUpdate;
        TutorialSettingsMessage.OnSettingsButtonPressed -= TutorialCheck_OnQuestAdded;
        SkipTutorial                                    -= SkipTutorialClicked;
        OnHeroGuideSelected                             -= HeroGuidePressed;

        TutorialMessage.OnMainUpdated                      -= TutorialCheck_OnQuestAdded;
        PlayerQueue.OnQuestAdded                           -= TutorialCheck_OnQuestAdded;
        QueueCompletionMessage.OnCompleteQueueItemTutorail -= TutorialCheck_OnQuestAdded;
    }

    private void TutorialCheck_OnQuestAdded()
    {
        if (PlayerManager.TutorialData.MainTutorialStory == 100)
            return;

        if (_mainStoryStatus != PlayerManager.TutorialData.MainTutorialStory)
        {
            if (_mainTutorial[_mainStoryStatus].TutorialStep.Count == steps + 1)
                switch (_mainTutorial[_mainStoryStatus].QuestReward.Type)
                {
                    case RequirnmentType.None:
                        // do nothing
                        break;
                    case RequirnmentType.Quest:
                        Debug.Log("Complete Quest");
                        Assets.GetQuest(_mainTutorial[_mainStoryStatus].QuestReward.Id, out QuestChainSO questChainSO);
                        ClientNetworkUtils.SendServerMessage(MessageNames.CompleteQuest, new CompleteQuestMessageData(questChainSO.Id));
                        break;
                }

            steps            = 0;
            _mainStoryStatus = PlayerManager.TutorialData.MainTutorialStory;
            _tutorial        = _mainTutorial[_mainStoryStatus].TutorialStep[steps];
        }

        if (steps == 0)
            foreach (AddionalRequirnments requi in _mainTutorial[_mainStoryStatus].AddionalRequirnment)
            {
                if (!QuestRequirnmentChecking(requi))
                    return;
            }

        foreach (AddionalRequirnments requi in _tutorial.AdditionalRequirnment)
        {
            if (!QuestRequirnmentChecking(requi))
                return;
        }

        NextStepUpdate();
    }

    private bool QuestRequirnmentChecking(AddionalRequirnments requi)
    {
        switch (requi.Type)
        {
            case RequirnmentType.Quest:
                PlayerManager.Quests.GetQuest(requi.Id, out QuestsProgressData questsProgressData);
                int currentQuestIndex = questsProgressData.CurrentStepIndex;

                if (currentQuestIndex < requi.Value)
                    return false;

                break;
            case RequirnmentType.BuildingInQueue:
                int queueCount = PlayerManager.Queue.GetQueueCount(QueueType.Building, requi.Id);

                if (queueCount < requi.Value)
                    return false;

                break;
            case RequirnmentType.TotalBuildings:
                Assets.GetBuilding(requi.Id, out PlaceableSO totalplaceableSO);

                if (totalplaceableSO.BuildType == BuildType.Cosmetic)
                    return false;

                if (totalplaceableSO.BuildType == BuildType.BaseShip)
                {
                    if (!PlayerManager.Base.GetBuilding(requi.Id, out BuildingsData baseShipsData))
                        return false;

                    int buildingCount = baseShipsData.Positions.Where(x => !x.InQueue).Count();

                    if (buildingCount < requi.Value)
                        return false;
                }
                else
                {
                    if (!PlayerManager.Base.GetBuilding(requi.Id, out BuildingsData buildingsData))
                        return false;

                    int buildingCount = buildingsData.Positions.Where(x => !x.InQueue).Count();

                    if (buildingCount < requi.Value)
                        return false;
                }

                break;
            case RequirnmentType.TotoalBuildingsIncludingQueue:
                Assets.GetBuilding(requi.Id, out PlaceableSO everythingTotalplaceableSO);

                if (everythingTotalplaceableSO.BuildType == BuildType.Cosmetic)
                {
                    return false;
                }

                if (everythingTotalplaceableSO.BuildType == BuildType.BaseShip)
                {
                    int baseCount = PlayerManager.Base.GetBuildingCount(requi.Id);

                    if (baseCount < requi.Value)
                        return false;
                }
                else
                {
                    int buildingCount = PlayerManager.Base.GetBuildingCount(requi.Id);

                    if (buildingCount < requi.Value)
                        return false;
                }

                break;
        }

        return true;
    }

    private void NextStepUpdate()
    {
        int count = _mainTutorial[_mainStoryStatus].TutorialStep.Count;

        Debug.Log($"Next Step Update. Count: {count}");

        if (steps + 1 != count)
        {
            if (_tutorial.Toggle != null)
            {
                //                 if (steps != (_mainTutorial[_mainStoryStatus].TutorialStep.Count - 1))
                //                 {
                //                     if (_tutorial.ToggleGroup == null)
                //                     {
                //                         _tutorial.ToggleSetup.Tog.onValueChanged.Invoke(true);
                //                         _tutorial.ToggleSetup.Tog.gameObject.SetActive(!_tutorial.DisableWhenClicked);
                //                     }
                //                     else if (_tutorial.ToggleSetup.UseCount == false)
                //                     {
                //                         _tutorial.ToggleGroup.SetAllTogglesOff();
                //                         _tutorial.ToggleSetup.Tog.isOn = true;
                //                     }
                //                 }
            }

            //             else if (_tutorial.ToggleSetup.UseCount == true)
            //             {
            //                 _tutorial.ToggleGroup.SetAllTogglesOff();
            //                 _tutorial.ToggleGroup.transform.GetChild(_tutorial.ToggleSetup.ChildCount).GetComponent<Toggle>().isOn = true;
            //             }

            if (_tutorial.Button != null)
            {
                //                 if (steps != (_mainTutorial[_mainStoryStatus].TutorialStep.Count - 1))
                //                 {
                //                     _tutorial.Button.onClick.Invoke();
                //                     _tutorial.Button.gameObject.SetActive(!_tutorial.DisableWhenClicked);
                //                 }
            }

            steps++;
            _tutorial = _mainTutorial[_mainStoryStatus].TutorialStep[steps];

            StartCoroutine(UpdateDisplayAfterTime());
        }
        else
        {
            // do the completed quest rewards
            NextTutorialStep();
        }
    }

    // following sets up the hero for the tutorial
    private void HeroGuidePressed(ushort id)
    {
        Assets.GetHero(id, out _heroSO);
        NextMainTutorialStep?.Invoke();
    }

    private void UpdateDisplay()
    {
        Debug.Log($"Update Display. Step: {steps}");

        #region Determine if hero is already select, set heroSo , Show Ship && enable Camera && Enable Target
        //         if (_mainStoryStatus == 0 && steps == 0 && playerData.TutorialData.HeroGuide != 0)
        //         {
        //             Debug.Log("Main Hero Been Selected");
        //             Assets.GetHero(playerData.TutorialData.HeroGuide, out _heroSO);
        //             NextStepUpdate();
        //             return;
        //         }
        //         else
        //             Debug.Log("Main Hero Hasn't Been Selected");

        // set the tutorial main hero
        if (_heroSO == null)
            Assets.GetHero(PlayerManager.TutorialData.HeroGuide, out _heroSO);

        TutorialStepsSO stepSO = _mainTutorial[_mainStoryStatus].TutorialStep[steps].StepSO;

        //_objects.SkipButton.SetActive(stepSO.ShowSkip);
        //_objects.SkipButton.GetComponent<TutorialConfirmationSkip>().Init(TutorialType.Main);

        //         _movableCamera.enabled = _tutorial.CameraMovement;
        //                 if (_tutorial.Targets != null)
        //                      _tutorial.Targets.SetActive(true);

        //                   if (steps > 0 && _mainTutorial[_mainStoryStatus].TutorialStep[steps - 1].Targets != null && _mainTutorial[_mainStoryStatus].TutorialStep[steps - 1].DisableWhenClicked == true)
        //                       _mainTutorial[_mainStoryStatus].TutorialStep[steps - 1].Targets.SetActive(false);

        //ClearOldData();
        #endregion

        if (_spawnedObject != null)
            Destroy(_spawnedObject);

        if (_tutorial.Targets == null)
        {
            Debug.Log("No Target");

            _objects.Dialogue.Init(stepSO, _spawnedObject, _heroSO, false);
            _objects.Arrow.Init(stepSO, _spawnedObject, false);
            _objects.Black.Init(stepSO, _spawnedObject, false);
            _objects.Blocker.Init(stepSO, _spawnedObject, false);
        }

        // 2D Target
        else if (_tutorial.Targets.GetComponent<RectTransform>() != null)
        {
            Debug.Log("2D Target");

            if (_tutorial.Targets != null)
            {
                RectTransform originalTransform = _tutorial.Targets.GetComponent<RectTransform>();

                _spawnedObject                                         = Instantiate(_targetObject, _tutorial.Targets.transform);
                _spawnedObject.GetComponent<RectTransform>().sizeDelta = new Vector2(originalTransform.rect.width, originalTransform.rect.height);
            }

            _objects.Dialogue.Init(stepSO, _spawnedObject, _heroSO, true);
            _objects.Arrow.Init(stepSO, _spawnedObject, true);
            _objects.Black.Init(stepSO, _spawnedObject, true);
            _objects.Blocker.Init(stepSO, _spawnedObject, true);
        }
        else if (_tutorial.Targets.GetComponent<Transform>() != null)
        {
            switch (_tutorial.FindBuilding)
            {
                case BuildingId.UseTarget:
                    // move camera to spot
                    Vector3 distance = _tutorial.Targets.transform.position - _target3DReference.transform.position;

                    Vector3 camLocation         = _cameraManager.transform.localPosition;
                    Vector3 camTargetedLocation = camLocation + distance;

                    float mulX = Screen.safeArea.width / Screen.width;
                    float mulY = Screen.safeArea.height / Screen.height;

                    Debug.Log($"thingi {camTargetedLocation} x: {mulX} y: {mulY}");

                    // #SIMONETODO - Camera moving to position
                    _cameraManager.enabled = true;
                    _cameraManager.SetTweenValue(0, camLocation, camTargetedLocation);

                    //_cameraManager.SetTweenValue(0, camLocation, new Vector3(camTargetedLocation.x, camTargetedLocation.y, camTargetedLocation.z * mulX));
                    _cameraManager.Play();

                    /*                    OnTutorialAim?.Invoke();*/

                    _objects.Dialogue.Init(stepSO, _tutorial.Targets, _heroSO, false);
                    _objects.Arrow.Init(stepSO, _tutorial.Targets, false);
                    _objects.Black.Init(stepSO, _tutorial.Targets, false);
                    _objects.Blocker.Init(stepSO, _tutorial.Targets, false);
                    break;
                default:
                    Debug.Log("TODO - Target Building");
                    break;
            }
        }
        else
        {
            Debug.LogError("Target Error");
        }
    }

    private void SkipTutorialClicked(bool skipEverything)
    {
        // clear the tutorial pop ups  (that are spawned)
        ClearOldData();

        // turn off the skip button
        _objects.SkipButton.SetActive(false);

        //         _movableCamera.enabled = true;

        // go one step forward
        if (!skipEverything)
            ClientNetworkUtils.SendServerMessage(MessageNames.TutorialMessage, new TutorialMessageData(100));

        // skip the whole quest
        else
            ClientNetworkUtils.SendServerMessage(MessageNames.TutorialMessage, new TutorialMessageData(150));
    }

    private void NextTutorialStep()
    {
        // clear the tutorial popups  (that are spawned)
        //ClearOldData();

        Debug.Log("Next Tutorial Step");

        // turn off the skip button
        _objects.SkipButton.SetActive(false);
        /*        _movableCamera.enabled = true;*/

        ClientNetworkUtils.SendServerMessage(MessageNames.TutorialMessage, new TutorialMessageData(100));
    }

    private IEnumerator UpdateDisplayAfterTime()
    {
        yield return new WaitForSeconds(0.05f);

        //After we have waited 5 seconds print the time again.
        UpdateDisplay();
    }

    private void ClearOldData() => Debug.Log("Clear Old Data");

    //         if (_objects.ArrowSpawnPoint.childCount > 0)
    //             foreach (Transform child in _objects.ArrowSpawnPoint)
    //                 Destroy(child.gameObject);
    // 
    //         if (_objects.DialogueSpawnPoint.childCount > 0)
    //             foreach (Transform child in _objects.DialogueSpawnPoint)
    //                 Destroy(child.gameObject);
    // 
    //         if (_objects.BlackSpawnPoint.childCount > 0)
    //             foreach (Transform child in _objects.BlackSpawnPoint)
    //                 Destroy(child.gameObject);
    // 
    //         if (_objects.BlockoutSpawnPoint.childCount > 0)
    //             foreach (Transform child in _objects.BlockoutSpawnPoint)
    //                 Destroy(child.gameObject);
}