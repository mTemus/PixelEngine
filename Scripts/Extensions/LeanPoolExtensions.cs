using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

namespace PixelEngine.Extensions
{
    public static class LeanPoolExtensions
    {
        public static void LeanPoolDespawnAllElements<T>(this List<T> elements) where T : Component
        {
            for (var i = elements.Count - 1; i >= 0; --i)
                LeanPool.Despawn(elements[i]);
        
            elements.Clear();
        }
    }
}