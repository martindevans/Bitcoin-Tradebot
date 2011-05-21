using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MtgoxWebsockets
{
    public static class ObservableExtensions
    {
        public static IObservable<JObject> AsJson(this IObservable<string> strings)
        {
            return strings.Select(a => (a[0] != '{' ? ("{" + a + "}") : a)).Select(a => JObject.Parse(a));
        }

        public static IObservable<JObject> FilterByKeyValue(this IObservable<JObject> objects, string key, string value)
        {
            return objects
                .Where(a =>
                    {
                        JToken c = a[key];
                        if (c == null)
                            return false;
                        return (((string)c).Equals(value));
                    });
        }

        public static IObservable<IEnumerable<T>> SlidingWindow<T>(this IObservable<T> o, int length)
        {
            List<T> window = new List<T>();

            return o.Scan<T, IEnumerable<T>>(new T[0], (a, b) =>
                {
                    window.Add(b);
                    if (window.Count > length)
                        window.RemoveAt(0);
                    return window.ToArray();
                });
        }
    }
}
