using UnityEngine;

public class DebugGameManager : GameManager
{
    [SerializeField] private CombatSceneParameters debugSceneParameters;

    protected override void Awake()
    {
        base.Awake();

        CurrentParameters = debugSceneParameters;
    }
}
