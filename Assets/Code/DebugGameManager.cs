using UnityEngine;

public class DebugGameManager : GameManager
{
    [SerializeField] private CombatSceneParameters debugSceneParameters;

    protected override void Awake()
    {
        if (!Application.isEditor)
        {
            Destroy(gameObject);
        }

        base.Awake();

        CurrentParameters = debugSceneParameters;
    }
}
