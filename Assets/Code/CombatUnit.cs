using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUnit : MonoBehaviour
{
    [SerializeField] private CombatUnitStats stats;

    public CombatUnitStats Stats => stats;
    public bool IsDead => CurrentHP <= 0;
    public int Attack => Stats.Attack + AttackBuff;

    private int CurrentHP { get; set; }
    private bool IsEnemy => !Stats.IsPlayer && !Stats.IsCompanion;
    private List<ActiveEffect> ActiveEffects { get; set; }
    private int AttackBuff { get; set; }
    private Image Image { get; set; }
    private Color OriginalColour { get; set; }

    private void Awake()
    {
        CurrentHP = Stats.MaxHP;
        ActiveEffects = new List<ActiveEffect>();
        Image = gameObject.GetComponent<Image>();
        OriginalColour = Image.color;
    }

    public void OnClick()
    {
        CombatManager combatManager = CombatManager.Instance;

        switch (combatManager.CurrentInputMode)
        {
            case InputMode.ATTACK:
                if (IsEnemy) combatManager.TakePlayerAction(this);
                break;
            case InputMode.SKILL:
                if (CanAttack(combatManager.SelectedSkill))
                {
                    combatManager.ExecuteSkill(this);
                }

                break;
        }
    }

    public void OnStartTurn()
    {
        ReduceActiveEffectRemainingDurations();

        Debug.Log($"[TEST] {Stats.UnitName} Current HP: {CurrentHP}");
        Debug.Log($"[TEST] {Stats.UnitName} Attack Buff: {AttackBuff}");
    }

    public void TakeDamage(int rawDamage)
    {
        int finalDamage = rawDamage - Stats.Defence;
        finalDamage = Mathf.Max(finalDamage, 0);

        CurrentHP -= finalDamage;
        CurrentHP = Mathf.Max(CurrentHP, 0);

        Debug.Log($"[TEST] {gameObject.name} Takes Damage: {finalDamage} from raw damage {rawDamage}");

        if (IsDead)
        {
            gameObject.SetActive(false);
        }

        StartCoroutine(FlashColour(Color.red));
    }

    public void ApplyEffect(StatType stat, int amount, int duration)
    {
        switch (stat)
        {
            case StatType.HP:
                CurrentHP += amount;
                break;
            case StatType.ATK:
                AttackBuff = amount;
                break;
        }

        ActiveEffects.Add(new ActiveEffect(stat, amount, duration));

        StartCoroutine(FlashColour(Color.blue));
    }

    private void ReduceActiveEffectRemainingDurations()
    {
        foreach (var activeEffect in ActiveEffects)
        {
            activeEffect.ReduceRemainingDuration();

            if (activeEffect.ShouldRemove())
            {
                switch (activeEffect.Stat)
                {
                    case StatType.HP:
                        CurrentHP += -activeEffect.Amount;
                        break;
                    case StatType.ATK:
                        AttackBuff = 0;
                        break;
                }
            }
        }

        ActiveEffects.RemoveAll(effect => effect.ShouldRemove());
    }

    private bool CanAttack(Skill selectedSkill)
    {
        return (IsEnemy && selectedSkill.CanTargetEnemy) || (Stats.IsCompanion && selectedSkill.CanTargetCompanion);
    }

    private IEnumerator FlashColour(Color colour)
    {
        Image.color = colour;

        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float lerpFactor = t / 0.5f;
            Image.color = Color.Lerp(colour, OriginalColour, lerpFactor);
            yield return null;
        }

        Image.color = OriginalColour;
    }
}
