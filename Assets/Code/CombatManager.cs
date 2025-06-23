using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject companionPrefab;
    [SerializeField] private List<EnemyPrefabPair> enemyPrefabs;
    [SerializeField] private RectTransform playerPosition;
    [SerializeField] private RectTransform companionPosition;
    [SerializeField] private List<RectTransform> enemyPositions;

    public static CombatManager Instance { get; private set; }
    private Dictionary<EnemyType, GameObject> PrefabMap { get; set; }
    private Queue<CombatUnit> TurnQueue { get; set; }
    private List<CombatUnit> Units { get; set; }
    private List<CombatUnit> PlayerUnits { get; set; }
    private List<CombatUnit> EnemyUnits { get; set; }
    private CombatUnit PlayerUnit { get; set; }
    private int SetupCount { get; set; }
    private bool HasStarted { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SetUp();
        SetUpUnits();
        SetUpTurnOrder();
        StartNextTurn();
    }

    public void TakePlayerAction(CombatUnit target)
    {
        int damage = PlayerUnit.Stats.CalculateDamage(target.Stats.Defence);

        target.TakeDamage(damage);

        Invoke(nameof(StartNextTurn), 1.0f);
    }

    public void OnPlayerDied()
    {

    }

    private void SetUp()
    {
        SetUpPrefabMap();

        TurnQueue = new Queue<CombatUnit>();
    }

    private void SetUpPrefabMap()
    {
        PrefabMap = new Dictionary<EnemyType, GameObject>();

        foreach (var pair in enemyPrefabs)
        {
            if (!PrefabMap.ContainsKey(pair.type)) PrefabMap.Add(pair.type, pair.prefab);
        }
    }

    private void SetUpTurnOrder()
    {
        Units.Sort((a, b) =>
        {
            if (a.Stats.Speed == b.Stats.Speed)
            {
                if (a.Stats.IsPlayer) return -1;
                if (b.Stats.IsPlayer) return 1;
                return 0;
            }
            return b.Stats.Speed.CompareTo(a.Stats.Speed);
        });

        foreach (var unit in Units)
        {
            if (!unit.IsDead) TurnQueue.Enqueue(unit);
        }
    }

    private void SetUpUnits()
    {
        CombatSceneParameters parameters = GameManager.Instance.CurrentParameters;
        Units = new List<CombatUnit>();
        EnemyUnits = new List<CombatUnit>();
        PlayerUnits = new List<CombatUnit>();
        PlayerUnit = Instantiate(playerPrefab, playerPosition).GetComponent<CombatUnit>();

        Units.Add(PlayerUnit);
        PlayerUnits.Add(PlayerUnit);

        if (parameters.HasCompanion)
        {
            CombatUnit companionUnit = Instantiate(companionPrefab, companionPosition).GetComponent<CombatUnit>();

            Units.Add(companionUnit);
            PlayerUnits.Add(companionUnit);
        }

        int enemyPositionCounter = 0;

        foreach (var enemyType in parameters.Enemies)
        {
            if (PrefabMap.TryGetValue(enemyType, out GameObject prefab))
            {
                CombatUnit enemyUnit = Instantiate(prefab, enemyPositions[enemyPositionCounter]).GetComponent<CombatUnit>();

                Units.Add(enemyUnit);
                EnemyUnits.Add(enemyUnit);

                enemyPositionCounter++;
            }
        }
    }

    private void StartNextTurn()
    {
        if (HasBattleEnded())
        {
            // Invoke(nameof(ReloadScene), 1.0f);
            return;
        }

        if (TurnQueue.Count == 0)
        {
            SetUpTurnOrder();
            StartNextTurn();
            return;
        }

        CombatUnit currentUnit = TurnQueue.Dequeue();
        Debug.Log($"Current Turn: {currentUnit.Stats.UnitName}");

        if (currentUnit.Stats.IsPlayer)
        {
            return;
        }

        if (currentUnit.IsDead)
        {
            Debug.Log($"{currentUnit.Stats.name} is dead. Going to next turn...");
            StartNextTurn();
            return;
        }

        TakeAction(currentUnit);
    }

    private void TakeAction(CombatUnit currentUnit)
    {
        CombatUnit target = PickTarget(currentUnit);

        if (target == null)
        {
            StartNextTurn();
            return;
        }

        int damage = currentUnit.Stats.CalculateDamage(target.Stats.Defence);

        target.TakeDamage(damage);

        Invoke(nameof(StartNextTurn), 1.0f);
    }

    private CombatUnit PickTarget(CombatUnit currentUnit)
    {
        List<CombatUnit> targets = PlayerUnits.Contains(currentUnit) ? EnemyUnits : PlayerUnits;

        foreach (var target in targets)
        {
            if (!target.IsDead) return target;
        }

        return null;
    }

    private bool HasBattleEnded()
    {
        bool isPlayerDead = PlayerUnit.IsDead;
        bool areAllEnemiesDead = EnemyUnits.TrueForAll(unit => unit.IsDead);

        if (isPlayerDead)
        {
            Debug.Log("You are dead.");
            Invoke(nameof(ReloadScene), 3.0f);
            return true;
        }

        if (areAllEnemiesDead)
        {
            Debug.Log("You won.");
            return true;
        }

        return false;
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene("CombatScene");
    }
}
