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
}
