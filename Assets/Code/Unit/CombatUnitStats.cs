using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CombatUnitStats", menuName = "Scriptable Objects/CombatUnitStats")]
[System.Serializable]
public class CombatUnitStats : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string unitName;
    [SerializeField] private Sprite icon;

    [Header("Allegiance")]
    [SerializeField] private bool isPlayer;
    [SerializeField] private bool isCompanion;

    [Header("Combat Stats")]
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defence;
    [SerializeField] private int speed;

    [Header("Enemy-specific Stats")]
    [Tooltip("This is only used for enemies.")]
    [SerializeField] private bool hasSkill;

    // Public Properties
    public string UnitName => unitName;
    public Sprite Icon => icon;

    public bool IsPlayer => isPlayer;
    public bool IsCompanion => isCompanion;

    public int MaxHP => maxHP;
    public int Attack => attack;
    public int Defence => defence;
    public int Speed => speed;

    public bool HasSkill => hasSkill;
}
