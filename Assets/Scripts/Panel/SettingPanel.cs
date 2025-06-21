using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VisualNovel.GameJam.Manager;

namespace VisualNovel.GameJam.UI
{
    public class SettingPanel : MonoBehaviour
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private TextMeshProUGUI masterText;

        [SerializeField] private Slider musicSlider;
        [SerializeField] private TextMeshProUGUI musicText;

        [SerializeField] private Slider sfxSlider;
        [SerializeField] private TextMeshProUGUI sfxText;

        public CanvasGroup canvasGroup { get; private set; }

        public void Init()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            masterSlider.value = AudioManager.Instance.GetMasterVolume();
            masterText.text = Mathf.RoundToInt(masterSlider.value * 100f).ToString();
            masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);

            musicSlider.value = AudioManager.Instance.GetMusicVolume();
            musicText.text = Mathf.RoundToInt(musicSlider.value * 100f).ToString();
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

            sfxSlider.value = AudioManager.Instance.GetSFXVolume();
            sfxText.text = Mathf.RoundToInt(sfxSlider.value * 100f).ToString();
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }

        private void OnMasterSliderChanged(float value)
        {
            AudioManager.Instance.SetMasterVolume(value);
            masterText.text = Mathf.RoundToInt(value * 100f).ToString();
        }

        private void OnMusicSliderChanged(float value)
        {
            AudioManager.Instance.SetMusicVolume(value);
            musicText.text = Mathf.RoundToInt(value * 100f).ToString();
        }

        private void OnSFXSliderChanged(float value)
        {
            AudioManager.Instance.SetSFXVolume(value);
            sfxText.text = Mathf.RoundToInt(value * 100f).ToString();
        }
    }
}