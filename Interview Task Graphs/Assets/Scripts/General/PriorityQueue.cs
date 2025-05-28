using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.General
{
    public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
    {
        private readonly SortedDictionary<TPriority, Queue<TElement>> _dict = new();

        public void Enqueue(TElement element, TPriority priority)
        {
            if (!_dict.TryGetValue(priority, out var queue))
            {
                queue = new Queue<TElement>();
                _dict[priority] = queue;
            }

            queue.Enqueue(element);
        }

        public TElement Dequeue()
        {
            var firstPair = _dict.First();
            var element = firstPair.Value.Dequeue();
            if (firstPair.Value.Count == 0)
                _dict.Remove(firstPair.Key);
            return element;
        }

        public int Count => _dict.Sum(pair => pair.Value.Count);
    }
}