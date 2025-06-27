using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public event Action<CombatUnit> OnTurnStarted;
    public event Action<CombatUnit> OnTurnEnded;
    public event Action OnRoundEnded;

    private Queue<CombatUnit> turnQueue = new();
    private List<CombatUnit> allUnits = new();

    public void Initialise(List<CombatUnit> units)
    {
        allUnits = new List<CombatUnit>(units);
        BuildTurnQueue();
    }

    public void StartNextTurn()
    {
        if (IsBattleOver())
            return;

        if (turnQueue.Count == 0)
        {
            OnRoundEnded?.Invoke();
            BuildTurnQueue();
        }

        foreach (var unit in allUnits)
            unit.OnEndTurn();

        CombatUnit currentUnit = turnQueue.Dequeue();

        if (currentUnit.IsDead)
        {
            StartNextTurn();
            return;
        }

        OnTurnStarted?.Invoke(currentUnit);
        currentUnit.OnStartTurn();
    }

    public void EndCurrentTurn(CombatUnit unit)
    {
        OnTurnEnded?.Invoke(unit);
        StartNextTurn();
    }

    private void BuildTurnQueue()
    {
        List<CombatUnit> aliveUnits = allUnits.FindAll(u => !u.IsDead);

        aliveUnits.Sort((a, b) =>
        {
            if (a.Stats.Speed == b.Stats.Speed)
            {
                if (a.Stats.IsPlayer) return -1;
                if (b.Stats.IsPlayer) return 1;
                return 0;
            }
            return b.Stats.Speed.CompareTo(a.Stats.Speed);
        });

        turnQueue = new Queue<CombatUnit>(aliveUnits);
    }

    private bool IsBattleOver()
    {
        bool allEnemiesDead = allUnits.TrueForAll(u => u.Stats.IsPlayer || u.IsDead);
        bool allPlayersDead = allUnits.TrueForAll(u => !u.Stats.IsPlayer || u.IsDead);

        return allEnemiesDead || allPlayersDead;
    }
}
