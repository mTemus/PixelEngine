using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelEngine.Extensions
{
    public static class ListExtensions
    {
        public static T TakeFirstElement<T>(this List<T> elements)
        {
            var element = elements[0];
            elements.RemoveAt(0);

            return element;
        }

        public static T TakeLastElement<T>(this List<T> elements)
        {
            var element = elements[^1];
            elements.RemoveAt(elements.Count - 1);

            return element;
        }

        public static T Take<T>(this List<T> elements, Func<T, bool> predicate)
        {
            var element = elements.FirstOrDefault(predicate);
            
            if (element != null)
                elements.Remove(element);

            return element;
        }

        public static bool TryTake<T>(this List<T> elements, Func<T, bool> predicate, out T element)
        {
            element = elements.Take(predicate);
            return element != null;
        }
    }
}
