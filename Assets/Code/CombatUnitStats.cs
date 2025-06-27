using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CombatUnitStats", menuName = "Scriptable Objects/CombatUnitStats")]
[System.Serializable]
public class CombatUnitStats : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string unitName;
    [SerializeField] private Image icon;

    [Header("Allegiance")]
    [SerializeField] private bool isPlayer;
    [SerializeField] private bool isCompanion;

    [Header("Combat Stats")]
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defence;
    [SerializeField] private int speed;

    // Public Properties
    public string UnitName => unitName;
    public Image Icon => icon;

    public bool IsPlayer => isPlayer;
    public bool IsCompanion => isCompanion;

    public int MaxHP => maxHP;
    public int Attack => attack;
    public int Defence => defence;
    public int Speed => speed;
}
