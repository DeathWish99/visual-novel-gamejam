using UnityEngine;

[CreateAssetMenu(fileName = "CombatUnitStats", menuName = "Scriptable Objects/CombatUnitStats")]
[System.Serializable]
public class CombatUnitStats : ScriptableObject
{
    [SerializeField] private string unitName;
    [SerializeField] private bool isPlayer;
    [SerializeField] private bool isCompanion;
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defence;
    [SerializeField] private int speed;

    public string UnitName => unitName;
    public int MaxHP => maxHP;
    public int Attack => attack;
    public int Defence => defence;
    public int Speed => speed;
    public bool IsPlayer => isPlayer;
    public bool IsCompanion => isCompanion;
}
