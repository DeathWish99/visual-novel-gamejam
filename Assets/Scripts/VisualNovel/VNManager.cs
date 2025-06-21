using UnityEngine;
using UnityEngine.UI;

namespace VisualNovel.GameJam.Manager
{
    public class VNManager : MonoBehaviour
    {
        #region Singleton
        public static VNManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        #endregion

        [Header("VN Button")]
        [SerializeField] private Button menuButton;

        private void Start()
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(() => UniversalMenuManager.Instance.OnOpenPauseMenu?.Invoke());
        }

        #region Auto & Skip
        private bool _isAutoMode;
        public bool IsAutoMode => _isAutoMode;

        private bool _isSkipMode;
        public bool IsSkipMode => _isSkipMode;

        public void ToggleAutoMode()
        {
            _isAutoMode = !IsAutoMode;
        }

        public void ToggleSkipMode()
        {
            _isSkipMode = !IsSkipMode;
        }

        public void TurnOffAll()
        {
            _isAutoMode = false;
            _isSkipMode = false;
        }
        #endregion
    }
}