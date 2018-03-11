using UnityEngine;

namespace Planetaria
{
    public static class FreeLineDrawMode
    {
        public static LevelCreatorEditor.CreateShape draw_first_point(bool force_quit = false)
        {
            if (force_quit)
            {
                return escape();
            }
            // MouseDown 1: create first corner of a shape
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                temporary_arc = ArcBuilder.arc_builder(LevelCreatorEditor.get_mouse_position());
                return draw_tangent;
            }

            return draw_first_point;
        }

        /// <summary>
        /// Mutator - MouseUp creates slope at position
        /// </summary>
        /// <param name="editor">The reference to the LevelCreatorEditor object that stores shape information.</param>
        /// <returns>mouse_up if nothing was pressed; mouse_down if MouseUp or Escape was pressed.</returns>
        private static LevelCreatorEditor.CreateShape draw_tangent(bool force_quit = false)
        {
            if (force_quit)
            {
                return escape();
            }
            temporary_arc.data.receive_vector(LevelCreatorEditor.get_mouse_position());

            // MouseUp 1-n: create the right-hand-side slope for the last point added
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                temporary_arc.data.next();
                return draw_nth_point;
            }
            return draw_tangent;
        }

        /// <summary>
        /// Mutator - MouseDown creates point at position
        /// </summary>
        /// <returns>The next mode for the state machine.</returns>
        private static LevelCreatorEditor.CreateShape draw_nth_point(bool force_quit = false)
        {
            if (force_quit)
            {
                return escape();
            }
            temporary_arc.data.receive_vector(LevelCreatorEditor.get_mouse_position());

            // MouseDown 2-n: create arc through point using last right-hand-side slope
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                temporary_arc.data.next();
                return draw_tangent;
            }
            return draw_nth_point;
        }

        private static LevelCreatorEditor.CreateShape escape()
        {
            // Escape: close the shape so that it meets with the original point (using original point for slope)
            if (temporary_arc.exists)
            {
                temporary_arc.data.close_shape();
                Debug.Log("Happening");
                temporary_arc = new optional<ArcBuilder>();
            }
            return LevelCreatorEditor.draw_initialize;
        }

        private static optional<ArcBuilder> temporary_arc;
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