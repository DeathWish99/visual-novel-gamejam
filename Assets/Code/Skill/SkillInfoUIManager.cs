using UnityEngine;

public class SkillInfoUIManager : MonoBehaviour
{
    public static SkillInfoUIManager Instance { get; private set; }

    [SerializeField] private SkillInfoUI skillInfoUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowSkill(Skill skill) => skillInfoUI.ShowSkill(skill);
    public void LockSkillDisplay(Skill skill) => skillInfoUI.LockSkillDisplay(skill);
    public void Unlock() => skillInfoUI.UnlockSkillDisplay();
    public void Clear() => skillInfoUI.Clear();
}