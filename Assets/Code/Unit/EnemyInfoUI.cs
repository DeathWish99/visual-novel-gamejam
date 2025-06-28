using TMPro;
using UnityEngine;

public class EnemyInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI statsText;

    public void Show(CombatUnit unit)
    {
        if (unit == null) return;

        var stats = unit.Stats;

        nameText.text = stats.UnitName.ToUpper();
        hpText.text = $"hp: {unit.CurrentHP}";
        statsText.text = $"atk: {stats.Attack}\ndef: {stats.Defence}";
    }

    public void Clear()
    {
        nameText.text = "";
        hpText.text = "";
        statsText.text = "";
    }
}
