using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VisualNovel.GameJam.Manager
{
    public class SceneLoaderManager : MonoBehaviour
    {
        public static SceneLoaderManager Instance { get; private set; }

        public bool IsLoadingScene { get; private set; } = false;

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

        public void LoadNewScene(string sceneName)
        {
            if (IsSceneLoaded(sceneName))
            {
                Debug.LogWarning($"Scene '{sceneName}' is already loaded.");
                return;
            }

            StartCoroutine(LoadNewSceneRoutine(sceneName));
        }

        private IEnumerator LoadNewSceneRoutine(string sceneName)
        {
            IsLoadingScene = true;
            UniversalMenuManager.Instance.ShowBlackPanel?.Invoke(true);
            yield return new WaitForSeconds(0.5f);

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            yield return loadOp;

            yield return new WaitForSeconds(0.5f);
            UniversalMenuManager.Instance.ShowBlackPanel?.Invoke(false);
            IsLoadingScene = false;
        }

        public void LoadSceneAdditive(string sceneName)
        {
            if (IsSceneLoaded(sceneName))
            {
                Debug.LogWarning($"Scene '{sceneName}' is already loaded.");
                return;
            }

            StartCoroutine(LoadSceneAdditiveRoutine(sceneName));
        }

        private IEnumerator LoadSceneAdditiveRoutine(string sceneName)
        {
            IsLoadingScene = true;
            UniversalMenuManager.Instance.ShowBlackPanel?.Invoke(true);
            yield return new WaitForSeconds(0.5f);

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return loadOp;

            yield return new WaitForSeconds(0.5f);
            UniversalMenuManager.Instance.ShowBlackPanel?.Invoke(false);
            IsLoadingScene = false;
        }

        private bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.name == sceneName) return true;
            }

            return false;
        }
    }
}