using TMPro;
using UnityEngine;

namespace UI.Hero
{
public class SPCostUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _amountText;

    internal void Init(int cost) => _amountText.text = $"{cost}";
}
}