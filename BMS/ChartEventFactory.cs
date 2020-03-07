using System;
using System.Collections.Generic;
using System.Text;

using Utils;

namespace BMS {
    public class ChartEventFactory {
        private readonly SortedSet<BMSEvent> eventTree;

        public ChartEventFactory() {
            eventTree = new SortedSet<BMSEvent>();
        }

        public void ToList(ICollection<BMSEvent> list) {
            if(list is List<BMSEvent> genericList && genericList.Capacity < eventTree.Count)
                genericList.Capacity = eventTree.Count;
            foreach(var ev in eventTree)
                list.Add(ev);
        }

        public List<BMSEvent> ToList() =>
            new List<BMSEvent>(eventTree);

        public void AddEvent(BMSEvent bmsEvent) => eventTree.Add(bmsEvent);
    }
}
