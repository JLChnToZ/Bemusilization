using System;
using System.Collections;
using System.Collections.Generic;

namespace LitJson.ExtensionsHelpers {
    class JsonWrapperKeyValueEnumerable: IEnumerable<KeyValuePair<string, IJsonWrapper>> {
        readonly IJsonWrapper jsonWrapper;
        private class Enumerator: IEnumerator<KeyValuePair<string, IJsonWrapper>> {
            readonly IJsonWrapper jsonWrapper;
            readonly IEnumerator keyEnumerator;
            int index;

            public Enumerator(IJsonWrapper jsonWrapper) {
                this.jsonWrapper = jsonWrapper;
                if(jsonWrapper != null && jsonWrapper.IsObject)
                    keyEnumerator = jsonWrapper.Keys.GetEnumerator();
                index = -1;
            }

            public KeyValuePair<string, IJsonWrapper> Current {
                get {
                    if(jsonWrapper == null)
                        return default(KeyValuePair<string, IJsonWrapper>);
                    if(keyEnumerator != null) {
                        string current = keyEnumerator.Current as string;
                        return new KeyValuePair<string, IJsonWrapper>(
                            current,
                            jsonWrapper[current] as IJsonWrapper
                        );
                    }
                    return new KeyValuePair<string, IJsonWrapper>(
                        index.ToString(),
                        (jsonWrapper as IList)[index] as IJsonWrapper
                    );
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

        public JsonWrapperKeyValueEnumerable(IJsonWrapper jsonWrapper) {
            this.jsonWrapper = jsonWrapper;
        }

        public IEnumerator<KeyValuePair<string, IJsonWrapper>> GetEnumerator() {
            return new Enumerator(jsonWrapper);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator(jsonWrapper);
        }
    }
}
