using System.Linq;
using Managers;
using Shared.Data.Equipment;
using Shared.Enums;
using Shared.Scriptables.Equipment;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Equipment
{
/// <summary>
///     Following script is used in the Equipment Crafted Popup to display what equipment was crafted and that's equipment
///     stats
/// </summary>
public class EquipmentCraftedUI : MonoBehaviour
{
    [Header("Apperence"), SerializeField]
    private GameObject _image;

    [SerializeField]
    private TMP_Text _name;

    [SerializeField]
    private GameObject _statSpawnPoint;

    [Header("Prefabs"), SerializeField]
    private GameObject _statPrefab;

    private void OnDisable()
    {
        _name.text = "";

        foreach (Transform child in _statSpawnPoint.GetComponent<Transform>())
            Destroy(child.gameObject);
    }

    public void Init(ushort equipmentId, Rarity rarity)
    {
        if (!Assets.GetEquipment(equipmentId, out EquipmentSO equipmentSO))
            return;

        if (!PlayerManager.Equipment.Equipments.GetEquipment(equipmentId, out Shared.Data.Equipment.Equipment equipment))
            return;

        _name.text                          = $"{equipmentSO.Name}";
        _image.GetComponent<Image>().sprite = equipmentSO.Sprite;

        foreach (Transform child in _statSpawnPoint.GetComponent<Transform>())
            Destroy(child.gameObject);

        int           size          = equipment.Items.Count();
        EquipmentData equipmentData = equipment.Items[size - 1];

        //#SimoneTODO
        //             foreach (KeyValuePair<StatType, EquipmentStatsData> stat in equipmentData.Stats)
        //             {
        //                 GameObject obj = Instantiate(_statPrefab, _statSpawnPoint.GetComponent<Transform>());
        //                 obj.GetComponent<UIStatsText>().Init_EquipmentStatsData(equipmentData, stat.Value);
        //             }
    }
}
}