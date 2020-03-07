using System.Collections;

namespace LitJson.ExtensionsHelpers
{
    internal struct PlaceHolder {
        public readonly IJsonWrapper source;
        private readonly object container;
        private readonly int index;
        private readonly object key;

        public PlaceHolder(IJsonWrapper source) {
            this.source = source;
            this.container = null;
            this.index = 0;
            this.key = null;
        }

        public PlaceHolder(IJsonWrapper source, ArrayList arrayList, int index) {
            this.source = source;
            this.container = arrayList;
            this.index = index;
            this.key = null;
        }

        public PlaceHolder(IJsonWrapper source, Hashtable hashtable, object key) {
            this.source = source;
            this.container = hashtable;
            this.index = 0;
            this.key = key;
        }

        public void Assign(object value) {
            if(container is ArrayList arrayList)
                arrayList[index] = value;
            else if(container is Hashtable hashtable)
                hashtable[key] = value;
        }
    }
}
