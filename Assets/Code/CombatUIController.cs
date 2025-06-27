using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIController : MonoBehaviour
{
    [SerializeField] private List<SkillButton> skillButtons;

    private int cooldownTimer = 0;

    private void OnEnable()
    {
        CombatManager.OnTurnChanged += HandleTurnChanged;
        CombatManager.OnSkillUsed += HandleSkillUsed;
    }

    private void OnDisable()
    {
        CombatManager.OnTurnChanged -= HandleTurnChanged;
        CombatManager.OnSkillUsed -= HandleSkillUsed;
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
