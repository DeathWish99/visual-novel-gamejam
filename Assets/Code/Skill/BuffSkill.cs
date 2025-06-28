using UnityEngine;

[CreateAssetMenu(menuName = "Skills/BuffSkill")]
public class BuffSkill : Skill
{
    [SerializeField] private StatType[] buffedStats;
    [SerializeField] private int buffAmount;
    [SerializeField] private int duration;

    private StatType[] BuffedStats => buffedStats;
    private int BuffAmount => buffAmount;
    private int Duration => duration;

    public override void Activate(CombatUnit user, CombatUnit target)
    {
        foreach (var stat in BuffedStats)
        {
            target.ApplyEffect(stat, BuffAmount, Duration);
        }
    }

    public override string GetStatsText()
    {
        string prefix = buffAmount < 0 ? "Debuff" : "Buff";

        string affectedStats = string.Join(", ", BuffedStats);

        return $"{prefix} Amount: {buffAmount}\nDuration: {duration} Turns\nAffects: {affectedStats}";
    }

}
