using TMPro;
using UnityEngine;
using UnityEngine.UI;

// following is on the Tutorial Confirmation Panel
namespace UI.Tutorial
{
public class TutorialConfirmationPanel : MonoBehaviour
{
    public GameObject toggle;
    public TMP_Text   _dialoge;
    public GameObject _warning;
    public Button     yesButton;
    public Button     noButton;


    public void Init(TutorialType type)
    {
        switch (type)
        {
            case TutorialType.Main:
                toggle.SetActive(true);

                _dialoge.text =
                    "Are you sure you want to skip this tutorial? \n You might get confused with how things work\n You can reset tutorial in settings anytime ";

                _warning.SetActive(toggle.GetComponent<UnityEngine.UI.Toggle>().isOn);
                yesButton.onClick.AddListener(() => SkipMainTutorial(toggle.GetComponent<UnityEngine.UI.Toggle>().isOn));

                toggle.GetComponent<UnityEngine.UI.Toggle>()
                      .onValueChanged.AddListener(isOn =>
                       {
                           if (isOn)
                               _warning.SetActive(true);
                           else
                               _warning.SetActive(false);
                       });

                break;
            case TutorialType.Panel:
                toggle.SetActive(false);
                _warning.SetActive(false);

                _dialoge.text =
                    "Are you sure you want to skip this tutorial? \n You can replay this tutorial anytime on the panels help page";

                //yesButton.onClick.AddListener(() => SkipPanelTutorial());
                break;
        }

        noButton.onClick.AddListener(() => NoPressed());
    }

    private void SkipMainTutorial(bool skipEverything)
    {
        Debug.Log($"Skipping Main (everything{skipEverything})");
        TutorialMain.SkipTutorial?.Invoke(skipEverything);
    }

    // private void SkipPanelTutorial() => TutorialPanels.SkipTutorial?.Invoke();

    private void NoPressed() => Destroy(gameObject);
}
}