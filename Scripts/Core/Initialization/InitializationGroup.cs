using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper;
using EditorAttributes;
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

        public List<InitializableComponent> GetInitializables(bool reversed)
        {
            return reversed ? m_initializables : m_initializables.AsEnumerable().Reverse().ToList();
        }

        public void AddInitializables(List<InitializableComponent> initializables)
        {
            m_initializables.AddRange(initializables);
            
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

#if UNITY_EDITOR
        public void RemoveNulls()
        {
            for (var i = m_initializables.Count - 1; i >= 0; i--) 
                if (m_initializables[i] == null)
                    m_initializables.RemoveAt(i);
        }
#endif
        
    }
}