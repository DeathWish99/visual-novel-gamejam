using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Skill skill;

    private Skill Skill => skill;
    private Image IconImage { get; set; }
    private Button Button { get; set; }

    private void Awake()
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        SkillInfoUIManager.Instance.ShowSkill(skill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SkillInfoUIManager.Instance.Clear();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SkillInfoUIManager.Instance.LockSkillDisplay(skill);
    }
}
