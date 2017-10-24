using System;
using System.Collections.Generic;

public class ConcaveGeometryVisitor : GeometryVisitor
{
    public ConcaveGeometryVisitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float extrusion) : base(arc_list, arc_index, extrusion) { }

    public override GeometryVisitor move_cursor(float delta_length, float extrusion)
    {
        recalculate(delta_length, extrusion);

        // FIXME: implement
    }

    protected override void calculate_boundary(float delta_length, float extrusion)
    {
        bool right = delta_length > 0;

        // FIXME: implement
    }

    protected override void calculate_extrusion(float extrusion)
    {
        int left_index = left_arc_index.index;

        // FIXME: implement
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