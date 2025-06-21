using UnityEngine;
using UnityEngine.UI;

namespace VisualNovel.GameJam.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Main Menu Button")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button exitGameButton;

        private void Start()
        {
            InitializeMainMenuButtons();
        }

        private void InitializeMainMenuButtons()
        {
            newGameButton.onClick.RemoveAllListeners();

            loadButton.onClick.RemoveAllListeners();

            settingButton.onClick.RemoveAllListeners();
            settingButton.onClick.AddListener(() => UniversalMenuManager.Instance.OnOpenSettingMainMenu?.Invoke());

            exitGameButton.onClick.RemoveAllListeners();
            exitGameButton.onClick.AddListener(() => Application.Quit());
        }
    }
}