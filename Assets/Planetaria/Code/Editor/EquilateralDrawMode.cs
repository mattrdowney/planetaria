using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    public static class EquilateralDrawMode
    {
        /// <summary>
        /// Mutator - MouseDown creates equilateral shape with center at mouse position
        /// </summary>
        /// <returns>The next mode for the state machine.</returns>
        public static LevelCreatorEditor.CreateShape draw_center(bool force_quit = false)
        {
            if (force_quit)
            {
                return escape();
            }
            // MouseDown: create center of equilateral shape
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                builder = EquilateralBuilder.equilateral_builder(LevelCreatorEditor.get_mouse_position(), LevelCreatorEditor.edges);
                return draw_equilateral;
            }

            return draw_center;
        }

        /// <summary>
        /// Mutator - MouseDown finalizes equilateral shape.
        /// </summary>
        /// <returns>The next mode for the state machine.</returns>
        public static LevelCreatorEditor.CreateShape draw_equilateral(bool force_quit = false)
        {
            if (force_quit)
            {
                return escape();
            }

            builder.data.set_edge(LevelCreatorEditor.get_mouse_position());

            // MouseUp: create first vertex of equilateral shape
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                builder.data.close_shape();
                builder = new optional<EquilateralBuilder>();
                return draw_center;
            }

            return draw_equilateral;
        }

        private static LevelCreatorEditor.CreateShape escape()
        {
            builder.data.close_shape();
            builder = new optional<EquilateralBuilder>();
            return LevelCreatorEditor.draw_initialize;
        }

        private static optional<EquilateralBuilder> builder;
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