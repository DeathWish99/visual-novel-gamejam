using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatSceneParameters", menuName = "Scriptable Objects/CombatSceneParameters")]
[System.Serializable]
public class CombatSceneParameters : ScriptableObject
{
    [SerializeField] private bool hasCompanion;
    [SerializeField] private List<EnemyType> enemies;
    [SerializeField] private List<SkillType> enabledSkills;
    [SerializeField] private bool disableSkills;

    [SerializeField] private string nextDialogID;

    public bool HasCompanion => hasCompanion;
    public List<EnemyType> Enemies => enemies;
    public List<SkillType> EnabledSkills => enabledSkills;
    public bool DisableKills => disableSkills;

    public string NextDialogID => nextDialogID;
}
