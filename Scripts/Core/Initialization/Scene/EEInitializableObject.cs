using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelEngine.Scripts.Core.Initialization.Scene
{
    public abstract class EEInitializableObject : MonoBehaviour
    {
        [SerializeField]
        protected EInitializationGroup m_group;

        [SerializeField] protected List<InitializableComponent> m_components;

        public EInitializationGroup Group => m_group;
        public List<InitializableComponent> Components => m_components;

        protected virtual void OnValidate()
        {
            GatherInitializables();
        }

        private void GatherInitializables()
        {
            var initializables = GetComponents<IInitializable>().ToList();

            var childCount = transform.childCount;

            for (var i = 0; i < childCount; i++)
                if (TryGetChildInitializables(transform.GetChild(i), out var childInitializables))
                    initializables.AddRange(childInitializables);
            
            // for (var i = toAdd.Count - 1; i >= 0; --i)
            // {
            //     if (((MonoBehaviour)toAdd[i]).TryGetComponent<EEInitializableSceneObject>(out _))
            //         toAdd.RemoveAt(i);
            // }

            initializables = initializables.Distinct().ToList();

            if (initializables.Count == m_components.Count)
                return;

            foreach (var component in new List<InitializableComponent>(m_components))
            {
                if (!initializables.Contains(component.Component))
                    m_components.Remove(component);
                else
                    initializables.Remove(component.Component);
            }

            foreach (var initializable in initializables)
                m_components.Add(new InitializableComponent(initializable));
        }

        private bool TryGetChildInitializables(Transform child, out List<IInitializable> initializables)
        {
            var childCount = child.childCount;
            initializables = new List<IInitializable>();

            if (TryGetComponent<EEInitializableObject>(out _))
                return false;

            for (var i = 0; i < childCount; i++)
                if (TryGetChildInitializables(transform.GetChild(i), out var childInitializables))
                    initializables.AddRange(childInitializables);
            
            initializables.AddRange(GetComponents<IInitializable>());
            return true; 
        }
        
    }
}
