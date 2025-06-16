using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    [SerializeField] private CombatUnitStats stats;

    public CombatUnitStats Stats => stats;
    private int currentHP;

    void Start()
    {
        currentHP = stats.MaxHP;
    }
}
