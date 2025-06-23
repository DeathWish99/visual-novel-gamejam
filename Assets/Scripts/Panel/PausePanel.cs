using System;
using UnityEngine;
using UnityEngine.UI;

namespace VisualNovel.GameJam.UI
{
    public class PausePanel : MonoBehaviour
    {
        [Header("Pause Panel Button")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button exitGameButton;

        public CanvasGroup canvasGroup { get; private set; }

        public void Init(Action continueGame, Action pausedMenu)
        {
            canvasGroup = GetComponent<CanvasGroup>();

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => continueGame?.Invoke());

            saveButton.onClick.RemoveAllListeners();

            loadButton.onClick.RemoveAllListeners();

            settingButton.onClick.RemoveAllListeners();
            settingButton.onClick.AddListener(() => pausedMenu?.Invoke());

            exitGameButton.onClick.RemoveAllListeners();
            exitGameButton.onClick.AddListener(() => Application.Quit());
        }
    }
}