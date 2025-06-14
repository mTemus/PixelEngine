using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PixelEngine.Extensions;
using PixelEngine.Utility.Threading;
using UnityEngine.SceneManagement;
using UnityUtils;

namespace PixelEngine.Core.SceneManagement.Loading
{
    public class SceneGroupManager
    {
        public event Action<SceneGroup> OnSceneGroupLoaded = delegate {  };
        public event Action<SceneGroup> OnBeforeSceneGroupUnloaded = delegate {  };

        private readonly int m_sceneMillisecondsDelay = 100;
        private SceneGroup m_activeSceneGroup;

        public SceneGroupManager(int sceneMillisecondsDelay)
        {
            m_sceneMillisecondsDelay = sceneMillisecondsDelay;
        }

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDuplicates = false)
        {
            m_activeSceneGroup = group;

            var loadedScenes = new List<string>();

            await UnloadSceneGroup();
            
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
            }

            while (!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.Progress);
                await Task.Delay(m_sceneMillisecondsDelay); 
            }
            
            var activeScene = SceneManager.GetSceneByName(m_activeSceneGroup.FindSceneNameByType(ESceneType.Gameplay));
            SceneManager.SetActiveScene(activeScene);
            
            OnSceneGroupLoaded.Invoke(m_activeSceneGroup);

            var scenesToInitialize = m_activeSceneGroup.Scenes
                .Where(s => s.IsInitializable)
                .Select(s => s.Scene)
                .Select(s => SceneManager.GetSceneByName(s.Name))
                .Select(s => s.GetComponentFromScene<SceneController>())
                .ToList();

            if (scenesToInitialize.Count == 0)
                return;

            while (scenesToInitialize.Any(s => !s.SceneIsReady))
            {
                await Task.Delay(m_sceneMillisecondsDelay); 
            }
            
            //TODO: while on active scene "scene manager" "IsReady" property, when the initialization/data load is done.
        }

        public async Task UnloadSceneGroup()
        {
            if (m_activeSceneGroup == null)
                return;

            OnBeforeSceneGroupUnloaded.Invoke(m_activeSceneGroup);
            var operationGroup = new AsyncOperationGroup(m_activeSceneGroup.Scenes.Count);
            
            for (int i = m_activeSceneGroup.Scenes.Count - 1; i >= 0; i--)
            {
                var scene = SceneManager.GetSceneByName(m_activeSceneGroup.Scenes[i].Name);
                
                if (!scene.isLoaded)
                    continue;
                
                var operation = SceneManager.UnloadSceneAsync(scene);

                if (operation == null)
                    continue;
                
                operationGroup.Operations.Add(operation);
            }
            
            while (!operationGroup.IsDone)
                await Task.Delay(m_sceneMillisecondsDelay); 

            m_activeSceneGroup = null;
        }
    }
}