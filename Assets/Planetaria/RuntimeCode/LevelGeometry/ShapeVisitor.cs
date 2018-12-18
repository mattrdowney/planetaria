using UnityEngine;

namespace Planetaria
{
    public /*sealed?*/ class ShapeVisitor
    {
        /// <summary>
        /// Constructor (Named) -
        /// </summary>
        /// <param name="arc_visitor"></param>
        /// <param name="angular_position"></param>
        /// <param name="extrusion"></param>
        /// <param name="transformation"></param>
        /// <returns></returns>
        public static ShapeVisitor geometry_visitor(ArcVisitor arc_visitor, float angular_position, float extrusion, Transform transformation)
        {
            ShapeVisitor result = new ShapeVisitor(arc_visitor, extrusion, transformation);
            result.set_position(angular_position);
            return result;
        }

        private ShapeVisitor(ArcVisitor arc_visitor, float extrusion, Transform transformation)
        {
            this.arc_visitor = arc_visitor;
            block_transform = transformation;
            last_extrusion = extrusion;
            angular_position = 0;
            initialize();
            calculate_location();
        }

        public static bool concave(Arc arc, float extrusion)
        {
            bool concave_surfaced = arc.type == ArcType.ConcaveCorner && extrusion > 0;
            bool concave_burrowed = arc.type == ArcType.ConvexCorner && extrusion < 0;
            bool concave = concave_surfaced || concave_burrowed;
            return concave;
        }

        public void move_position(float delta_length, optional<float> extrusion = new optional<float>()) // CONSIDER: combine move_position/set_position?
        {
            if (extrusion.exists)
            {
                last_extrusion = extrusion.data;
                initialize();
            }
            float delta_angle = delta_length * (arc_angle/arc_length);
            set_position(angular_position + delta_angle);
        }

        public Vector3 normal()
        {
            if (block_transform.rotation != Quaternion.identity) // Cannot be cached since platform may move
            {
                return block_transform.rotation * cached_normal;
            }
            return cached_normal;
        }

        public Vector3 position()
        {
            if (block_transform.rotation != Quaternion.identity) // Cannot be cached since platform may move
            {
                return block_transform.rotation * cached_position;
            }
            return cached_position;
        }

        public bool contains(Vector3 position) // this is intended for detecting if the PlanetariaRigidbody should derail.
        {
            position.Normalize(); // FIXME ? : this is an approximation

            if (block_transform.rotation != Quaternion.identity)
            {
                position = Quaternion.Inverse(block_transform.rotation) * position;
            }

            float extrusion = offset * (1 + Precision.threshold);
            bool center_contains = arc_visitor.arc.contains(position, extrusion); // generally, the player stays on the current arc
            bool left_contains = arc_visitor[-1].contains(position, extrusion); // ocasionally, you switch to the next arc: left...
            bool right_contains = arc_visitor[+1].contains(position, extrusion); // ... or right
            return center_contains || left_contains || right_contains; // if left, right, and center do not contain, then derail
        }

        /// <summary>
        /// The (positive or negative) offset from the path. Negative for (at least partially) submerged objects; scale/2 for typical objects.
        /// </summary>
        public float offset
        {
            get
            {
                return last_extrusion;
            }
            set
            {
                last_extrusion = value;
                initialize();
                calculate_location();
            }
        }

        private void set_position(float angular_position)
        {
            this.angular_position = angular_position;
            if (angular_position < left_angle_boundary)
            {
                float extra_length = Mathf.Abs((left_angle_boundary - angular_position) * (arc_length / arc_angle));
                arc_visitor = arc_visitor.left();
                initialize(); // re-initialize
                angular_position = right_angle_boundary;
                set_position(angular_position - extra_length * (arc_angle / arc_length));
            }
            else if (angular_position > right_angle_boundary)
            {
                float extra_length = Mathf.Abs((angular_position - right_angle_boundary) * (arc_length / arc_angle));
                arc_visitor = arc_visitor.right();
                initialize(); // re-initialize
                angular_position = left_angle_boundary;
                set_position(angular_position + extra_length * (arc_angle / arc_length));
            }
            calculate_location();
        }

        private void initialize()
        {
            arc_angle = arc_visitor.arc.angle();

            float floor_length = arc_visitor.arc.length(); // edge case
            float ceiling_length = arc_visitor.arc.length(2*offset); // corner case
            arc_length = Mathf.Max(floor_length, ceiling_length); // use longer distance to make movement feel consistent

            left_angle_boundary = -arc_angle/2;
            if (concave(arc_visitor[-1], offset)) // set left boundary
            {
                optional<Vector3> intersection = PlanetariaIntersection.arc_arc_intersection(arc_visitor[0], arc_visitor[-2], offset);
                left_angle_boundary = arc_visitor.arc.position_to_angle(intersection.data);
            }

            right_angle_boundary = +arc_angle/2;
            if (concave(arc_visitor[+1], offset)) // set right boundary
            {
                optional<Vector3> intersection = PlanetariaIntersection.arc_arc_intersection(arc_visitor[0], arc_visitor[+2], offset);
                right_angle_boundary = arc_visitor.arc.position_to_angle(intersection.data);
            }
        }

        private void calculate_location()
        {
            cached_position = arc_visitor.arc.position(angular_position, offset);
            cached_normal = arc_visitor.arc.normal(angular_position, offset);
            Debug.DrawRay(cached_position, cached_normal, Color.green);
        }
    
        private Transform block_transform;
        private ArcVisitor arc_visitor;

        private Vector3 cached_position;
        private Vector3 cached_normal;

        private float angular_position;
        private float last_extrusion = float.NaN;

        private float left_angle_boundary;
        private float right_angle_boundary;

        private float arc_angle;
        private float arc_length;
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