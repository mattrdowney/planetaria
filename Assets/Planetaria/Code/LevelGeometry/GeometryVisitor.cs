using System.Collections.Generic;

public abstract class GeometryVisitor
{
    public static GeometryVisitor geometry_visitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float angle)
    {
        if (!arc_list[arc_index.index].exists)
        {
            return new ConcaveGeometryVisitor(arc_list, arc_index, angle);
        }

        return new ConvexGeometryVisitor(arc_list, arc_index, angle);
    }

    protected static GeometryVisitor geometry_visitor(GeometryVisitor self, List<optional<Arc>> arc_list, float angle)
    {
        self.arc_list_variable = arc_list;
        self.cursor_position = angle;
        return self;
    }

    protected abstract void recalculate_boundaries(float delta_length, float extrusion);

    protected float cursor_position;
    protected float last_extrusion = float.NaN;
    protected float left_angle_boundary;
    protected float right_angle_boundary;
    protected List<optional<Arc>> arc_list_variable;
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