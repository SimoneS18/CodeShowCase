using UnityEngine;
using UnityEngine.UI;

namespace UI.Tutorial
{
public class TutorialNextButton : MonoBehaviour
{
    private void Awake() => gameObject.GetComponent<Button>().onClick.AddListener(() => NextMainTutorialStep());

    //     public void Init(/*TutorialType _type*/)
    //     {
    //         if (gameObject.GetComponent<Button>() != null)
    //         {
    //             Button button = gameObject.GetComponent<Button>();
    // //             switch (_type)
    // //             {
    // /*                case TutorialType.Main:*/
    //                     button.onClick.AddListener(() => NextMainTutorialStep());
    // /*                    break;*/
    // //                 case TutorialType.Panel:
    // //                     button.onClick.AddListener(() => NextPanelTutorialStep());
    // //                     break;
    // /*            }*/
    //         }
    //     }

    // Wont be used for panel tutorials
    private void OnMouseDown()
    {
        if (gameObject.GetComponent<Button>() == null)
        {
            Ray        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //hits for sure, because we are in the click event
            Physics.Raycast(ray, out hit);

            NextMainTutorialStep();
            gameObject.SetActive(false);
        }
    }

    private void NextMainTutorialStep() => TutorialMain.NextMainTutorialStep?.Invoke();

    // private void NextPanelTutorialStep() => TutorialPanels.NextPanelTutorialStep?.Invoke();
}
}