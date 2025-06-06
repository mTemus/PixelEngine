using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PixelEngine.Utility.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelEngine.Core.SceneManagement
{
    public class SingleSceneManager
    {
        public event Action<SceneData> OnSceneLoaded = delegate {  };
        public event Action<SceneData> OnScenePreUnloaded = delegate {  };
        public event Action<SceneData> OnSceneUnloaded = delegate {  };
           
        private readonly List<SceneData> m_activeScenes = new List<SceneData>();

        private readonly int m_sceneMillisecondsDelay = 100;

        public SingleSceneManager(int sceneMillisecondsDelay)
        {
            m_sceneMillisecondsDelay = sceneMillisecondsDelay;
        }

        public async void LoadScene(SceneData sceneData)
        {
            if (m_activeScenes.Contains(sceneData))
            {
                Debug.LogError($"SingleSceneManager --- Scene {sceneData.Scene.Name} is already loaded!");
                return;
            }
            
            var operation = SceneManager.LoadSceneAsync(sceneData.Scene.Path, LoadSceneMode.Additive);
            
            while (!operation.isDone)
                await Task.Delay(100); 
            
            m_activeScenes.Add(sceneData);
            OnSceneLoaded.Invoke(sceneData);
        }
        
        public async void LoadScenes(List<SceneData> scenes)
        {
            var scenesToLoad = scenes.Count;
            var operationGroup = new AsyncOperationGroup(scenesToLoad);

            for (var i = 0; i < scenesToLoad; i++)
            {
                var sceneData = scenes[i];
                
                if(m_activeScenes.Contains(sceneData))
                {
                    Debug.LogError($"SingleSceneManager --- Scene {sceneData.Scene.Name} is already loaded!");
                    continue;
                }
                
                var operation = SceneManager.LoadSceneAsync(sceneData.Scene.Path, LoadSceneMode.Additive);
                
                operationGroup.Operations.Add(operation);
            }
            
            while (!operationGroup.IsDone)
                await Task.Delay(m_sceneMillisecondsDelay); 
            
            for (var i = 0; i < scenesToLoad; i++)
            {
                var sceneData = scenes[i];
                
                m_activeScenes.Add(sceneData);
                OnSceneLoaded.Invoke(scenes[i]);
            }
        }

        public async void UnloadScenes(List<SceneData> scenes)
        {
            var scenesToUnload = scenes.Count;
            var operationGroup = new AsyncOperationGroup(scenesToUnload);

            for (var i = 0; i < scenesToUnload; i++)
            {
                var sceneData = scenes[i];
                
                if(!m_activeScenes.Contains(sceneData))
                {
                    Debug.LogError($"SingleSceneManager --- Scene {sceneData.Scene.Name} is not loaded!");
                    continue;
                }

                OnScenePreUnloaded.Invoke(sceneData);
                
                var operation = SceneManager.UnloadSceneAsync(sceneData.Scene.Path);
                operationGroup.Operations.Add(operation);
            }
            
            while (!operationGroup.IsDone)
                await Task.Delay(m_sceneMillisecondsDelay); 
            
            for (var i = 0; i < scenesToUnload; i++)
            {
                var sceneData = scenes[i];
                
                if(!m_activeScenes.Contains(sceneData))
                    continue;
                
                m_activeScenes.Remove(sceneData);
                OnSceneUnloaded.Invoke(scenes[i]);
            }
        } 
    }
}