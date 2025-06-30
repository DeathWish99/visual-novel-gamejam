using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIController : MonoBehaviour
{
    [SerializeField] private List<SkillButton> skillButtons;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    private int cooldownTimer = 0;

    private void Awake()
    {
        CombatManager.OnTurnChanged += HandleTurnChanged;
        CombatManager.OnSkillUsed += HandleSkillUsed;
        CombatManager.OnPlayerDied += HandlePlayerDied;
        CombatManager.DisableSkills += HandleDisableSkills;
        CombatManager.OnVictory += HandleVictory;
    }

    private void OnDisable()
    {
        CombatManager.OnTurnChanged -= HandleTurnChanged;
        CombatManager.OnSkillUsed -= HandleSkillUsed;
        CombatManager.OnPlayerDied -= HandlePlayerDied;
        CombatManager.DisableSkills -= HandleDisableSkills;
        CombatManager.OnVictory -= HandleVictory;
    }

    private void HandleTurnChanged(CombatUnit unit)
    {
        if (unit.Stats.IsPlayer)
        {
            if (PlayerCanUseSkill())
            {
                EnableButtons();
            }
        }
        else
        {
            DisableButtons();
        }
    }

    private void HandleSkillUsed()
    {
        cooldownTimer = 2;

        DisableButtons();
    }

    private void HandlePlayerDied()
    {
        gameOverPanel.SetActive(true);
        CombatManager.Instance.ReloadScene();
    }

    private void HandleDisableSkills()
    {
        CombatManager.OnTurnChanged -= HandleTurnChanged;
        CombatManager.OnSkillUsed -= HandleSkillUsed;

        foreach (var button in skillButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void HandleVictory()
    {
        victoryPanel.SetActive(true);
    }

    private bool PlayerCanUseSkill()
    {
        if (cooldownTimer-- <= 0)
        {
            return true;
        }

        return false;
    }

    private void EnableButtons()
    {
        foreach (var button in skillButtons)
            button.Enable();
    }

    private void DisableButtons()
    {
        foreach (var button in skillButtons)
            button.Disable();
    }
}
