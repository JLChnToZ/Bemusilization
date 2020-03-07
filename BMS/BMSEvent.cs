using System;

namespace BMS {
    public enum BMSEventType: byte {
        Unknown,
        BMP,
        WAV,
        BPM,
        STOP,
        Note,
        LongNoteStart,
        LongNoteEnd,
        BeatReset
    }

    [Serializable]
    public struct BMSEvent: IComparable<BMSEvent>, IEquatable<BMSEvent>, IComparable {
        public BMSEventType type;
        public int ticks;
        public int measure;
        public float beat;
        public TimeSpan time, time2;
        public int data1;
        public long data2;
        public TimeSpan sliceStart, sliceEnd;

        public double Data2F {
            get => BitConverter.Int64BitsToDouble(data2);
            set => data2 = BitConverter.DoubleToInt64Bits(value);
        }

        public bool IsNote =>
            type == BMSEventType.Note ||
            type == BMSEventType.LongNoteStart ||
            type == BMSEventType.LongNoteEnd;

        public int CompareTo(BMSEvent other) {
            if(Equals(other)) return 0;
            int comparison;
            if((comparison = time.CompareTo(other.time)) != 0) return comparison;
            if((comparison = ticks.CompareTo(other.ticks)) != 0) return comparison;
            if((comparison = measure.CompareTo(other.measure)) != 0) return comparison;
            if((comparison = beat.CompareTo(other.beat)) != 0) return comparison;
            return 0;
        }

        int IComparable.CompareTo(object obj) =>
            obj is BMSEvent other ? CompareTo(other) : 0;

        public bool Equals(BMSEvent other) {
            return type == other.type && (
                ticks == other.ticks ||
                (measure == other.measure && beat == other.beat) ||
                time == other.time) &&
                data1 == other.data1 &&
                data2 == other.data2;
        }

        public override bool Equals(object obj) =>
            obj is BMSEvent other && Equals(other);

        public override int GetHashCode() {
            unchecked {
                int hashCode = 17;
                hashCode = hashCode * 23 + type.GetHashCode();
                hashCode = hashCode * 23 + ticks.GetHashCode();
                hashCode = hashCode * 23 + measure.GetHashCode();
                hashCode = hashCode * 23 + beat.GetHashCode();
                hashCode = hashCode * 23 + time.GetHashCode();
                hashCode = hashCode * 23 + data1.GetHashCode();
                hashCode = hashCode * 23 + data2.GetHashCode();
                return hashCode;
            }
        }
    }

}
