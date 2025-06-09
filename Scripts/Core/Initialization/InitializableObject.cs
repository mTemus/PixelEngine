using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEngine;

namespace PixelEngine.Core.Initialization
{
    public class InitializableObject : MonoBehaviour
    {
        [SerializeField]
        private EInitializationGroup m_initializationGroup;
        
        [SerializeField]
        private List<InitializableComponent> m_initializables = new List<InitializableComponent>();

        public EInitializationGroup Group => m_initializationGroup;
        public List<InitializableComponent> Initializables => m_initializables;

        #region Editor

#if UNITY_EDITOR
        
        private void OnValidate()
        {
            GatherInitializables();
            SortByPriority();
        }

        [Button("Gather Object Initializables")]
        private void GatherInitializables()
        {
            var componentInterfaces = GetComponentsInChildren<IInitializable>().ToList();

            RemoveNulls();
            
            foreach (var component in new List<InitializableComponent>(m_initializables))
            {
                if (!componentInterfaces.Contains(component.Component))
                    m_initializables.Remove(component);
                else
                    componentInterfaces.Remove(component.Component);
            }

            foreach (var componentInterface in componentInterfaces)
                m_initializables.Add(new InitializableComponent(componentInterface));
        }

        private void RemoveNulls()
        {
            for (var i = m_initializables.Count - 1; i >= 0; i--)
                if (m_initializables[i].Component == null)
                    m_initializables.RemoveAt(i);
        }

        private void SortByPriority()
        {
            m_initializables.Sort((x, y) => y.Priority.CompareTo(x.Priority));
        }

#endif
        
        #endregion
        
    }
}