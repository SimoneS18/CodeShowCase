using UnityEngine;
using UnityEngine.UI;

// following sets up the skip button and spawn in the Confirmation Panel on click
namespace UI.Tutorial
{
public class TutorialConfirmationSkip : MonoBehaviour
{
    public Button     skipButton;
    public Transform  spawnPoint;
    public GameObject confirmationPrefab;

    private TutorialType type;

    public void Init(TutorialType _type)
    {
        type = _type;

        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(() => SkipButtonPressed());
    }

    private void SkipButtonPressed()
    {
        GameObject confirmationPanel = Instantiate(confirmationPrefab, spawnPoint);
        confirmationPanel.GetComponent<TutorialConfirmationPanel>().Init(type);
    }
}
}