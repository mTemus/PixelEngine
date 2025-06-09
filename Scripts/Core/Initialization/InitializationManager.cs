using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using EditorAttributes;
using PixelEngine.Extensions;
using UnityEngine;
using UnityUtils;

#if UNITY_EDITOR
using UnityEditor;
using System.Threading.Tasks;
#endif

//TODO: runtime initializables
namespace PixelEngine.Core.Initialization
{
    public class InitializationManager : MonoBehaviour
    {
        [SerializeField] 
        private SerializedDictionary<EInitializationGroup, InitializationGroup> m_groups;
        
        #region Editor

        private void OnValidate()
        {
            PopulateKeys();
            SortByPriority();
            RemoveNulls();
            GatherSceneInitializables();
            SortInitializables();
        }

        private void PopulateKeys()
        {
            var enumValues = Enum.GetValues(typeof(EInitializationGroup));

            if (m_groups.Keys.Count == enumValues.Length) 
                return;
            
            var keys = m_groups.Keys;
                
            foreach (EInitializationGroup groupEnum in enumValues)
            {
                if (keys.Contains(groupEnum))
                    continue;
                    
                m_groups.Add(groupEnum, new InitializationGroup());
            }
        }

        private async void SortByPriority()
        {
            try
            {
                var sortedDict = m_groups
                    .OrderByDescending(x => x.Value.Priority)
                    .ToDictionary(x => x.Key, x => x.Value);
            
                await Task.Delay(500); 
                
                m_groups = new SerializedDictionary<EInitializationGroup, InitializationGroup>
                (
                    sortedDict
                );
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void RemoveNulls() => m_groups.ForEach(x => x.Value.RemoveNulls());
        private void SortInitializables() => m_groups.ForEach(x => x.Value.SortByPriority());

        [Button("Gather Initializables")]
        private void GatherSceneInitializables()
        {
            if (!gameObject.scene.TryGetComponents<InitializableObject>(out var initializableObjects, true))
            {
                Debug.LogWarning($"InitialzationManager ({gameObject.scene.name}) --- No InitializableObjects found in scene!");
                return;
            }

            foreach (var initializableObject in initializableObjects)
            {
                if (initializableObject.Group == EInitializationGroup.Default)
                    Debug.LogError($"SceneInitializationManager ({gameObject.scene.name}) --- Initializable ({initializableObject.gameObject.name} has Default group!");
                
                m_groups[initializableObject.Group].AddInitializables(initializableObject.Initializables);
            }
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(this); // Mark the object as dirty so Unity serializes the change
#endif
        }
        
        #endregion

        #region Initialization

        public void RunInitialization(bool initializeAsNew)
        {
            InitializeAsEarly();
            if (initializeAsNew) InitializeAsNew();
            else InitializeAsLoaded();
            LateInitialize();
        }

        public void RunUninitialization()
        {
            Uninitialize();
        }

        private void InitializeAsEarly()
        {
            foreach (var group in m_groups.Values)
                group.EarlyInitialize();
        }

        private void InitializeAsNew()
        {
            foreach (var group in m_groups.Values)
                group.InitializeAsNew();
        }

        private void InitializeAsLoaded()
        {
            foreach (var group in m_groups.Values)
                group.InitializeAsLoaded();
        }

        private void LateInitialize()
        {
            foreach (var group in m_groups.Values)
                group.LateInitialize();
        }

        private void Uninitialize()
        {
            foreach (var group in m_groups.Values)
                group.Uninitialize();
        }

        #endregion
    }
}