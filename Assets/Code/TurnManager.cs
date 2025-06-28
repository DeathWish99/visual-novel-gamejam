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

        currentUnit.OnStartTurn();
        currentUnit.StartCoroutine(currentUnit.Agent.TakeTurn(currentUnit, EndTurn));
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
        CombatUnit player = allUnits.Find(u => u.Stats.IsPlayer && !u.IsDead);
        List<CombatUnit> otherUnits = allUnits.FindAll(u => !u.Stats.IsPlayer && !u.IsDead);

        otherUnits.Sort((a, b) => b.Stats.Speed.CompareTo(a.Stats.Speed));

        if (player != null)
            turnQueue.Enqueue(player);
        
        foreach (var unit in otherUnits)
            turnQueue.Enqueue(unit);
    }
}
