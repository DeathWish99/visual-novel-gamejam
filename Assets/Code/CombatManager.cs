using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VisualNovel.GameJam.Manager;

public class CombatManager : MonoBehaviour
{
    // --------------------- Serialized Fields ---------------------
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject companionPrefab;
    [SerializeField] private List<EnemyPrefabPair> enemyPrefabs;

    [Header("Positions")]
    [SerializeField] private RectTransform playerPosition;
    [SerializeField] private RectTransform companionPosition;
    [SerializeField] private List<RectTransform> enemyPositions;

    [Header("UI")]
    [SerializeField] private List<SkillButton> skillButtons;

    [Header("Turn Order UI")]
    [SerializeField] private RectTransform turnOrderBar;
    [SerializeField] private GameObject turnOrderIconPrefab;

    // --------------------- Properties ---------------------
    public static CombatManager Instance { get; private set; }
    public InputMode CurrentInputMode { get; private set; }
    public Skill SelectedSkill { get; private set; }
    private List<SkillButton> SkillButtons => skillButtons;

    // --------------------- Private Fields ---------------------
    private Dictionary<EnemyType, GameObject> PrefabMap { get; set; }
    private Dictionary<CombatUnit, GameObject> UnitIcons { get; set; }
    private Queue<CombatUnit> TurnQueue { get; set; }
    private List<CombatUnit> Units { get; set; }
    private List<CombatUnit> PlayerUnits { get; set; }
    private List<CombatUnit> EnemyUnits { get; set; }
    private List<CombatUnit> IconDisplayOrder { get; set; }
    private CombatUnit PlayerUnit { get; set; }
    private SkillExecutor SkillExecutor { get; set; }
    private int SkillButtonTimer { get; set; }

    // --------------------- Unity Lifecycle ---------------------
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
        DisableSkillButtons();
        CurrentInputMode = InputMode.SKILL;
        SelectedSkill = skill;
    }

    public void ExecuteSkill(CombatUnit target)
    {
        CurrentInputMode = InputMode.ATTACK;
        SkillExecutor.Execute(SelectedSkill, PlayerUnit, target);
        SkillButtonTimer = 2;
        Invoke(nameof(StartNextTurn), 1.0f);
    }

    public void TakePlayerAction(CombatUnit target)
    {
        PlayerUnit.OnAttack();
        int damage = PlayerUnit.Attack;
        target.TakeDamage(damage);
        Invoke(nameof(StartNextTurn), 1.0f);
    }

    public void OnPlayerDied()
    {
        // Placeholder
    }

    // --------------------- Setup Methods ---------------------
    private void SetUp()
    {
        UnitIcons = new();
        IconDisplayOrder = new();
        SkillExecutor = GetComponent<SkillExecutor>();
        SetUpPrefabMap();
        TurnQueue = new Queue<CombatUnit>();
    }

    private void SetUpPrefabMap()
    {
        PrefabMap = new Dictionary<EnemyType, GameObject>();

        foreach (var pair in enemyPrefabs)
        {
            if (!PrefabMap.ContainsKey(pair.type))
                PrefabMap.Add(pair.type, pair.prefab);
        }
    }

    private void SetUpUnits()
    {
        CombatSceneParameters parameters = GameManager.Instance.CurrentParameters;

        Units = new List<CombatUnit>();
        PlayerUnits = new List<CombatUnit>();
        EnemyUnits = new List<CombatUnit>();

        PlayerUnit = Instantiate(playerPrefab, playerPosition).GetComponent<CombatUnit>();
        Units.Add(PlayerUnit);
        PlayerUnits.Add(PlayerUnit);

        if (parameters.HasCompanion)
        {
            CombatUnit companionUnit = Instantiate(companionPrefab, companionPosition).GetComponent<CombatUnit>();
            Units.Add(companionUnit);
            PlayerUnits.Add(companionUnit);
            IconDisplayOrder.Add(companionUnit);
        }

        IconDisplayOrder.Add(PlayerUnit);

        int enemyPositionCounter = 0;
        foreach (var enemyType in parameters.Enemies)
        {
            if (PrefabMap.TryGetValue(enemyType, out GameObject prefab))
            {
                CombatUnit enemyUnit = Instantiate(prefab, enemyPositions[enemyPositionCounter]).GetComponent<CombatUnit>();
                Units.Add(enemyUnit);
                EnemyUnits.Add(enemyUnit);
                IconDisplayOrder.Add(enemyUnit);
                enemyPositionCounter++;
            }
        }

        foreach (CombatUnit unit in IconDisplayOrder)
        {
            GameObject icon = Instantiate(turnOrderIconPrefab, turnOrderBar);

            Image iconImage = icon.GetComponent<Image>();

            if (iconImage != null && unit.Stats.Icon)
            {
                iconImage.sprite = unit.Stats.Icon.sprite;
            }

            UnitIcons.Add(unit, icon);
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
            if (!unit.IsDead)
                TurnQueue.Enqueue(unit);
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
        Debug.Log($"Current Turn: {currentUnit.Stats.UnitName}");

        if (currentUnit.IsDead)
        {
            Debug.Log($"{currentUnit.Stats.name} is dead. Going to next turn...");
            UpdateIconStates();
            StartNextTurn();
            return;
        }

        UpdateIconStates();
        HighlightCurrentUnitIcon(currentUnit);

        currentUnit.OnStartTurn();

        if (currentUnit.Stats.IsPlayer)
        {
            if (SkillButtonTimer > 0)
                SkillButtonTimer--;

            if (SkillButtonTimer <= 0)
                EnableSkillButtons();

            return;
        }

        DisableSkillButtons();
        StartCoroutine(InvokeAIAction(currentUnit, 1.0f));
    }

    private IEnumerator InvokeAIAction(CombatUnit currentUnit, float delay)
    {
        yield return new WaitForSeconds(delay);
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

        currentUnit.OnAttack();
        target.TakeDamage(currentUnit.Attack);
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

    // --------------------- UI Helpers ---------------------
    private void DisableSkillButtons()
    {
        foreach (var button in SkillButtons)
        {
            button.Disable();
        }
    }

    private void EnableSkillButtons()
    {
        foreach (var button in SkillButtons)
        {
            button.Enable();
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
