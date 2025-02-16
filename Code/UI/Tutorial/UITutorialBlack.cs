using Shared.Enums.Tutorial;
using Shared.Scriptables.Tutorial;
using Shared.Utils.Values;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tutorial
{
public class UITutorialBlack : MonoBehaviour
{
    [SerializeField]
    private GameObject _everything;

    [SerializeField]
    private GameObject _target;

    [SerializeField]
    private Camera _camera;

    private bool       _target2D;
    private GameObject _targetAim;

    private TutorialStepsSO _tutorialStepSO;

    public GameObject Everything => _everything;
    public GameObject Target     => _target;

    public void Init(TutorialStepsSO tutorialStepSO, GameObject targetAim, bool target2D)
    {
        _tutorialStepSO = tutorialStepSO;
        _targetAim      = targetAim;
        _target2D       = target2D;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_tutorialStepSO.UseBlackout)
        {
            switch (_tutorialStepSO.BlockerType)
            {
                case BlockerType.NoBlocker:
                    _everything.SetActive(false);
                    _target.SetActive(false);
                    break;
                case BlockerType.BlockEverything:
                    _everything.SetActive(true);
                    _target.SetActive(false);
                    break;
                case BlockerType.BlockTarget:
                    _everything.SetActive(false);
                    _target.SetActive(true);

                    // get blackouts target transform
                    RectTransform blackoutTransform = _target.GetComponent<RectTransform>();

                    if (_target2D)
                    {
                        // get target transform, set size
                        RectTransform target = _targetAim.GetComponent<RectTransform>();

                        blackoutTransform.sizeDelta = new Vector2(target.rect.width + GlobalSettings.blackoutExtender,
                                                                  target.rect.height + GlobalSettings.blackoutExtender);

                        blackoutTransform.position = target.gameObject.transform.position;
                    }

                    //                             Vector2 centrePoint = _camera.WorldToScreenPoint(_target.transform.position);
                    //                             Vector2 anchorPoints = new Vector2(centrePoint.x / Screen.safeArea.width, centrePoint.y / Screen.safeArea.height);
                    // 
                    // 
                    //                             blackoutTransform.anchorMin = anchorPoints;
                    //                             blackoutTransform.anchorMax = anchorPoints;
                    // 
                    //                             float pointX = Screen.safeArea.width * anchorPoints.x;
                    //                             float pointY = Screen.safeArea.height * anchorPoints.y;
                    // 
                    //                             Vector2 startPosition = new Vector2(arrowTransform.localPosition.x + (GlobalSettings.ArrowX[_tutorialStepSO.Direction] * GlobalSettings.arrowDistance), arrowTransform.localPosition.y + (GlobalSettings.ArrowY[_tutorialStepSO.Direction] * GlobalSettings.arrowDistance));
                    //-----
                    //                             // get the screen point
                    //                             Vector2 centrePoint = _camera.WorldToScreenPoint(_tutorial.TargetControl.Targets.transform.GetChild(2).transform.position);
                    // 
                    //                             // get left and right point to get size of black out
                    //                             Vector2 leftPoint = _camera.WorldToScreenPoint(_tutorial.TargetControl.Targets.transform.GetChild(0).transform.position);
                    //                             Vector2 rightPoint = _camera.WorldToScreenPoint(_tutorial.TargetControl.Targets.transform.GetChild(1).transform.position);
                    // 
                    //                             Vector2 screen = new Vector2(Screen.width, Screen.height);
                    //                             Vector2 safeArea = new Vector2(Screen.safeArea.width, Screen.safeArea.height);
                    // 
                    //                             float distance;
                    // 
                    //                             // changes distance depending if using safe area or not
                    //                             if (screen != safeArea)
                    //                                 distance = Vector2.Distance(leftPoint, rightPoint) - Vector3.Magnitude(screen - safeArea);
                    //                             else
                    //                                 distance = Vector2.Distance(leftPoint, rightPoint);
                    // 
                    //                             Vector2 anchorPoints = new Vector2(centrePoint.x / Screen.safeArea.width, centrePoint.y / Screen.safeArea.height);
                    // 
                    //                             // set the anchor points
                    //                             blackoutTransform.anchorMin = anchorPoints;
                    //                             blackoutTransform.anchorMax = anchorPoints;
                    // 
                    //                             blackoutTransform.sizeDelta = new Vector2(distance, distance);
                    //set blackoutsShape
                    switch (_tutorialStepSO.BlackOutShapes)
                    {
                        case BlackOutShapes.Circle:
                            _target.GetComponent<Image>().sprite = Resources.Load<Sprite>("Tutorial/Circle");
                            break;
                        case BlackOutShapes.Square:
                            _target.GetComponent<Image>().sprite = Resources.Load<Sprite>("Tutorial/Square");
                            break;
                        case BlackOutShapes.Other:
                            _target.GetComponent<Image>().sprite = _tutorialStepSO.NewShape;
                            break;
                    }

                    break;
                case BlockerType.BlockTargetOnly:
                    // do nothing
                    break;
            }
        }
        else
        {
            _everything.SetActive(false);
            _target.SetActive(false);
        }
    }
}
}