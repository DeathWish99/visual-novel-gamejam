using UnityEngine;
public class EnemyInfoUIManager : MonoBehaviour
{
    public static EnemyInfoUIManager Instance { get; private set; }
    [SerializeField] private EnemyInfoUI enemyInfoUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Show(CombatUnit unit) => enemyInfoUI.Show(unit);
    public void Clear() => enemyInfoUI.Clear();
}