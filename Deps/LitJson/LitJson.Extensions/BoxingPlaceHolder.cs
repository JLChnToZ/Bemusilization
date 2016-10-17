using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitJson.ExtensionsHelpers {
    internal class PlaceHolder {
        public readonly IJsonWrapper source;
        private readonly ArrayList arrayList;
        private readonly Hashtable hashtable;
        private readonly int index;
        private readonly object key;

        public PlaceHolder(IJsonWrapper source) {
            this.source = source;
            this.arrayList = null;
            this.index = 0;
            this.hashtable = null;
            this.key = null;
        }

        public PlaceHolder(IJsonWrapper source, ArrayList arrayList, int index) {
            this.source = source;
            this.arrayList = arrayList;
            this.index = index;
            this.hashtable = null;
            this.key = null;
        }

        public PlaceHolder(IJsonWrapper source, Hashtable hashtable, object key) {
            this.source = source;
            this.arrayList = null;
            this.index = 0;
            this.hashtable = hashtable;
            this.key = key;
        }

        public void Assign(object value) {
            if(arrayList != null)
                arrayList[index] = value;
            else if(hashtable != null)
                hashtable[key] = value;
        }
    }
}
