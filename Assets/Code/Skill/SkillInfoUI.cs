using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statsText;

    private bool isLocked = false;

    public void ShowSkill(Skill skill)
    {
        if (isLocked || skill == null) return;

        nameText.text = skill.SkillName.ToUpper();
        descriptionText.text = skill.Description.ToLower();
        statsText.text = skill.GetStatsText().ToLower();
    }

    public void LockSkillDisplay(Skill skill)
    {
        isLocked = true;
        ShowSkill(skill);
    }

    public void UnlockSkillDisplay()
    {
        isLocked = false;
    }

    public void Clear()
    {
        if (isLocked) return;
        nameText.text = "";
        descriptionText.text = "";
        statsText.text = "";
    }
}
