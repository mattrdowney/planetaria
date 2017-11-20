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
            if (extrusion != last_extrusion)
            {
                upkeep(delta_length, extrusion);
                last_extrusion = extrusion;
            }
            float delta_angle = delta_length * (arc_angle/arc_length);
            return set_position(angular_position + delta_angle, extrusion);
        }

        public Vector3 normal()
        {
            Debug.DrawRay(cached_position, cached_normal, Color.blue);
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

            initialize();
            upkeep(-1, extrusion);
            upkeep(+1, extrusion);
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

        protected abstract void upkeep(float delta_length, float extrusion);
        protected abstract void initialize();
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

        protected override void initialize()
        {
            left_of_left_arc = left_arc.left();
            right_of_right_arc = right_arc.right();
            arc_angle = center_arc.arc.data.angle();
            left_angle_boundary = 0;
            right_angle_boundary = arc_angle;
        }

        protected override void upkeep(float delta_length, float center_of_mass_extrusion)
        {
            float floor_length = center_arc.arc.data.length(); // edge case
            float ceiling_length = center_arc.arc.data.length(2*center_of_mass_extrusion); // corner case
            arc_length = Mathf.Max(floor_length, ceiling_length); // use longer distance to make movement feel consistent

            if (!right_arc.arc.exists && delta_length > 0) // set right boundary
            {
                Vector3 intersection = PlanetariaIntersection.arc_arc_intersection(center_arc.arc.data, right_of_right_arc.arc.data, center_of_mass_extrusion);
                right_angle_boundary = center_arc.arc.data.position_to_angle(intersection);
            }
            else if (!left_arc.arc.exists && delta_length < 0) // set left boundary // no need to redefine boundaries if player isn't moving (delta_length == 0)
            {
                Vector3 intersection = PlanetariaIntersection.arc_arc_intersection(center_arc.arc.data, left_of_left_arc.arc.data, center_of_mass_extrusion);
                left_angle_boundary = center_arc.arc.data.position_to_angle(intersection);
            }
        }

        protected override void calculate_location()
        {
            cached_position = center_arc.arc.data.position(angular_position, last_extrusion);
            cached_normal = center_arc.arc.data.normal(angular_position, last_extrusion);
        }

        ArcVisitor left_of_left_arc;
        ArcVisitor right_of_right_arc;
    }

    internal sealed class ConcaveGeometryVisitor : GeometryVisitor
    {
        public ConcaveGeometryVisitor(ArcVisitor arc_index, float extrusion) : base(arc_index, extrusion) { }

        protected override void initialize()
        {
            left_normal = left_arc.arc.data.normal(left_arc.arc.data.angle()); // intentionally no extrusion
            right_normal = right_arc.arc.data.normal(0); // these normals will be overwritten
            arc_angle = Vector3.Angle(left_normal, right_normal)*Mathf.Deg2Rad; // doesn't vary (in theory) // TODO: verify
            left_angle_boundary = 0;
            right_angle_boundary = arc_angle;
        }

        protected override void upkeep(float delta_length, float center_of_mass_extrusion)
        {
            arc_length = (2*center_of_mass_extrusion) / Mathf.Cos(arc_angle / 2);
            cached_position = PlanetariaIntersection.arc_arc_intersection(left_arc.arc.data, right_arc.arc.data, center_of_mass_extrusion);
            float left_angle = left_arc.arc.data.position_to_angle(cached_position);
            float right_angle = right_arc.arc.data.position_to_angle(cached_position);
            left_normal = left_arc.arc.data.normal(left_angle, center_of_mass_extrusion);
            right_normal = right_arc.arc.data.normal(right_angle, center_of_mass_extrusion);
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