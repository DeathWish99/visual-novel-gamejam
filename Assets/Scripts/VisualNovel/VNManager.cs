using Fungus;
using System.Collections.Generic;
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

        [Header("VN Data")]
        [SerializeField] private List<Flowchart> flowcharts;

        [Header("VN Button")]
        [SerializeField] private Button menuButton;

        private void Start()
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(() => UniversalMenuManager.Instance.OnOpenPauseMenu?.Invoke());

            StartDialogue("Prologue");
        }

        public void StartDialogue(string blockName)
        {
            Debug.Log("Find Dialog...");

            foreach (Flowchart fc in flowcharts)
            {
                if (fc == null) continue;

                Block block = fc.FindBlock(blockName);

                if (block != null)
                {
                    Debug.Log("Play Dialog...");
                    fc.ExecuteBlock(block);
                    return;
                }
            }
        }

        #region Auto & Skip
        private bool _isAutoMode;
        public bool IsAutoMode => _isAutoMode;

        private bool _isSkipMode;
        public bool IsSkipMode => _isSkipMode;

        public void ToggleAutoMode()
        {
            _isAutoMode = !IsAutoMode;
            Debug.Log("Auto = " + _isAutoMode);
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