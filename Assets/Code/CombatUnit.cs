using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    [SerializeField] private CombatUnitStats stats;

    public CombatUnitStats Stats => stats;
    public bool IsDead => CurrentHP <= 0;
    private int CurrentHP { get; set; }

    private void Awake()
    {
        CurrentHP = Stats.MaxHP;
    }

    public void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        CurrentHP = Mathf.Max(CurrentHP, 0);

        Debug.Log($"{Stats.UnitName} takes {amount} damage! HP now {CurrentHP}");
    }
}
