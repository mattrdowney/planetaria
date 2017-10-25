using System.Collections.Generic;
using UnityEngine;

public abstract class GeometryVisitor
{
    public GeometryVisitor geometry_visitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float angular_position, float extrusion)
    {
        GeometryVisitor result = geometry_visitor(arc_list, arc_index, extrusion);
        return result.set_position(angular_position, extrusion);
    }

    private static GeometryVisitor geometry_visitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float extrusion)
    {
        GeometryVisitor result;
        if (!arc_list[arc_index.index].exists)
        {
            result = new ConcaveGeometryVisitor(arc_list, arc_index, extrusion);
        }
        else
        {
            result = new ConvexGeometryVisitor(arc_list, arc_index, extrusion);
        }

        return result;
    }

    protected GeometryVisitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float extrusion)
    {
        arc_list_variable = arc_list;

        arc_index_variable = arc_index;
        left_arc_index = arc_index.left();
        right_arc_index = arc_index.right();

        calculate_extrusion(extrusion);
        calculate_boundary(-1, extrusion);
        calculate_boundary(+1, extrusion);
        last_extrusion = extrusion;
    }

    private static GeometryVisitor right_visitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float rightward_length_from_boundary, float extrusion)
    {
        GeometryVisitor visitor = geometry_visitor(arc_list, arc_index.right(), extrusion);
        return visitor.set_position((visitor.left_angle_boundary+rightward_length_from_boundary)*(visitor.arc_angle/visitor.arc_length), extrusion);
    }

    private static GeometryVisitor left_visitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float leftward_length_from_boundary, float extrusion)
    {
        GeometryVisitor visitor = geometry_visitor(arc_list, arc_index.left(), extrusion);
        return visitor.set_position((visitor.right_angle_boundary-leftward_length_from_boundary)*(visitor.arc_angle/visitor.arc_length), extrusion);
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

    public GeometryVisitor set_position(float angular_position, float extrusion)
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

    public GeometryVisitor move_position(float delta_length, float extrusion)
    {
        recalculate(delta_length, extrusion);
        float delta_angle = delta_length * (arc_angle/arc_length);
        return set_position(angular_position + delta_angle, extrusion);
    }

    protected abstract void calculate_boundary(float delta_length, float extrusion);
    protected abstract void calculate_extrusion(float extrusion);

    protected List<optional<Arc>> arc_list_variable;
    protected ArcIndex arc_index_variable;
    protected ArcIndex left_arc_index;
    protected ArcIndex right_arc_index;

    protected float angular_position;
    protected float last_extrusion = float.NaN;

    protected float left_angle_boundary;
    protected float right_angle_boundary;

    protected float arc_angle;
    protected float arc_length;
}

internal sealed class ConvexGeometryVisitor : GeometryVisitor
{
    public ConvexGeometryVisitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float extrusion) : base(arc_list, arc_index, extrusion) { }

    protected override void calculate_boundary(float delta_length, float extrusion)
    {
        bool right = delta_length > 0;

        // FIXME: implement
    }

    protected override void calculate_extrusion(float extrusion)
    {
        int index = arc_index_variable.index;
        arc_length = arc_list_variable[index].data.length(extrusion);
        arc_angle = arc_list_variable[index].data.angle(extrusion);
    }
}

internal sealed class ConcaveGeometryVisitor : GeometryVisitor
{
    public ConcaveGeometryVisitor(List<optional<Arc>> arc_list, ArcIndex arc_index, float extrusion) : base(arc_list, arc_index, extrusion) { }

    protected override void calculate_boundary(float delta_length, float extrusion)
    {
        bool right = delta_length > 0;

        // FIXME: implement
    }

    protected override void calculate_extrusion(float extrusion)
    {
        int left_index = left_arc_index.index;
        int right_index = right_arc_index.index;

        Vector3 left_normal = arc_list_variable[left_index].data.normal(left_angle_boundary, extrusion);
        Vector3 right_normal = arc_list_variable[right_index].data.normal(right_angle_boundary, extrusion);
        arc_angle = Vector3.Angle(left_normal, right_normal);
        arc_length = extrusion / Mathf.Sin(arc_angle / 2);
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