using System.Collections;

public interface IAgent
{
    IEnumerator TakeTurn(CombatUnit unit, System.Action onComplete);
}