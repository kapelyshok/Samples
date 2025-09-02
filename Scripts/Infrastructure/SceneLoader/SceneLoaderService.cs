using System;
using AtomicApps.Infrastructure.Services.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AtomicApps.Infrastructure
{
    public class SceneLoaderService : MonoBehaviour
    {
        [SerializeField]
        private LoadingCurtain loadingCurtain;
        
        public async UniTask LoadScene(string sceneName, bool showCurtain = true, Action onLoadComplete = null)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                return;
            }

            if (showCurtain)
            {
                loadingCurtain.Show();
                await UniTask.WaitForSeconds(3f);
            }
            
            await SceneManager.LoadSceneAsync(sceneName);
            onLoadComplete?.Invoke();
            
            if (showCurtain)
            {
                await UniTask.WaitForSeconds(.5f);
                loadingCurtain.Hide();
            }
        }
        
        public async UniTask LoadScene(SceneName sceneName, bool showCurtain = true, Action onLoadComplete = null)
        {
            /*if (SceneManager.GetActiveScene().name == sceneName.ToString())
            {
                Debug.LogWarning($"Can't load scene {sceneName} because it is currently loaded!");
                return;
            }*/
            
            if (showCurtain)
            {
                loadingCurtain.Show();
                await UniTask.WaitForSeconds(.5f);
            }

            await SceneManager.LoadSceneAsync(sceneName.ToString());
            onLoadComplete?.Invoke();
            
            if (showCurtain)
            {
                await UniTask.WaitForSeconds(.5f);
                loadingCurtain.Hide();
            }
        }
    }
}