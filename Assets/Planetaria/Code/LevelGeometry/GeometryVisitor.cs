using System.Collections.Generic;

public abstract class GeometryVisitor
{
    public static GeometryVisitor geometry_visitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float extrusion)
    {
        GeometryVisitor result;
        if (!arc_list[arc_index.index].exists)
        {
            result = new ConcaveGeometryVisitor();
        }
        else
        {
            result = new ConvexGeometryVisitor();
        }
        geometry_visitor(result, arc_list, arc_index, extrusion);
        return result;
    }

    private static GeometryVisitor right_visitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float rightward_length_from_boundary, float extrusion)
    {
        GeometryVisitor visitor = geometry_visitor(arc_list, arc_index.right(), extrusion);
        return visitor.set_cursor((visitor.left_angle_boundary+rightward_length_from_boundary)*(visitor.arc_angle/visitor.arc_length), extrusion);
    }

    private static GeometryVisitor left_visitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float leftward_length_from_boundary, float extrusion)
    {
        GeometryVisitor visitor = geometry_visitor(arc_list, arc_index.left(), extrusion);
        return visitor.set_cursor((visitor.right_angle_boundary-leftward_length_from_boundary)*(visitor.arc_angle/visitor.arc_length), extrusion);
    }

    protected static GeometryVisitor geometry_visitor(GeometryVisitor self, List<optional<Arc>> arc_list, ArcIndex arc_index, float extrusion)
    {
        self.arc_list_variable = arc_list;
        self.calculate_extrusion(extrusion);
        self.calculate_boundary(-1, extrusion);
        self.calculate_boundary(+1, extrusion);
        self.last_extrusion = extrusion;
        return self;
    }

    protected void recalculate(float delta_length, float extrusion)
    {
        calculate_extrusion(extrusion);
        if (extrusion != last_extrusion && delta_length != 0)
        {
            calculate_boundary(delta_length, extrusion);
        }
        last_extrusion = extrusion;
    }

    public GeometryVisitor set_cursor(float angular_position, float extrusion)
    {
        GeometryVisitor result = this;
        if (angular_position < left_angle_boundary)
        {
            float extra_length = (left_angle_boundary - angular_position) * (arc_length/arc_angle);
            result = left_visitor(arc_list_variable, arc_index_variable, extra_length, extrusion);
        }
        else if (angular_position > right_angle_boundary)
        {
            float extra_length = (angular_position - right_angle_boundary) * (arc_length/arc_angle);
            result = right_visitor(arc_list_variable, arc_index_variable, extra_length, extrusion);
        }
        return result;
    }

    public abstract GeometryVisitor move_cursor(float delta_length, float extrusion);
    protected abstract void calculate_boundary(float delta_length, float extrusion);
    protected abstract void calculate_extrusion(float extrusion);

    protected List<optional<Arc>> arc_list_variable;
    protected ArcIndex arc_index_variable;
    protected ArcIndex left_arc_index;
    protected ArcIndex right_arc_index;

    protected float angular_position;
    protected float last_extrusion;

    protected float left_angle_boundary;
    protected float right_angle_boundary;

    protected float arc_angle;
    protected float arc_length;
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