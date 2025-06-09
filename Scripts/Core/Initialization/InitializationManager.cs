using System;
using System.Linq;

using AYellowpaper.SerializedCollections;
using UnityEngine;

#if UNITY_EDITOR
using System.Threading.Tasks;
#endif

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

        #endregion

        [Serializable]
        public enum EInitializationGroup
        {
            Core = 1,
            Systems = 2,
            UI = 3,
            Player = 4,
            Entities = 5,
            AI = 6,
            Environment = 7,
        }
    }
}