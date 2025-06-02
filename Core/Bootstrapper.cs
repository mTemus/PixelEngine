using PixelEngine.Extensions;
using PixelEngine.Utility.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelEngine.Core
{
    public class Bootstrapper : MonoSingleton<Bootstrapper>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void Init()
        {
            Debug.Log("Bootstrapping game...");
            
            await SceneManager.LoadSceneAsync("Core", LoadSceneMode.Single).AsTask();
        }
    }
}
