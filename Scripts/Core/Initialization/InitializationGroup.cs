using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelEngine.Core.Initialization
{
    [Serializable]
    public class InitializationGroup
    {
        [SerializeField] 
        [Range(0, 20)] 
        private int m_priority;
        
        [SerializeField] 
        private List<InitializableComponent> m_initializables = new List<InitializableComponent>();

        public int Priority => m_priority;

        public void AddInitializables(List<InitializableComponent> initializables)
        {
            for (var i = 0; i < initializables.Count; i++)
            {
                if (m_initializables.Contains(initializables[i]))
                    continue;
                
                m_initializables.Add(initializables[i]);
            }
            
            SortByPriority();
        }
        
        public void RemoveInitializables(List<InitializableComponent> initializables)
        {
            m_initializables.RemoveAll(initializables.Contains);
            
            SortByPriority();
        }
        
        public void SortByPriority()
        {
            m_initializables
                .Sort((x, y) => y.Priority.CompareTo(x.Priority));
        }

        #region Editor

#if UNITY_EDITOR
        public void RemoveNulls()
        {
            for (var i = m_initializables.Count - 1; i >= 0; i--) 
                if (m_initializables[i] == null)
                    m_initializables.RemoveAt(i);
        }

#endif
        
        #endregion

        #region Initialization

        public void EarlyInitialize() => m_initializables.ForEach(x => x.Component.EarlyInitialize());

        public void InitializeAsNew() => m_initializables.ForEach(x => x.Component.InitializeAsNew());

        public void InitializeAsLoaded() => m_initializables.ForEach(x => x.Component.InitializeAsLoaded());

        public void LateInitialize() => m_initializables.ForEach(x => x.Component.LateInitialize());

        public void Uninitialize()
        {
            for (var i = m_initializables.Count - 1; i >= 0; i--)
                m_initializables[i].Component.Uninitialize();
        }

        #endregion
    }
}