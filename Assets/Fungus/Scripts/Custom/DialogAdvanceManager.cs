using Fungus;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogAdvanceManager : MonoBehaviour
{
    [SerializeField] private float autoDelay = 1.5f;

    private SayDialog sayDialog;
    private MenuDialog menuDialog;

    private Button continueButton;
    private Coroutine autoCoroutine;

    private void Start()
    {
        sayDialog = SayDialog.GetSayDialog();
        menuDialog = MenuDialog.GetMenuDialog();

        continueButton = sayDialog.ContinueButton;
    }

    private void Update()
    {
        if ((VNManager.Instance.IsAutoMode || VNManager.Instance.IsSkipMode) && menuDialog != null && menuDialog.gameObject.activeInHierarchy)
        {
            VNManager.Instance.TurnOffAll();
        }

        if ((VNManager.Instance.IsAutoMode || VNManager.Instance.IsSkipMode) &&
            continueButton != null && continueButton.gameObject.activeInHierarchy &&
            autoCoroutine == null)
        {
            float delay = VNManager.Instance.IsSkipMode ? 0.1f : autoDelay;
            autoCoroutine = StartCoroutine(AutoClickAfterDelay(delay));
        }

        if (!VNManager.Instance.IsAutoMode && !VNManager.Instance.IsSkipMode && autoCoroutine != null)
        {
            StopCoroutine(autoCoroutine);
            autoCoroutine = null;
        }
    }

    private IEnumerator AutoClickAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (continueButton != null && continueButton.gameObject.activeInHierarchy)
        {
            continueButton.onClick.Invoke();
        }

        autoCoroutine = null;
    }
}
