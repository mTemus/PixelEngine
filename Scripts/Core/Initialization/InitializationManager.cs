using System;
using System.Linq;

using AYellowpaper.SerializedCollections;
using PixelEngine.Extensions;
using UnityEngine;

#if UNITY_EDITOR
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
            GatherSceneInitializables();
            HandleGroups();
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

        private void HandleGroups()
        {
            foreach (var initializationGroup in m_groups.Values)
            {
                initializationGroup.RemoveNulls();
                initializationGroup.SortByPriority();
            }
        }

        private void GatherSceneInitializables()
        {
            if (!gameObject.scene.TryGetComponents<InitializableObject>(out var initializables, true))
            {
                Debug.LogWarning($"InitialzationManager ({gameObject.scene.name}) --- No InitializableObjects found in scene!");
                return;
            }

            foreach (var initializable in initializables)
                m_groups[initializable.InitializationGroup].AddInitializables(initializable.Initializables);
        }
        
        #endregion

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
        
    }
}