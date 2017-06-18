using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Utils;

namespace BMS {
    public abstract partial class Chart {
        protected string title;
        protected string subTitle;
        protected string artist;
        protected string subArtist;
        protected string comments;
        protected string genre;
        protected int playerCount;
        protected float initialBPM;
        protected float minBpm;
        protected int playLevel;
        protected int rank;
        protected float volume;
        protected int maxCombos;
        private readonly List<BMSEvent> bmsEvents = new List<BMSEvent>();
        private readonly Dictionary<ResourceId, BMSResourceData> resourceDatas = new Dictionary<ResourceId, BMSResourceData>();
        private readonly Dictionary<ResourceId, BMSResourceData> metaResourceDatas = new Dictionary<ResourceId, BMSResourceData>();
        private readonly HashSet<int> allChannels = new HashSet<int>();

        private readonly List<WeakReference> eventDispatchers = new List<WeakReference>();

        public virtual string Title { get { return title; } }
        public virtual string SubTitle { get { return subTitle; } }
        public virtual string Artist { get { return artist; } }
        public virtual string SubArtist { get { return subArtist; } }
        public virtual string Comments { get { return comments; } }
        public virtual string Genre { get { return genre; } }
        public virtual int PlayerCount { get { return playerCount; } }
        public virtual float BPM { get { return initialBPM; } }
        public virtual float MinBPM { get { return minBpm; } }
        public virtual float PlayLevel { get { return playLevel; } }
        public virtual int Rank { get { return rank; } }
        public virtual float Volume { get { return volume; } }
        public virtual int MaxCombos { get { return maxCombos; } }
        public virtual string RawContent { get { return string.Empty; } }
        public virtual bool Randomized { get { return false; } }

        public IList<BMSEvent> Events {
            get { return new ReadOnlyCollection<BMSEvent>(bmsEvents); }
        }

        public ICollection<int> AllChannels {
            get { return allChannels; }
        }

        public virtual void Parse(ParseType parseType) {
            if((parseType & ParseType.Content) == ParseType.Content)
                OnDataRefresh();
        }

        private void OnDataRefresh() {
            lock(eventDispatchers)
                for(int i = 0; i < eventDispatchers.Count; i++) {
                    WeakReference wr = eventDispatchers[i];
                    if(!wr.IsAlive) {
                        eventDispatchers.RemoveAt(i--);
                        continue;
                    }
                    EventDispatcher target = wr.Target as EventDispatcher;
                    if(target != null)
                        target.OnBMSRefresh();
                }
        }

        protected void ResetAllData(ParseType parseType) {
            if((parseType & ParseType.Header) == ParseType.Header) {
                title = "";
                artist = "";
                subArtist = "";
                comments = "";
                genre = "";
                playerCount = 1;
                initialBPM = 130;
                minBpm = float.PositiveInfinity;
                playLevel = 0;
                rank = 0;
                volume = 1;
                metaResourceDatas.Clear();
            }
            if((parseType & ParseType.Resources) == ParseType.Resources) {
                resourceDatas.Clear();
                foreach(var kv in metaResourceDatas)
                    resourceDatas[kv.Key] = kv.Value;
            }
            if((parseType & ParseType.Content) == ParseType.Content) {
                maxCombos = 0;
                bmsEvents.Clear();
                allChannels.Clear();
                OnDataRefresh();
            }
        }

        protected void AddResource(ResourceType type, long id, string dataPath, object additionalData = null) {
            ResourceId resId = new ResourceId(type, id);
            BMSResourceData resData = new BMSResourceData {
                type = type,
                resourceId = id,
                dataPath = dataPath,
                additionalData = additionalData
            };
            resourceDatas[resId] = resData;
            if(id < 0) metaResourceDatas[resId] = resData;
        }

        protected int AddEvent(BMSEvent ev) {
            if(ev.IsNote) {
                allChannels.Add(ev.data1);
                maxCombos++;
            }
            return bmsEvents.InsertInOrdered(ev);
        }

        protected void AddEvents(IEnumerable<BMSEvent> events) {
            bmsEvents.InsertInOrdered(events);
            allChannels.UnionWith(
                events.Where(ev => ev.IsNote)
                .Select(ev => ev.data1)
            );
        }

        protected int FindEventIndex(BMSEvent ev) {
            int firstIndex = bmsEvents.BinarySearchIndex(ev, BinarySearchMethod.FirstExact);
            int lastIndex = bmsEvents.BinarySearchIndex(ev, BinarySearchMethod.LastExact, firstIndex);
            return firstIndex == lastIndex ? firstIndex :
                bmsEvents.IndexOf(ev, firstIndex, lastIndex - firstIndex + 1);
        }

        protected void ReplaceEvent(int index, BMSEvent newEv) {
            BMSEvent original = bmsEvents[index];
            if(original.CompareTo(newEv) == 0) {
                bmsEvents[index] = newEv;
                return;
            }
            bmsEvents.RemoveAt(index);
            bmsEvents.InsertInOrdered(newEv);
        }

        public EventDispatcher GetEventDispatcher() {
            return new EventDispatcher(this);
        }

        public IEnumerable<BMSResourceData> IterateResourceData(ResourceType type = ResourceType.Unknown) {
            if(type == ResourceType.Unknown)
                return resourceDatas.Values;
            return from kv in resourceDatas
                   where kv.Key.type == type
                   select kv.Value;
        }

        public BMSResourceData GetResourceData(ResourceType type, long id) {
            BMSResourceData result;
            resourceDatas.TryGetValue(new ResourceId(type, id), out result);
            return result;
        }

        public bool TryGetResourceData(ResourceType type, long id, out BMSResourceData result) {
            return resourceDatas.TryGetValue(new ResourceId(type, id), out result);
        }
    }

    [Flags]
    public enum ParseType {
        None = 0,
        Header = 0x1,
        Resources = 0x2,
        Content = 0x4,
    }
}
