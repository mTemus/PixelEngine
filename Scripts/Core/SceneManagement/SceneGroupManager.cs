using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelEngine.Core.SceneManagement
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded = delegate {  };
        public event Action<string> OnSceneUnloaded = delegate {  };
        public event Action OnSceneGroupLoaded = delegate {  };
        
        private SceneGroup m_activeSceneGroup;
        
        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDuplicates = false)
        {
            m_activeSceneGroup = group;

            var loadedScenes = new List<string>();

            await UnloadScenes();
            
            var sceneCount = SceneManager.loadedSceneCount;

            for (var i = 0; i < sceneCount; i++)
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);

            var totalScenesToLoad = m_activeSceneGroup.Scenes.Count;
            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (int i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = group.Scenes[i];
                
                if (reloadDuplicates == false && loadedScenes.Contains(sceneData.Name))
                    continue;
                
                var operation = SceneManager.LoadSceneAsync(sceneData.Scene.Path, LoadSceneMode.Additive);
                
                operationGroup.Operations.Add(operation);
                OnSceneLoaded.Invoke(sceneData.Name);
            }

            while (!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.Progress);
                await Task.Delay(100); 
            }

            var activeScene = SceneManager.GetSceneByName(m_activeSceneGroup.FindSceneNameByType(ESceneType.Gameplay));
            SceneManager.SetActiveScene(activeScene);
        }

        public async Task UnloadScenes()
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;
            
            int sceneCount = SceneManager.sceneCount;

            for (int i = sceneCount - 1; i > 0 ; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);

                if (!sceneAt.isLoaded)
                    continue;

                var sceneName = sceneAt.name;
                
                if (sceneName.Equals(activeScene) || sceneName == "Core") 
                    continue;

                scenes.Add(sceneName);
            }
            
            var operationGroup = new AsyncOperationGroup(scenes.Count);
            
            foreach (var sceneName in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(sceneName);

                if (operation == null)
                    return;
                
                operationGroup.Operations.Add(operation);
                OnSceneUnloaded.Invoke(sceneName);
            }

            while (!operationGroup.IsDone)
                await Task.Delay(100); 
            
            // unload assets?
        }
        
        public readonly struct AsyncOperationGroup 
        {
            public readonly List<AsyncOperation> Operations;

            public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
            public bool IsDone => Operations.All(o => o.isDone);
             
            public AsyncOperationGroup(int initialCapacity)
            {
                Operations = new List<AsyncOperation>(initialCapacity);
            }
        }
    }
}