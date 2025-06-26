using UnityEngine;

public class ActiveEffect
{
    public StatType Stat { get; private set; }
    public int Amount { get; private set; }
    private int RemainingDuration { get; set; }

    public ActiveEffect(StatType stat, int amount, int duration)
    {
        Stat = stat;
        Amount = amount;
        RemainingDuration = duration;
    }

    public void ReduceRemainingDuration()
    {
        RemainingDuration--;
    }

    public bool ShouldRemove()
    {
        return RemainingDuration <= 0;
    }
}