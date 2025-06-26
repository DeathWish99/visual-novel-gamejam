using UnityEngine;
using VisualNovel.GameJam.Manager;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    [SerializeField] private string combatSceneName;

    public static GameManager Instance { get; private set; }
    public CombatSceneParameters CurrentParameters { get; protected set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadCombatScene(CombatSceneParameters parameters)
    {
        CurrentParameters = parameters;
        SceneLoaderManager.Instance.LoadSceneAdditive(combatSceneName);
    }
}
