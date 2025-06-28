using Fungus;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace VisualNovel.GameJam.Manager
{
    public class DialogAdvanceManager : MonoBehaviour
    {
        [SerializeField] private float autoDelay = 1.5f;
        [SerializeField] private CanvasGroup log;

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
            if ((VNManager.Instance.IsAutoMode || VNManager.Instance.IsSkipMode) && menuDialog != null && log.alpha == 1)
            {
                //Debug.Log("1");
                VNManager.Instance.TurnOffAll();
            }

            if ((VNManager.Instance.IsAutoMode || VNManager.Instance.IsSkipMode) &&
                continueButton != null && continueButton.gameObject.activeInHierarchy &&
                autoCoroutine == null)
            {
                //Debug.Log("2");
                float delay = VNManager.Instance.IsSkipMode ? 0.1f : autoDelay;
                autoCoroutine = StartCoroutine(AutoClickAfterDelay(delay));
            }

            if (!VNManager.Instance.IsAutoMode && !VNManager.Instance.IsSkipMode && autoCoroutine != null)
            {
                //Debug.Log("3");
                StopCoroutine(autoCoroutine);
                autoCoroutine = null;
            }
        }

        private IEnumerator AutoClickAfterDelay(float delay)
        {
            Debug.Log("Auto");
            yield return new WaitForSeconds(delay);

            if (continueButton != null && continueButton.gameObject.activeInHierarchy)
            {
                continueButton.onClick.Invoke();
            }

            autoCoroutine = null;
        }
    }
}