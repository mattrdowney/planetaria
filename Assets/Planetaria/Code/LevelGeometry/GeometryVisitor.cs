using UnityEngine;

namespace Planetaria
{
    public abstract class GeometryVisitor
    {
        public static GeometryVisitor geometry_visitor(ArcVisitor arc_visitor, float angular_position, float extrusion)
        {
            GeometryVisitor result = geometry_visitor(arc_visitor, extrusion);
            return result.set_position(angular_position, extrusion);
        }

        public GeometryVisitor move_position(float delta_length, float extrusion)
        {
            recalculate(delta_length, extrusion);
            float delta_angle = delta_length * (arc_angle/arc_length);
            return set_position(angular_position + delta_angle, extrusion);
        }

        public Vector3 normal()
        {
            return cached_normal;
        }

        public Vector3 position()
        {
            return cached_position;
        }

        protected GeometryVisitor(ArcVisitor arc_visitor, float extrusion)
        {
            center_arc = arc_visitor;
            left_arc = arc_visitor.left();
            right_arc = arc_visitor.right();

            calculate_extrusion(extrusion);
            calculate_boundary(-1, extrusion);
            calculate_boundary(+1, extrusion);
            last_extrusion = extrusion;
        }

        private static GeometryVisitor geometry_visitor(ArcVisitor arc_visitor, float extrusion)
        {
            GeometryVisitor result;
            if (!arc_visitor.arc.exists)
            {
                result = new ConcaveGeometryVisitor(arc_visitor, extrusion);
            }
            else
            {
                result = new ConvexGeometryVisitor(arc_visitor, extrusion);
            }

            return result;
        }

        private static GeometryVisitor right_visitor(ArcVisitor arc_visitor, float rightward_length_from_boundary, float extrusion)
        {
            GeometryVisitor visitor = geometry_visitor(arc_visitor.right(), extrusion);
            return visitor.set_position(visitor.left_angle_boundary + rightward_length_from_boundary*(visitor.arc_angle/visitor.arc_length), extrusion);
        }

        private static GeometryVisitor left_visitor(ArcVisitor arc_visitor, float leftward_length_from_boundary, float extrusion)
        {
            GeometryVisitor visitor = geometry_visitor(arc_visitor.left(), extrusion);
            return visitor.set_position(visitor.right_angle_boundary - leftward_length_from_boundary*(visitor.arc_angle/visitor.arc_length), extrusion);
        }

        private void recalculate(float delta_length, float extrusion)
        {
            if (extrusion != last_extrusion)
            {
                calculate_extrusion(extrusion);
                calculate_boundary(delta_length, extrusion);
                last_extrusion = extrusion;
            }
        }

        private GeometryVisitor set_position(float angular_position, float extrusion)
        {
            GeometryVisitor result = this;
            if (angular_position < left_angle_boundary)
            {
                float extra_length = Mathf.Abs((left_angle_boundary - angular_position) * (arc_length/arc_angle));
                result = left_visitor(center_arc, extra_length, extrusion);
            }
            else if (angular_position > right_angle_boundary)
            {
                float extra_length = Mathf.Abs((angular_position - right_angle_boundary) * (arc_length/arc_angle));
                result = right_visitor(center_arc, extra_length, extrusion);
            }
            this.angular_position = angular_position;
            calculate_location();
            return result;
        }

        protected abstract void calculate_boundary(float delta_length, float extrusion);
        protected abstract void calculate_extrusion(float center_of_mass_extrusion);
        protected abstract void calculate_location();
    
        protected ArcVisitor center_arc;
        protected ArcVisitor left_arc;
        protected ArcVisitor right_arc;

        protected Vector3 cached_position;
        protected Vector3 cached_normal;

        protected float angular_position;
        protected float last_extrusion = float.NaN;

        protected float left_angle_boundary;
        protected float right_angle_boundary;

        protected float arc_angle;
        protected float arc_length;
    }

    internal sealed class ConvexGeometryVisitor : GeometryVisitor
    {
        public ConvexGeometryVisitor(ArcVisitor arc_index, float extrusion) : base(arc_index, extrusion) { }

        protected override void calculate_boundary(float delta_length, float extrusion)
        {
            if (delta_length != 0) // no need to redefine boundaries if player isn't moving
            {
                bool right = delta_length > 0;

                // FIXME: implement
                left_angle_boundary = 0;
                right_angle_boundary = arc_angle;
            }
        }

        protected override void calculate_extrusion(float center_of_mass_extrusion)
        {
            arc_angle = center_arc.arc.data.angle();
            float floor_length = center_arc.arc.data.length(); // edge case
            float ceiling_length = center_arc.arc.data.length(2*center_of_mass_extrusion); // corner case
            arc_length = Mathf.Max(floor_length, ceiling_length); // use longer distance to make movement feel consistent
        }

        protected override void calculate_location()
        {
            cached_position = center_arc.arc.data.position(angular_position, last_extrusion);
            cached_normal = center_arc.arc.data.normal(angular_position, last_extrusion);
            Debug.DrawRay(cached_position, cached_normal, Color.blue);
        }
    }

    internal sealed class ConcaveGeometryVisitor : GeometryVisitor
    {
        public ConcaveGeometryVisitor(ArcVisitor arc_index, float extrusion) : base(arc_index, extrusion) { }

        protected override void calculate_boundary(float delta_length, float extrusion)
        {
            // Note: right vs. left doesn't matter, both boundaries need to be recalculated
            // FIXME: implement
            left_angle_boundary = 0;
            right_angle_boundary = arc_angle;

            Vector3 left_position = left_arc.arc.data.position(left_angle_boundary, extrusion);
            Vector3 right_position = right_arc.arc.data.position(right_angle_boundary, extrusion);
            // Assert left_position == right_position
            cached_position = (left_position+right_position)/2; // FIXME: unneccessary
        }

        protected override void calculate_extrusion(float center_of_mass_extrusion)
        {
            left_normal = left_arc.arc.data.normal(left_angle_boundary, center_of_mass_extrusion);
            right_normal = right_arc.arc.data.normal(right_angle_boundary, center_of_mass_extrusion);
            arc_angle = Vector3.Angle(left_normal, right_normal);
            arc_length = 2 * center_of_mass_extrusion / Mathf.Sin(arc_angle / 2);
        }

        protected override void calculate_location()
        {
            cached_normal = Vector3.Slerp(left_normal, right_normal, angular_position/arc_angle);
        }

        Vector3 left_normal;
        Vector3 right_normal;
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