using Shared.Enums.Tutorial;
using Shared.Scriptables.Tutorial;
using Shared.Utils.Values;
using UnityEngine;

namespace UI.Tutorial
{
public class UITutorialArrow : MonoBehaviour
{
    [SerializeField]
    private GameObject _arrow;

    [SerializeField]
    private Camera _camera;

    private GameObject _target;
    private bool       _target2D;

    private TutorialStepsSO _tutorialStepSO;

    public void Init(TutorialStepsSO tutorialStepSO, GameObject target, bool target2D)
    {
        _tutorialStepSO = tutorialStepSO;
        _target         = target;
        _target2D       = target2D;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_tutorialStepSO.UseArrow)
        {
            _arrow.SetActive(true);

            RectTransform arrowTransform = _arrow.GetComponent<RectTransform>();

            // double check if there is a target
            if (_target == null)
                Debug.LogError("There is no target for the arrow to aim at!", gameObject);

            // reset arrows transform
            _arrow.transform.rotation = Quaternion.identity;

            // set the rotation
            switch (_tutorialStepSO.Direction)
            {
                case ArrowDirections.Left:
                    _arrow.transform.Rotate(0, 0.0f, 90.0f, Space.Self);
                    break;
                case ArrowDirections.Right:
                    _arrow.transform.Rotate(0, 0.0f, -90.0f, Space.Self);
                    break;
                case ArrowDirections.Up:
                    _arrow.transform.Rotate(0, 0.0f, 0, Space.Self);
                    break;
                case ArrowDirections.Down:
                    _arrow.transform.Rotate(0, 0.0f, 180.0f, Space.Self);
                    break;
            }

            if (_target2D)
            {
                _arrow.SetActive(true);

                RectTransform target = _target.GetComponent<RectTransform>();

                float width  = target.rect.width;
                float height = target.rect.height;

                //                              (-1, -1, 0, 0) * width) + (distance away from target * (-1, 1, 0, 0))
                float xPosition = GlobalSettings.ArrowX[_tutorialStepSO.Direction] * width +
                                  GlobalSettings.arrowDistance * GlobalSettings.ArrowX[_tutorialStepSO.Direction];

                float yPosition = GlobalSettings.ArrowY[_tutorialStepSO.Direction] * height +
                                  GlobalSettings.arrowDistance * GlobalSettings.ArrowY[_tutorialStepSO.Direction];

                // set anchor points
                arrowTransform.anchorMin = GlobalSettings.UIAnchorPosition(target);
                arrowTransform.anchorMax = GlobalSettings.UIAnchorPosition(target);

                // set position
                arrowTransform.anchoredPosition = new Vector3(xPosition, yPosition, 0);

                #region Set up Tween
                Vector3 PositionCurrent = arrowTransform.transform.localPosition;

                Vector3 PositionTo =
                    new Vector3(PositionCurrent.x + GlobalSettings.ArrowX[_tutorialStepSO.Direction] * GlobalSettings.arrowMovingDistance,
                                PositionCurrent.y + GlobalSettings.ArrowY[_tutorialStepSO.Direction] * GlobalSettings.arrowMovingDistance,
                                PositionCurrent.z);

                // set up animation of tween
                Jun_TweenRuntime tween = _arrow.GetComponent<Jun_TweenRuntime>();

                tween.SetTweenValue(0, PositionCurrent, PositionTo);
                tween.SetTweenValue(1, PositionTo,      PositionCurrent);
                #endregion
            }
            else
            {
                /*                    Debug.Log("TODO - 3D Target");*/

                // following gets the position of the World object and converts it so can be used on the canvas
                Vector3 centrePoint = _camera.WorldToScreenPoint(_target.transform.position);

                Vector2 anchorPoints = new Vector2(centrePoint.x / Screen.safeArea.width, centrePoint.y / Screen.safeArea.height);

                // set the anchor points

                Vector2 centre = new Vector2(anchorPoints.x + GlobalSettings.ArrowX[_tutorialStepSO.Direction] * GlobalSettings.arrowDistance,
                                             anchorPoints.y + GlobalSettings.ArrowY[_tutorialStepSO.Direction] * GlobalSettings.arrowDistance);

                arrowTransform.anchorMin = anchorPoints;
                arrowTransform.anchorMax = anchorPoints;

                float pointX = Screen.safeArea.width * anchorPoints.x;
                float pointY = Screen.safeArea.height * anchorPoints.y;

                //Vector2 startPosition = arrowTransform.localPosition;
                Vector2 startPosition =
                    new Vector2(arrowTransform.localPosition.x + GlobalSettings.ArrowX[_tutorialStepSO.Direction] * GlobalSettings.arrowDistance,
                                arrowTransform.localPosition.y + GlobalSettings.ArrowY[_tutorialStepSO.Direction] * GlobalSettings.arrowDistance);

                Vector2 finishPosition =
                    new Vector2(startPosition.x + GlobalSettings.ArrowX[_tutorialStepSO.Direction] * GlobalSettings.arrowMovingDistance,
                                startPosition.y + GlobalSettings.ArrowY[_tutorialStepSO.Direction] * GlobalSettings.arrowMovingDistance);

                // set up animation of tween
                Jun_TweenRuntime tween = _arrow.GetComponent<Jun_TweenRuntime>();

                tween.SetTweenValue(0, startPosition,  finishPosition);
                tween.SetTweenValue(1, finishPosition, startPosition);
            }
        }
        else
        {
            _arrow.SetActive(false);
        }
    }
}
}