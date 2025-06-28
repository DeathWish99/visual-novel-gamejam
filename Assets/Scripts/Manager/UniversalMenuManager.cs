using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VisualNovel.GameJam.UI;

namespace VisualNovel.GameJam.Manager
{
    public class UniversalMenuManager : MonoBehaviour
    {
        #region Singleton
        public static UniversalMenuManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        [Header("Top Panel")]
        [SerializeField] private GameObject[] topPanelCloseButtons;
        [SerializeField] private CanvasGroup topPanelCanvasGroup;
        [SerializeField] private Image pageIcon;
        [SerializeField] private Sprite[] pageIcons;
        [SerializeField] private TextMeshProUGUI pageTitleText;

        [Header("Panel")]
        [SerializeField] private CanvasGroup blackPanel;
        [SerializeField] private SettingPanel settingPanel;
        [SerializeField] private PausePanel pausePanel;

        #region Events
        [HideInInspector] public UnityEvent OnOpenSettingMainMenu;
        [HideInInspector] public UnityEvent OnOpenPauseMenu;

        [HideInInspector] public UnityEvent<bool> ShowBlackPanel;
        #endregion

        private void Start()
        {
            settingPanel.Init();
            OnOpenSettingMainMenu.RemoveAllListeners();
            OnOpenSettingMainMenu.AddListener(OpenSettingMainMenu);

            pausePanel.Init(ClosePauseMenu, OpenSettingPause);
            OnOpenPauseMenu.RemoveAllListeners();
            OnOpenPauseMenu.AddListener(OpenPauseMenu);

            ShowBlackPanel.RemoveAllListeners();
            ShowBlackPanel.AddListener((show) => ToggleCanvasGroupPanel(blackPanel, show));
        }

        #region Main Menu Panel
        private void OpenSettingMainMenu()
        {
            SetCurrentPageHeader(pageIcons[0], "Setting");
            ShowHeaderCloseButton(true);

            topPanelCloseButtons[0].GetComponent<Button>().onClick.RemoveAllListeners();
            topPanelCloseButtons[0].GetComponent<Button>().onClick.AddListener(CloseSettingMainMenu);

            ToggleCanvasGroupPanel(settingPanel.canvasGroup, true);
            ToggleCanvasGroupPanel(topPanelCanvasGroup, true);
        }

        private void CloseSettingMainMenu()
        {
            ToggleCanvasGroupPanel(settingPanel.canvasGroup, false);
            ToggleCanvasGroupPanel(topPanelCanvasGroup, false);
        }
        #endregion

        #region Pause Menu Panel
        private void OpenPauseMenu()
        {
            SetCurrentPageHeader(pageIcons[1], "Pause");
            ShowHeaderCloseButton(true);

            topPanelCloseButtons[0].GetComponent<Button>().onClick.RemoveAllListeners();
            topPanelCloseButtons[0].GetComponent<Button>().onClick.AddListener(ClosePauseMenu);

            ToggleCanvasGroupPanel(pausePanel.canvasGroup, true);
            ToggleCanvasGroupPanel(topPanelCanvasGroup, true);
        }

        private void ClosePauseMenu()
        {
            ToggleCanvasGroupPanel(pausePanel.canvasGroup, false);
            ToggleCanvasGroupPanel(topPanelCanvasGroup, false);
        }

        private void OpenSettingPause()
        {
            SetCurrentPageHeader(pageIcons[0], "Setting");
            ShowHeaderCloseButton(false);

            topPanelCloseButtons[1].GetComponent<Button>().onClick.RemoveAllListeners();
            topPanelCloseButtons[1].GetComponent<Button>().onClick.AddListener(CloseSettingPause);

            ToggleCanvasGroupPanel(settingPanel.canvasGroup, true);
        }

        private void CloseSettingPause()
        {
            SetCurrentPageHeader(pageIcons[1], "Pause");
            ShowHeaderCloseButton(true);

            ToggleCanvasGroupPanel(settingPanel.canvasGroup, false);
        }
        #endregion

        #region UI Functions
        private void ToggleCanvasGroupPanel(CanvasGroup canvasGroup, bool show)
        {
            canvasGroup.DOKill();
            canvasGroup.alpha = show ? 0.0f : 1.0f;
            canvasGroup.DOFade(show ? 1.0f : 0.0f, 0.5f).SetEase(Ease.OutBack);
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;
        }

        private void SetCurrentPageHeader(Sprite icon, string titleText)
        {
            pageIcon.sprite = icon;
            pageTitleText.text = titleText;
        }

        private void ShowHeaderCloseButton(bool show)
        {
            topPanelCloseButtons[0].SetActive(show);
            topPanelCloseButtons[1].SetActive(!show);
        }
        #endregion
    }
}