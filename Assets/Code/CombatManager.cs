using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VisualNovel.GameJam.Manager;

public class CombatManager : MonoBehaviour
{
    // --------------------- Serialized Fields ---------------------
    [SerializeField] private UnitSpawner unitSpawner;

    [Header("Turn Order UI")]
    [SerializeField] private RectTransform turnOrderBar;
    [SerializeField] private GameObject turnOrderIconPrefab;

    // --------------------- Properties ---------------------
    public static CombatManager Instance { get; private set; }
    public InputMode CurrentInputMode { get; private set; }
    public Skill SelectedSkill { get; private set; }

    // --------------------- Public Fields ---------------------
    public static event System.Action<CombatUnit> OnTurnChanged;
    public static event System.Action OnSkillUsed;

    // --------------------- Private Fields ---------------------
    private Dictionary<CombatUnit, GameObject> UnitIcons { get; set; }
    private Queue<CombatUnit> TurnQueue { get; set; }
    private List<CombatUnit> Units { get; set; }
    private List<CombatUnit> PlayerUnits { get; set; }
    private List<CombatUnit> EnemyUnits { get; set; }
    private List<CombatUnit> IconDisplayOrder { get; set; }
    private CombatUnit PlayerUnit { get; set; }
    private SkillExecutor SkillExecutor { get; set; }

    // --------------------- Unity Lifecycle ---------------------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetUp();
        SetUpUnits();
        SetUpTurnOrder();
        StartNextTurn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UniversalMenuManager.Instance.OnOpenPauseMenu?.Invoke();
        }
    }

    // --------------------- Public Methods ---------------------
    public void OnSkillButtonClick(Skill skill)
    {
        CurrentInputMode = InputMode.SKILL;
        SelectedSkill = skill;
    }

    public void ExecuteSkill(CombatUnit target)
    {
        CurrentInputMode = InputMode.ATTACK;

        SkillExecutor.Execute(SelectedSkill, PlayerUnit, target);
        OnSkillUsed?.Invoke();
        Invoke(nameof(StartNextTurn), 1.0f);
    }

    public void TakePlayerAction(CombatUnit target)
    {
        PlayerUnit.OnAttack();

        int damage = PlayerUnit.Attack;

        target.TakeDamage(damage);
        Invoke(nameof(StartNextTurn), 1.0f);
    }

    public List<CombatUnit> GetTargets(CombatUnit targeter)
    {
        return PlayerUnits.Contains(targeter) ? EnemyUnits : PlayerUnits;
    }

    // --------------------- Setup Methods ---------------------
    private void SetUp()
    {
        UnitIcons = new();
        IconDisplayOrder = new();
        Units = new List<CombatUnit>();
        PlayerUnits = new List<CombatUnit>();
        EnemyUnits = new List<CombatUnit>();
        SkillExecutor = GetComponent<SkillExecutor>();
        TurnQueue = new Queue<CombatUnit>();
    }

    private void SetUpUnits()
    {
        SpawnPlayerUnits();
        SpawnEnemies();
        CreateTurnOrderIcons();
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
            if (!unit.IsDead)
                TurnQueue.Enqueue(unit);
        }
    }

    private void SpawnPlayerUnits()
    {
        bool hasCompanion = GameManager.Instance.CurrentParameters.HasCompanion;

        // Spawn player
        PlayerUnit = unitSpawner.SpawnPlayer();

        Units.Add(PlayerUnit);
        PlayerUnits.Add(PlayerUnit);

        // Spawn companion
        if (hasCompanion)
        {
            CombatUnit companionUnit = unitSpawner.SpawnCompanion();

            Units.Add(companionUnit);
            PlayerUnits.Add(companionUnit);
            IconDisplayOrder.Add(companionUnit);
        }

        IconDisplayOrder.Add(PlayerUnit);
    }

    private void SpawnEnemies()
    {
        List<EnemyType> enemyTypes = GameManager.Instance.CurrentParameters.Enemies;
        List<CombatUnit> enemies = unitSpawner.SpawnEnemies(enemyTypes);

        foreach (var enemy in enemies)
        {
            Units.Add(enemy);
            EnemyUnits.Add(enemy);
            IconDisplayOrder.Add(enemy);
        }
    }

    // --------------------- Turn Logic ---------------------
    private void StartNextTurn()
    {
        if (HasBattleEnded()) return;

        if (TurnQueue.Count == 0)
        {
            SetUpTurnOrder();
            StartNextTurn();
            return;
        }

        foreach (var unit in Units)
        {
            unit.OnEndTurn();
        }

        CombatUnit currentUnit = TurnQueue.Dequeue();

        OnTurnChanged?.Invoke(currentUnit);

        if (currentUnit.IsDead)
        {
            Debug.Log($"{currentUnit.Stats.name} is dead. Going to next turn...");
            UpdateIconStates();
            StartNextTurn();
            return;
        }

        UpdateIconStates();
        HighlightCurrentUnitIcon(currentUnit);

        if (currentUnit.Stats.IsPlayer) return;

        currentUnit.OnStartTurn();
        StartCoroutine(currentUnit.Agent.TakeTurn(currentUnit, StartNextTurn));
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

    // --------------------- UI Helpers ---------------------
    private void CreateTurnOrderIcons()
    {
        foreach (CombatUnit unit in IconDisplayOrder)
        {
            GameObject icon = Instantiate(turnOrderIconPrefab, turnOrderBar);

            Image iconImage = icon.GetComponent<Image>();

            if (iconImage != null && unit.Stats.Icon)
            {
                iconImage.sprite = unit.Stats.Icon;
            }

            UnitIcons.Add(unit, icon);
        }
    }

    private void HighlightCurrentUnitIcon(CombatUnit currentUnit)
    {
        foreach (var pair in UnitIcons)
        {
            bool isCurrent = pair.Key == currentUnit;

            Vector3 targetScale = isCurrent ? Vector3.one * 1.3f : Vector3.one;

            StartCoroutine(ScaleIcon(pair.Value, targetScale, 0.3f));
        }
    }

    private IEnumerator ScaleIcon(GameObject icon, Vector3 targetScale, float duration)
    {
        Vector3 startScale = icon.transform.localScale;
        float time = 0;

        while (time < duration)
        {
            icon.transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        icon.transform.localScale = targetScale;
    }

    private void UpdateIconStates()
    {
        foreach (var pair in UnitIcons)
        {
            var unit = pair.Key;
            var icon = pair.Value;
            var canvasGroup = icon.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = unit.IsDead ? 0.3f : 1f;
                icon.GetComponent<Image>().color = unit.IsDead 
                    ? new Color(1f, 1f, 1f, 0.3f)
                    : Color.white;
            }
        }
    }
}
