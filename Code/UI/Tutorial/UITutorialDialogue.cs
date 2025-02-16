using Shared.Enums;
using Shared.Enums.Tutorial;
using Shared.Scriptables.Hero;
using Shared.Scriptables.Tutorial;
using Shared.Utils.Values;
using UnityEngine;

namespace UI.Tutorial
{
public class UITutorialDialogue : MonoBehaviour
{
    [Header("Dialogues"), SerializeField]
    private GameObject _main;

    [SerializeField]
    private GameObject _mini;

    [SerializeField]
    private GameObject _large;

    [SerializeField]
    private GameObject _other;

    [SerializeField]
    private Camera _camera;

    private HeroSO     _heroSO;
    private GameObject _target;
    private bool       _target2D;

    //         [Header("Spawning Targeting")]
    //         [SerializeField]
    //         private GameObject _targetObject;
    //         private GameObject _spawnedObject;

    private TutorialStepsSO _tutorialStepSO;

    public void Init(TutorialStepsSO tutorialStepSO, GameObject target, HeroSO heroSO, bool target2D)
    {
        _tutorialStepSO = tutorialStepSO;
        _target         = target;
        _heroSO         = heroSO;
        _target2D       = target2D;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_tutorialStepSO.DialogueType == DialogueType.MainDialogue)
        {
            _main.SetActive(true);
            _mini.SetActive(false);
            _large.SetActive(false);

            TutorialPopup mainTutorial = _main.GetComponent<TutorialPopup>();

            mainTutorial.Name.text         = _heroSO.FirstName;
            mainTutorial.Instructions.text = _tutorialStepSO.Description;

            // #todoSimone
            // mainTutorial.Guide.sprite = _heroSO.GetHeroGestureImage(_tutorialStepSO.Gestures);
        }
        else if (_tutorialStepSO.DialogueType == DialogueType.MiniPopup)
        {
            _main.SetActive(false);
            _mini.SetActive(true);
            _large.SetActive(false);

            RectTransform dialogueMinRectTransform = _mini.GetComponent<RectTransform>();
            float         width                    = dialogueMinRectTransform.rect.width;
            float         height                   = dialogueMinRectTransform.rect.height;

            // not to target
            if (_tutorialStepSO.YPosition != YPosition.Target)
            {
                // set the anchor points of the dialog depending on 
                dialogueMinRectTransform.anchorMin = new Vector2(GlobalSettings.XPositions[_tutorialStepSO.XPosition],
                                                                 GlobalSettings.YPositions[_tutorialStepSO.YPosition]);

                dialogueMinRectTransform.anchorMax = new Vector2(GlobalSettings.XPositions[_tutorialStepSO.XPosition],
                                                                 GlobalSettings.YPositions[_tutorialStepSO.YPosition]);

                float xPosition = dialogueMinRectTransform.rect.width / 2 * GlobalSettings.LeftOfRight[_tutorialStepSO.XPosition] +
                                  GlobalSettings.dialogueSpacing * GlobalSettings.LeftOfRight[_tutorialStepSO.XPosition];

                float yPosition = dialogueMinRectTransform.rect.height / 2 * GlobalSettings.UpOrDown[_tutorialStepSO.YPosition] +
                                  GlobalSettings.dialogueSpacing * GlobalSettings.UpOrDown[_tutorialStepSO.YPosition];

                dialogueMinRectTransform.anchoredPosition = new Vector2(xPosition, yPosition);
            }

            // targeting
            else
            {
                if (_target == null)
                {
                    Debug.LogError("NO TARGET");
                    return;
                }

                // if 2D Target
                if (_target2D)
                {
                    RectTransform originalTransform = _target.GetComponent<RectTransform>();

                    float targetW = originalTransform.rect.width;
                    float targetH = originalTransform.rect.height;

                    // set anchors (to target)
                    dialogueMinRectTransform.anchorMin = GlobalSettings.UIAnchorPosition(originalTransform);
                    dialogueMinRectTransform.anchorMax = GlobalSettings.UIAnchorPosition(originalTransform);

                    // set Position
                    float newPositionX = width / 2 * GlobalSettings.ArrowX[_tutorialStepSO.Direction] +
                                         targetW / 2 * GlobalSettings.ArrowX[_tutorialStepSO.Direction];

                    float newPositionY = height / 2 * GlobalSettings.ArrowY[_tutorialStepSO.Direction] +
                                         targetH / 2 * GlobalSettings.ArrowY[_tutorialStepSO.Direction];

                    // add spacing
                    newPositionX = newPositionX + GlobalSettings.dialogueSpacing * GlobalSettings.ArrowX[_tutorialStepSO.Direction];
                    newPositionY = newPositionY + GlobalSettings.dialogueSpacing * GlobalSettings.ArrowY[_tutorialStepSO.Direction];

                    // set the position
                    dialogueMinRectTransform.anchoredPosition = new Vector2(newPositionX, newPositionY);

                    // determine if on or off the safe area
                    Vector2 offScreenAdding = GlobalSettings.KeepDialogueOnScreen(dialogueMinRectTransform, _main);

                    Vector2 arrowDiatanceAdding = GlobalSettings.AddArrowDistance(_tutorialStepSO);

                    dialogueMinRectTransform.anchoredPosition = new Vector2(newPositionX + offScreenAdding.x + arrowDiatanceAdding.x,
                                                                            newPositionY + offScreenAdding.y + arrowDiatanceAdding.y);
                }
                else
                {
                    Transform originalTransform = _target.GetComponent<Transform>();

                    Vector2 centrePoint  = _camera.WorldToScreenPoint(originalTransform.position);
                    Vector2 anchorPoints = new Vector2(centrePoint.x / Screen.safeArea.width, centrePoint.y / Screen.safeArea.height);

                    // Debug.Log($"centre Poiont{centrePoint}");
                }
            }

            if (_mini.GetComponent<TutorialPopup>() != null)
            {
                TutorialPopup setup = _mini.GetComponent<TutorialPopup>();

                setup.GetComponent<TutorialPopup>().Name.text         = _heroSO.FirstName;
                setup.GetComponent<TutorialPopup>().Instructions.text = _tutorialStepSO.Description;
                setup.GetComponent<TutorialPopup>().Guide.sprite      = _heroSO.Icon;
            }
        }
        else if (_tutorialStepSO.DialogueType == DialogueType.MiniLargePopup)
        {
            _main.SetActive(false);
            _mini.SetActive(false);
            _large.SetActive(true);
            ;

            RectTransform rectLargeMini = _large.GetComponent<RectTransform>();

            // set to anchors
            if ((int)_tutorialStepSO.YPosition > 0 && (int)_tutorialStepSO.YPosition < 4)
            {
                // set the anchor points of the dialog depending on 
                rectLargeMini.anchorMin = new Vector2(GlobalSettings.XPositions[_tutorialStepSO.XPosition],
                                                      GlobalSettings.YPositions[_tutorialStepSO.YPosition]);

                rectLargeMini.anchorMax = new Vector2(GlobalSettings.XPositions[_tutorialStepSO.XPosition],
                                                      GlobalSettings.YPositions[_tutorialStepSO.YPosition]);

                float xPosition = rectLargeMini.position.x + rectLargeMini.rect.width * GlobalSettings.LeftOfRight[_tutorialStepSO.XPosition];
                float yPosition = rectLargeMini.position.y + rectLargeMini.rect.height * GlobalSettings.UpOrDown[_tutorialStepSO.YPosition];

                rectLargeMini.position = new Vector2(xPosition, yPosition);
            }

            // next to arrow (target)
            else if ((int)_tutorialStepSO.YPosition > 3)
            {
                // get target
                RectTransform recTarget = _target.GetComponent<RectTransform>();

                float width     = recTarget.rect.width;
                float height    = recTarget.rect.height;
                float xPosition = GlobalSettings.ArrowX[_tutorialStepSO.Direction] * width;
                float yPosition = GlobalSettings.ArrowY[_tutorialStepSO.Direction] * height;

                // set anchors (to target)
                recTarget.anchorMin = GlobalSettings.UIAnchorPosition(recTarget);
                recTarget.anchorMax = GlobalSettings.UIAnchorPosition(recTarget);

                // set Position
                recTarget.anchoredPosition = new Vector3(xPosition, yPosition, 0);
            }

            if (_large.GetComponent<TutorialPopup>() != null)
            {
                TutorialPopup setup = _large.GetComponent<TutorialPopup>();

                setup.GetComponent<TutorialPopup>().Name.text         = _heroSO.FirstName;
                setup.GetComponent<TutorialPopup>().Instructions.text = _tutorialStepSO.Description;
                setup.GetComponent<TutorialPopup>().Guide.sprite      = _heroSO.Icon;
            }
        }
        else if (_tutorialStepSO.DialogueType == DialogueType.Other)
        {
            Debug.Log("Other Dialogue Spawning", gameObject);

            if (_tutorialStepSO.NewPopup == null)
                Debug.LogError("NO POPUP ADDED");

            // Default  - Already Set up Prefab
            GameObject otherPopup = Instantiate(_tutorialStepSO.NewPopup, _other.GetComponent<Transform>());

            if (otherPopup.GetComponent<TutorialPopup>() != null)
            {
                TutorialPopup setup = otherPopup.GetComponent<TutorialPopup>();

                setup.GetComponent<TutorialPopup>().Name.text         = _heroSO.FirstName;
                setup.GetComponent<TutorialPopup>().Instructions.text = _tutorialStepSO.Description;

                // #todoSimone
                // setup.GetComponent<TutorialPopup>().Guide.sprite = _heroSO.GetHeroGestureImage(_tutorialStepSO.Gestures);
            }
        }
        else if (_tutorialStepSO.DialogueType == DialogueType.None)
        {
            _main.SetActive(false);
            _mini.SetActive(false);
            _large.SetActive(false);
        }
    }
}
}