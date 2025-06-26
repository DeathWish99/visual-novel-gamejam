using Fungus;
using UnityEngine;

[CommandInfo("Custom", "Combat Event", "Combat")]
[AddComponentMenu("")]
public class CombatCommand : Command
{
    [SerializeField] protected CombatSceneParameters parameters;

    public override void OnEnter()
    {
        GameManager.Instance.LoadCombatScene(parameters);
        Continue();
    }
}
