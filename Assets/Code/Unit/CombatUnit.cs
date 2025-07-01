using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VisualNovel.GameJam.Manager;

public class CombatUnit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // === Constants ===
    private const string ATTACK_TRIGGER = "Attack";

    // === Serialized Fields ===
    [SerializeField] private CombatUnitStats stats;
    [SerializeField] private Image healthFill;
    [SerializeField] private string attackSFXID;

    // === Public Properties ===
    public CombatUnitStats Stats => stats;
    public IAgent Agent { get; private set; }
    public bool IsDead => CurrentHP <= 0;
    public int Attack => Stats.Attack + AttackBuff;
    public int CurrentHP { get; private set; }

    // === Private Fields ===
    private int AttackBuff { get; set; }
    private List<ActiveEffect> ActiveEffects { get; set; }
    private bool IsEnemy => !Stats.IsPlayer && !Stats.IsCompanion;

    private Image Image { get; set; }
    private Color OriginalColour { get; set; }
    private Color CurrentColour { get; set; }
    private Animator Animator { get; set; }

    private CharacterHighlight characterHighlight;

    // === Unity Methods ===
    private void Awake()
    {
        CurrentHP = Stats.MaxHP;

        if (stats.IsPlayer && GameManager.Instance.CurrentParameters.BuffPlayer)
        {
            CurrentHP = 500;
        }

        AttackBuff = 0;
        ActiveEffects = new List<ActiveEffect>();

        Image = gameObject.GetComponent<Image>();
        OriginalColour = Image.color;

        Animator = gameObject.GetComponent<Animator>();
        Agent = GetComponent<IAgent>();

        characterHighlight = GetComponent<CharacterHighlight>();
    }

    // === Public Event Handlers ===
    public void OnClick()
    {
        CombatManager combatManager = CombatManager.Instance;

        switch (combatManager.CurrentInputMode)
        {
            case InputMode.ATTACK:
                if (IsEnemy)
                    combatManager.TakePlayerAction(this);
                break;
            case InputMode.SKILL:
                if (CanExecuteSkill(combatManager.SelectedSkill))
                    combatManager.ExecuteSkill(this);
                break;
            default:
                break;
        }
    }

    public void OnStartTurn()
    {
        ReduceActiveEffectRemainingDurations();
        characterHighlight.SetActiveTurn(true);
    }

    public void OnEndTurn()
    {
        characterHighlight.SetActiveTurn(false);
    }

    public void OnAttack()
    {
        Animator.SetTrigger(ATTACK_TRIGGER);

        SoundController.Instance.PlaySFX(0);
    }

    public void TakeDamage(int rawDamage)
    {
        int finalDamage = rawDamage - Stats.Defence;
        finalDamage = Mathf.Max(finalDamage, 0);

        CurrentHP -= finalDamage;
        CurrentHP = Mathf.Max(CurrentHP, 0);

        UpdateHealthBar();

        CombatManager.Instance.CheckForVictory();

        if (IsDead)
        {
            if (Stats.IsPlayer)
            {
                CombatManager.Instance.HandlePlayerDied();
            }

            gameObject.SetActive(false);
            return;
        }

        StartCoroutine(FlashColour(Color.red));
    }

    public void ApplyEffect(StatType stat, int amount, int duration)
    {
        switch (stat)
        {
            case StatType.HP:
                CurrentHP += amount;
                break;
            case StatType.ATK:
                AttackBuff = amount;
                break;
        }

        UpdateHealthBar();

        ActiveEffects.Add(new ActiveEffect(stat, amount, duration));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Stats.IsPlayer && !Stats.IsCompanion)
        {
            EnemyInfoUIManager.Instance.Show(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EnemyInfoUIManager.Instance.Clear();
    }

    // === Private Methods ===
    private void ReduceActiveEffectRemainingDurations()
    {
        foreach (var activeEffect in ActiveEffects)
        {
            activeEffect.ReduceRemainingDuration();

            if (activeEffect.ShouldRemove())
            {
                switch (activeEffect.Stat)
                {
                    case StatType.HP:
                        CurrentHP -= activeEffect.Amount;
                        break;
                    case StatType.ATK:
                        AttackBuff = 0;
                        break;
                }
            }
        }

        ActiveEffects.RemoveAll(effect => effect.ShouldRemove());
    }

    private bool CanExecuteSkill(Skill selectedSkill)
    {
        return (IsEnemy && selectedSkill.CanTargetEnemy) ||
               (Stats.IsCompanion && selectedSkill.CanTargetCompanion);
    }

    private IEnumerator FlashColour(Color colour)
    {
        Image.color = colour;

        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float lerpFactor = t / 0.5f;
            Image.color = Color.Lerp(colour, OriginalColour, lerpFactor);
            yield return null;
        }

        Image.color = OriginalColour;
    }

    private void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = (float)CurrentHP / Stats.MaxHP;
        }
    }

}
