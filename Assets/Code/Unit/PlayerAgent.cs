using System.Collections;
using UnityEngine;

public class PlayerAgent : MonoBehaviour, IAgent
{
    public IEnumerator TakeTurn(CombatUnit unit, System.Action onComplete)
    {
        CombatManager.Instance.OnPlayerTurn();
        yield return null;
    }
}
