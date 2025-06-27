using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIController : MonoBehaviour
{
    [SerializeField] private List<SkillButton> skillButtons;
    [SerializeField] private GameObject gameOverPanel;

    private int cooldownTimer = 0;

    private void OnEnable()
    {
        CombatManager.OnTurnChanged += HandleTurnChanged;
        CombatManager.OnSkillUsed += HandleSkillUsed;
        CombatManager.OnPlayerDied += HandlePlayerDied;
        CombatManager.DisableSkills += HandleDisableSkills;
    }

    private void OnDisable()
    {
        CombatManager.OnTurnChanged -= HandleTurnChanged;
        CombatManager.OnSkillUsed -= HandleSkillUsed;
        CombatManager.OnPlayerDied -= HandlePlayerDied;
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
