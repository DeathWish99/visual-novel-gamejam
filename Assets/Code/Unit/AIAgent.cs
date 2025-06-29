using System.Collections;
using UnityEngine;

public class AIAgent : MonoBehaviour, IAgent
{
    [SerializeField] private Skill skill;

    private int skillCooldownTimer = 0;

    public IEnumerator TakeTurn(CombatUnit unit, System.Action onComplete)
    {
        yield return new WaitForSeconds(1f);

        CombatUnit target = FindTarget(unit);

        if (target != null)
        {
            unit.OnAttack();

            if (unit.Stats.HasSkill)
            {
                if (skillCooldownTimer-- <= 0)
                {
                    skillCooldownTimer = 2;
                    CombatManager.Instance.ExecuteSkill(skill, unit, target);
                }
            }
            else
            {
                target.TakeDamage(unit.Attack);
            }
        }

        yield return new WaitForSeconds(1f);

        onComplete?.Invoke();
    }

    private CombatUnit FindTarget(CombatUnit unit)
    {
        var potentialTargets = CombatManager.Instance.GetTargets(unit);

        return potentialTargets.Find(t => !t.IsDead);
    }
}
