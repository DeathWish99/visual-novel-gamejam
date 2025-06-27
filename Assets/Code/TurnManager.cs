using System;
using System.Collections.Generic;
using UnityEngine;
using VisualNovel.GameJam.Manager;

public class TurnManager : MonoBehaviour
{
    public event Action<CombatUnit> OnTurnStarted;

    private Queue<CombatUnit> turnQueue;
    private List<CombatUnit> allUnits;

    public void Initialise(List<CombatUnit> units)
    {
        allUnits = units;
        turnQueue = new Queue<CombatUnit>();
        PopulateTurnQueue();
    }

    public void StartNextTurn()
    {
        if (turnQueue.Count == 0)
        {
            PopulateTurnQueue();
        }

        CombatUnit currentUnit = turnQueue.Dequeue();

        if (currentUnit.IsDead)
        {
            StartNextTurn();
            return;
        }

        OnTurnStarted?.Invoke(currentUnit);

        if (!currentUnit.Stats.IsPlayer)
        {
            currentUnit.OnStartTurn();
            currentUnit.StartCoroutine(currentUnit.Agent.TakeTurn(currentUnit, EndTurn));
        }
    }

    public void EndTurn()
    {
        foreach (var unit in allUnits)
        {
            unit.OnEndTurn();
        }

        StartNextTurn();
    }

    private void PopulateTurnQueue()
    {
        allUnits.Sort((a, b) =>
        {
            if (a.Stats.Speed == b.Stats.Speed)
            {
                if (a.Stats.IsPlayer) return -1;
                if (b.Stats.IsPlayer) return 1;
                return 0;
            }
            return b.Stats.Speed.CompareTo(a.Stats.Speed);
        });

        foreach (var unit in allUnits)
        {
            if (!unit.IsDead)
                turnQueue.Enqueue(unit);
        }
    }
}
