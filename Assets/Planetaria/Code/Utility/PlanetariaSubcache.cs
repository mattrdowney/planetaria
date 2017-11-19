using System.Collections.Generic;

namespace Planetaria
{
    public class PlanetariaSubcache<Key, Value>
    {
        public void cache(Key key, Value value)
        {
            map_cache.Add(key, value);
        }

        public void uncache(Key key)
        {
            map_cache.Remove(key);
        }

        public optional<Value> get(Key key)
        {
            if (!map_cache.ContainsKey(key))
            {
                return new optional<Value>();
            }

            return map_cache[key];
        }

        public void clear()
        {
            map_cache.Clear();
        }

        private Dictionary<Key, Value> map_cache = new Dictionary<Key, Value>();
    }
}

/*
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/