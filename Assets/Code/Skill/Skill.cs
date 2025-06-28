using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [SerializeField] protected string skillName;
    [TextArea][SerializeField] protected string description;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected SkillType type;
    [SerializeField] protected bool canTargetPlayer;
    [SerializeField] protected bool canTargetCompanion;
    [SerializeField] protected bool canTargetEnemy;

    public Sprite Icon => icon;
    public string SkillName => skillName;
    public string Description => description;
    public bool CanTargetPlayer => canTargetPlayer;
    public bool CanTargetCompanion => canTargetCompanion;
    public bool CanTargetEnemy => canTargetEnemy;

    public abstract void Activate(CombatUnit user, CombatUnit target);

    public abstract string GetStatsText();
}
