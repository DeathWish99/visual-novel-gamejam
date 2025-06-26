using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AttackSkill")]
public class AttackSkill : Skill
{
    [SerializeField] private float damagePercent;
    [SerializeField] private float critRate;

    private const float CRIT_MODIFIER = 1.3f;

    private float DamagePercent => damagePercent / 100f;
    private float CritRate => critRate;


    public override void Activate(CombatUnit user, CombatUnit target)
    {
        bool isCrit = Random.value < CritRate;
        float damage = user.Stats.Attack * DamagePercent;

        if (isCrit) damage *= CRIT_MODIFIER;

        target.TakeDamage((int)damage);
    }
}
