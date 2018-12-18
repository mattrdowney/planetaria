using UnityEngine;
using UnityEngine.SceneManagement;

namespace Planetaria
{
    public class BasicLoadingStrategy : LoadingStrategy
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BasicLoadingStrategy()
        {
            for (int level_index = 0; level_index < SceneManager.sceneCountInBuildSettings; ++level_index) // FIXME: This should NOT load Menu Screen and other GUI levels
            {
                // FIXME: load
            }
        }

        /// <summary>
        /// Inspector - Return 100% loaded, since the basic loader loads all levels in Awake.
        /// </summary>
        /// <param name="level_index">The index of the level that would have been loaded. (Should match Unity level index.)</param>
        /// <returns>1, meaning 100% loaded.</returns>
        public override float fraction_loaded(int level_index)
        {
            // CONSIDER: invalid indices return 0
            return 1;
        }

        /// <summary>
        /// Inspector - Since all levels are loaded, do nothing.
        /// </summary>
        /// <param name="level_index">The index of the level that would have been loaded. (Should match Unity level index.)</param>
        /// <param name="priority">Higher priority levels are loaded sooner and kept longer.</param>
        public override void request_level(int level_index, float priority)
        {
        }

        /// <summary>
        /// Mutator - Activates the level if necessary (and--if necessary--unloads other levels).
        /// </summary>
        /// <param name="level_index">The index of the level that will be loaded. (Should match Unity level index.)</param>
        public override void activate_level(int level_index)
        {
        }
    }
}

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.