using System;
using System.Collections;
using System.Collections.Generic;

namespace LitJson.ExtensionsHelpers {
    internal class JsonWrapperEnumerable: IEnumerable<IJsonWrapper> {
        readonly IJsonWrapper jsonWrapper;
        private class Enumerator: IEnumerator<IJsonWrapper> {
            readonly IJsonWrapper jsonWrapper;
            readonly IEnumerator keyEnumerator;
            int index;

            public Enumerator(IJsonWrapper jsonWrapper) {
                this.jsonWrapper = jsonWrapper;
                if(jsonWrapper != null && jsonWrapper.IsObject)
                    keyEnumerator = jsonWrapper.Keys.GetEnumerator();
                index = -1;
            }

            public IJsonWrapper Current {
                get {
                    if(jsonWrapper == null) return null;
                    if(keyEnumerator != null)
                        return jsonWrapper[keyEnumerator.Current] as IJsonWrapper;
                    return (jsonWrapper as IList)[index] as IJsonWrapper;
                }
            }

            object IEnumerator.Current {
                get { return Current; }
            }

            public void Dispose() {
                IDisposable keyEnumeratorDisposable = keyEnumerator as IDisposable;
                if(keyEnumeratorDisposable != null)
                    keyEnumeratorDisposable.Dispose();
            }

            public bool MoveNext() {
                if(jsonWrapper == null) return false;
                if(keyEnumerator != null)
                    return keyEnumerator.MoveNext();
                return jsonWrapper.IsArray && ++index < jsonWrapper.Count;
            }

            public void Reset() {
                if(keyEnumerator != null) {
                    keyEnumerator.Reset();
                    return;
                }
                index = -1;
            }
        }

        public JsonWrapperEnumerable(IJsonWrapper jsonWrapper) {
            this.jsonWrapper = jsonWrapper;
        }

        public IEnumerator<IJsonWrapper> GetEnumerator() {
            return new Enumerator(jsonWrapper);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator(jsonWrapper);
        }
    }
}
