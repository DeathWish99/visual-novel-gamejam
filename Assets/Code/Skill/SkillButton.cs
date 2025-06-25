using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private Skill skill;

    private Skill Skill => skill;
    private Image IconImage { get; set; }
    private Button Button { get; set; }

    private void Start()
    {
        IconImage = gameObject.GetComponent<Image>();
        Button = gameObject.GetComponent<Button>();

        if (Skill != null && IconImage != null)
        {
            IconImage.sprite = Skill.Icon;
        }
    }

    public void OnClick()
    {
        CombatManager.Instance.OnSkillButtonClick(Skill);
    }

    public void Disable()
    {
        Button.interactable = false;
    }

    public void Enable()
    {
        Button.interactable = true;
    }
}
