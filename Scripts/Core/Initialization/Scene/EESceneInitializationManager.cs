#if UNITY_EDITOR
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using EditorAttributes;
using PixelEngine.Extensions;
using UnityEditor;
using UnityEngine;
using Void = EditorAttributes.Void;

namespace PixelEngine.Scripts.Core.Initialization.Scene
{
    public class SceneInitializationManager : MonoBehaviour
    {
        [SerializeField] 
        private SerializedDictionary<InitializationGroup, List<InitializableComponent>> m_initializables;
        
        #region Editor setup
        
#if UNITY_EDITOR

        private void OnValidate()
        {
            PopulateKeys();
            OrderGroupsAscending();
            CheckForNulls();
        }
        
        [Title("Dev Buttons", titleSize: 22, titleSpace: 12, alignment: TextAnchor.MiddleCenter, drawLine: true, lineThickness: 2)]
        [SerializeField] private Void m_buttonGroup;
        
        [Button]
        public void GatherSceneInitializables()
        {
            foreach (var components in m_initializables.Values)
                components.Clear();
            
            gameObject.scene.TryGetComponents<EEInitializableSceneObject>(out var sceneInitializables, true);

            foreach (var initializable in sceneInitializables)
                AddSceneInitializables(initializable.Components, initializable.Group);
            
            EditorUtility.SetDirty(this); // Mark the object as dirty so Unity serializes the change
        }        
        
        [Button]
        private void OrderGroupsAscending()
        {
            m_initializables = new SerializedDictionary<InitializationGroup, List<InitializableComponent>>(m_initializables.OrderByDescending(group => group.Key.Priority));
        }
        
        [Button]
        private void CheckForNulls()
        {
            foreach (var initializable in m_initializables)
            {
                for (var i = initializable.Value.Count - 1; i >= 0; i--)
                    if (initializable.Value[i].Component == null)
                        initializable.Value.RemoveAt(i);
            }
        }
        
        [Button]
        private void PopulateKeys()
        {
            var enumValues = Enum.GetValues(typeof(EInitializationGroup));
            
            if (m_initializables.Keys.Count == enumValues.Length) 
                return;
            
            var keys = m_initializables.Keys;
                
            foreach (EInitializationGroup groupEnum in enumValues)
            {
                if (keys.Any(key => key.Group == groupEnum))
                    continue;
                    
                m_initializables.Add(new InitializationGroup(groupEnum), new List<InitializableComponent>());
            }
        }
#endif
        #endregion

        #region Initialization

        public void StartInitialization()
        {
#if UNITY_EDITOR
            Debug.Log($"Starting early initialization of scene {gameObject.scene.name}.");
#endif

            SetInitializationState(EarlyInitialize);
        }

        public void InitializeAsNew()
        {
#if UNITY_EDITOR
            Debug.Log($"Starting new initialization of scene {gameObject.scene.name}.");
#endif

            SetInitializationState(InitializeAsNew);
            SetInitializationState(LateInitialize);
        }

        public void InitializeAsLoaded()
        {
#if UNITY_EDITOR
            Debug.Log($"Starting initialization as loaded of scene {gameObject.scene.name}.");
#endif

            SetInitializationState(InitializeAsLoaded);
            SetInitializationState(LateInitialize);
        }

        //Uninitializing should run backward to initializing!
        public void Uninitialize()
        {
            SetInitializationStateReversed(Uninitialize);
        }

        private void Uninitialize(List<InitializableComponent> components)
        {
            foreach (var component in components)
                component.Component.Uninitialize();
        }
        
        #endregion

        #region Initialization States

        private void SetInitializationState(Action<List<IInitializable>> state)
        {
            // var groupComponents = new List<InitializableComponent>();

            foreach (var initializableGroup in m_initializables)
                state.Invoke(initializableGroup.Value.Select(comp => comp.Component).ToList());
            
            // for (var i = 0; i < m_initializationGroups.Length; i++)
            // {
            //     if (m_initializables.TryGetValue(m_initializationGroups[i].Group, out groupComponents))
            //         state.Invoke(groupComponents.Select(comp => comp.Component).ToList());
            // }
        }

        private void SetInitializationStateReversed(Action<List<IInitializable>> state)
        {
            var keys = m_initializables.Keys.ToList();

            for (var i = keys.Count - 1; i >= 0; i--)
                if (m_initializables.TryGetValue(keys[i], out var groupComponents))
                    state.Invoke(groupComponents.Select(comp => comp.Component).ToList());
        }

        private void EarlyInitialize(List<IInitializable> initializables)
        {
            for (var i = 0; i < initializables.Count; i++)
                initializables[i].EarlyInitialize();
        }

        private void InitializeAsNew(List<IInitializable> initializables)
        {
            for (var i = 0; i < initializables.Count; i++)
                initializables[i].InitializeAsNew();
        }

        private void InitializeAsLoaded(List<IInitializable> initializables)
        {
            for (var i = 0; i < initializables.Count; i++)
                initializables[i].InitializeAsLoaded();
        }

        private void LateInitialize(List<IInitializable> initializables)
        {
            for (var i = 0; i < initializables.Count; i++)
                initializables[i].LateInitialize();
        }

        private void Uninitialize(List<IInitializable> initializables)
        {
            initializables.Reverse();

            for (var i = 0; i < initializables.Count; i++)
                initializables[i].Uninitialize();
        }

        #endregion
        
        #region Scene Initializables

        public void AddSceneInitializables(List<InitializableComponent> components, EInitializationGroup group)
        {
            if (group == EInitializationGroup.Default)
                Debug.LogError($"SceneInitializationManager --- Adding initializables of group 'Default' from object {(components[0].Component as MonoBehaviour).name}, this should be done!");
            
            TryAddInitializables(components, group, m_initializables);
        }

        #endregion

        #region Unitilty

        private bool TryAddInitializables(List<InitializableComponent> components, EInitializationGroup group, Dictionary<InitializationGroup, List<InitializableComponent>> collection)
        {
            CheckAndRemoveComponentDuplicate(components, collection, group);

            var existingGroup = collection.Keys.FirstOrDefault(g => g.Group == group);
            
            collection[existingGroup].AddRange(components);
            collection[existingGroup] = collection[existingGroup].Distinct().OrderByDescending(comp => comp.Priority).ToList();
            
            
            //
            // if (existingGroup.Group == group)
            // {
            //     collection[group].AddRange(components);
            //     collection[group] = collection[group].Distinct().OrderByDescending(comp => comp.Priority).ToList();
            // }
            // else
            // {
            //     collection.Add(group, new List<InitializableComponent>(components));
            // }

            return true;
        }

        private void CheckAndRemoveComponentDuplicate(List<InitializableComponent> components, Dictionary<InitializationGroup, List<InitializableComponent>> collection, EInitializationGroup excludeGroup)
        {
            foreach (var initializationGroup in collection)
                initializationGroup.Value.RemoveAll(existingComp =>
                    components.Any(compToAdd => compToAdd.Component == existingComp.Component));
        }

        #endregion
        
        [Serializable]
        public struct InitializationGroup
        {
            [SerializeField] private EInitializationGroup m_group;

            // [HelpBox("Higher number then higher priority of the group.", MessageMode.None)]
            [SerializeField, Range(0, 100f)] private int m_priority;

            public InitializationGroup(EInitializationGroup group, int priority = 0)
            {
                m_group = group;
                m_priority = priority;
            }

            public EInitializationGroup Group => m_group;
            public int Priority => m_priority;
        }
    }
}
