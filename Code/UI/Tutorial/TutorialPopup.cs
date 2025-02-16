using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tutorial
{
public class TutorialPopup : MonoBehaviour
{
    [Header("Appearance"), SerializeField]
    private Image _guide;

    [SerializeField]
    private TMP_Text _name;

    [SerializeField]
    private TMP_Text _instructions;

    public Image    Guide        => _guide;
    public TMP_Text Name         => _name;
    public TMP_Text Instructions => _instructions;
}
}