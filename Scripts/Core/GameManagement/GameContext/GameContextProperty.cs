using System;
using CustomInspector;
using UnityEngine;

namespace PixelEngine.Core.GameManagement.Context
{
    [Serializable]
    public abstract class GameContextProperty
    {
        [SerializeField, ReadOnly]
        private ScriptableEnumGameContextPropertyId m_id;

        public ScriptableEnumGameContextPropertyId ID => m_id;

        protected GameContextProperty(ScriptableEnumGameContextPropertyId id)
        {
            m_id = id;
        }
    }
}