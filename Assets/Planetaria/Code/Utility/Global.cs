using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    public class Global : MonoBehaviour
    {
        public static Global self
        {
            get
            {
                if (self_variable.exists)
                {
                    return self_variable.data;
                }
                GameObject game_master = Miscellaneous.GetOrAddObject("GameMaster", false);
                game_master.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector; // hide by default
                self_variable = game_master.transform.GetOrAddComponent<Global>();
                return self_variable.data;
            }
        }

        [SerializeField] public bool hide_graphics; // (Shift + g(raphics))
        [SerializeField] public bool show_inspector; // (Shift + i(nspector))
        /// <summary>Number of rows in the grid. Equator is drawn when rows is odd</summary>     
        [SerializeField] public int rows = 63; 
        /// <summary>Number of columns in the grid (per hemisphere).</summary>
        [SerializeField] public int columns = 64;
        [SerializeField] public bool v_pressed = false;

        private static optional<Global> self_variable;
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