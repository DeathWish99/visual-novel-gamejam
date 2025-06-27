using System.Collections;
using UnityEngine;

public class AIAgent : MonoBehaviour, IAgent
{
    public IEnumerator TakeTurn(CombatUnit unit, System.Action onComplete)
    {
        yield return new WaitForSeconds(1f);

        CombatUnit target = FindTarget(unit);

        if (target != null)
        {
            unit.OnAttack();
            target.TakeDamage(unit.Attack);
        }

        onComplete?.Invoke();
    }

    private CombatUnit FindTarget(CombatUnit unit)
    {
        var potentialTargets = CombatManager.Instance.GetTargets(unit);

        return potentialTargets.Find(t => !t.IsDead);
    }
}
