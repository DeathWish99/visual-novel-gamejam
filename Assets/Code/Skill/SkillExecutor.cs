using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public void Execute(Skill skill, CombatUnit user, CombatUnit targets)
    {
        skill.Activate(user, targets);
    }
}
