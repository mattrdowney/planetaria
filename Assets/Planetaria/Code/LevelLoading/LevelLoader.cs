using System.Collections;
using UnityEngine;

namespace Planetaria
{
    public class LevelLoader : MonoBehaviour
    {
        public static optional<LoadingStrategy> loader { set; get; }

        private void Awake()
        {
            if (!loader.exists)
            {
                loader = new BasicLoadingStrategy();
                //loader.data.request_level(initial_level); // if loader doesn't exist, it must be startup time
                //wait(initial_level); // when future levels are loaded, do not re-request it (if condition is a must)
            }
        }

        IEnumerator wait(int level_index)
        {
            yield return new WaitUntil(() => loader.data.fraction_loaded(level_index) == 1f);
        }

        private int initial_level = 0;
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