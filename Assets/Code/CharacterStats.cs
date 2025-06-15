using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    [SerializeField] private string name;
    [SerializeField] private int maxHP;
    [SerializeField] private int currentHP;
    [SerializeField] private int attack;
    [SerializeField] private int defence;
    [SerializeField] private int speed;

    public int Defence => defence;
    public bool IsDead => currentHP <= 0;

    /// <summary>
    /// Calculates how much damage this character can deal to the target.
    /// </summary>
    /// <param name="target">The character which damage can be dealt to.</param>
    /// <returns>The damage that can be dealt to the target.</returns>
    public int CalculateDamage(CharacterStats target)
    {
        int rawDamage = attack - target.Defence;

        return Mathf.Max(rawDamage, 0);
    }
}
