using Shared.Enums.Tutorial;
using Shared.Scriptables.Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tutorial
{
public class UITutorialBlocker : MonoBehaviour
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

    public void Init(TutorialStepsSO tutorialStepSO, GameObject targetAim, bool target2D)
    {
        _tutorialStepSO = tutorialStepSO;
        _targetAim      = targetAim;
        _target2D       = target2D;

        UpdateDisplay();
    }

    private void UpdateDisplay()
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

                /*                _everything.GetComponent<TutorialNextButton>().Init(/ *TutorialType.Main* /);*/

                // enable skip or no skip - and enable||disable ray casting
                _everything.GetComponent<Button>().interactable = _tutorialStepSO.SkipEnabled;
                _everything.GetComponent<Image>().raycastTarget = true;
                break;
            case BlockerType.BlockTarget:
                _everything.SetActive(false);
                _target.SetActive(true);

                Debug.Log("TODO - block Target");

                Transform originalTransform = _target.GetComponent<Transform>();

                Vector2 centrePoint = _camera.WorldToScreenPoint(originalTransform.position);

                //                 #region BlockEverything except Target
                //                 GameObject blockout = Instantiate(_objects.BlockoutTargeted, _objects.BlockoutSpawnPoint);
                //                 RectTransform blockoutTransform = blockout.GetComponent<RectTransform>();
                // 
                //                 blockout.GetComponent<TutorialNextButton>().Init(TutorialType.Main);
                // 
                //                 foreach (Transform child in blockout.transform)
                //                     child.GetComponent<TutorialNextButton>().Init(TutorialType.Main);
                // 
                //                 // this determine if something is assigned to the toggle or button
                //                 // then disable the button on the blocker if there is or isnt
                //                 if (_tutorial.ToggleSetup.Tog != null || _tutorial.Button != null)
                //                 {
                //                     blockout.GetComponent<Button>().interactable = true;
                //                     blockout.GetComponent<Image>().raycastTarget = true;
                //                 }
                //                 else
                //                 {
                //                     blockout.GetComponent<Button>().interactable = false;
                //                     blockout.GetComponent<Image>().raycastTarget = false;
                //                 }
                // 
                //                 // enable skip or no skip
                //                 foreach (Transform child in blockoutTransform)
                //                 {
                //                     child.GetComponent<Button>().interactable = _tutorial.SkipEnabled;
                //                     child.GetComponent<Image>().raycastTarget = true;
                //                 }
                //                 if (_tutorial.TargetControl.UseTarget2D)
                //                 {
                //                     RectTransform target = _tutorial.TargetControl.Targets.GetComponent<RectTransform>();
                // 
                //                     // set size
                //                     blockoutTransform.sizeDelta = new Vector2(target.rect.width, target.rect.height);
                //                     // set position
                //                     blockoutTransform.position = target.gameObject.transform.position;
                //                 }
                //                 else
                //                 {
                //                     // get the screen point
                //                     var centrePoint = _camera.WorldToScreenPoint(_tutorial.TargetControl.Targets.transform.GetChild(2).transform.position);
                // 
                //                     // get left and right point to get size of black out
                //                     Vector2 leftPoint = _camera.WorldToScreenPoint(_tutorial.TargetControl.Targets.transform.GetChild(0).transform.position);
                //                     Vector2 rightPoint = _camera.WorldToScreenPoint(_tutorial.TargetControl.Targets.transform.GetChild(1).transform.position);
                // 
                //                     Vector2 screen = new Vector2(Screen.width, Screen.height);
                //                     Vector2 safeArea = new Vector2(Screen.safeArea.width, Screen.safeArea.height);
                // 
                //                     float distance;
                // 
                //                     // changes distance depending if using safe area or not
                //                     if (screen != safeArea)
                //                         distance = Vector2.Distance(leftPoint, rightPoint) - Vector3.Magnitude(screen - safeArea);
                //                     else
                //                         distance = Vector2.Distance(leftPoint, rightPoint);
                // 
                //                     Vector2 anchorPoints = new Vector2(centrePoint.x / Screen.safeArea.width, centrePoint.y / Screen.safeArea.height);
                // 
                //                     // set the anchor points
                //                     blockoutTransform.anchorMin = anchorPoints;
                //                     blockoutTransform.anchorMax = anchorPoints;
                // 
                //                     //blackoutTransform.sizeDelta = new Vector2(distance, distance);
                //                     blockoutTransform.sizeDelta = new Vector2(distance - 50, distance - 50);
                //                 }
                //                 #endregion
                break;
            case BlockerType.BlockTargetOnly:
                _everything.SetActive(false);
                _target.SetActive(true);

                Debug.Log("TODO - BlockTargetOnly");

                //                 GameObject blockoutTargetOnly = Instantiate(_objects.BlockoutTargetOnly, _objects.BlockoutSpawnPoint);
                //                 RectTransform blockoutTargetOnlyTransform = blockoutTargetOnly.GetComponent<RectTransform>();
                // 
                //                 blockoutTargetOnly.GetComponent<TutorialNextButton>().Init(TutorialType.Main);
                // 
                //                 blockoutTargetOnly.GetComponent<Button>().interactable = true;
                //                 blockoutTargetOnly.GetComponent<Image>().raycastTarget = true;
                // 
                //                 if (_tutorial.TargetControl.UseTarget2D)
                //                 {
                //                     RectTransform target = _tutorial.TargetControl.Targets.GetComponent<RectTransform>();
                // 
                //                     // set size
                //                     blockoutTargetOnlyTransform.sizeDelta = new Vector2(target.rect.width, target.rect.height);
                //                     // set position
                //                     blockoutTargetOnlyTransform.position = target.gameObject.transform.position;
                //                 }
                //                 else
                //                 {
                //                     // get the screen point
                //                     var centrePoint = _camera.WorldToScreenPoint(_tutorial.TargetControl.Targets.transform.GetChild(2).transform.position);
                // 
                //                     // get left and right point to get size of black out
                //                     Vector2 leftPoint = _camera.WorldToScreenPoint(_tutorial.TargetControl.Targets.transform.GetChild(0).transform.position);
                //                     Vector2 rightPoint = _camera.WorldToScreenPoint(_tutorial.TargetControl.Targets.transform.GetChild(1).transform.position);
                // 
                //                     Vector2 screen = new Vector2(Screen.width, Screen.height);
                //                     Vector2 safeArea = new Vector2(Screen.safeArea.width, Screen.safeArea.height);
                // 
                //                     float distance;
                // 
                //                     // changes distance depending if using safe area or not
                //                     if (screen != safeArea)
                //                         distance = Vector2.Distance(leftPoint, rightPoint) - Vector3.Magnitude(screen - safeArea);
                //                     else
                //                         distance = Vector2.Distance(leftPoint, rightPoint);
                // 
                //                     Vector2 anchorPoints = new Vector2(centrePoint.x / Screen.safeArea.width, centrePoint.y / Screen.safeArea.height);
                // 
                //                     // set the anchor points
                //                     blockoutTargetOnlyTransform.anchorMin = anchorPoints;
                //                     blockoutTargetOnlyTransform.anchorMax = anchorPoints;
                // 
                //                     //blackoutTransform.sizeDelta = new Vector2(distance, distance);
                //                     blockoutTargetOnlyTransform.sizeDelta = new Vector2(distance - 50, distance - 50);
                //                 }
                break;
        }
    }
}
}