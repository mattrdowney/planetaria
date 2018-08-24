﻿using UnityEngine;

namespace Planetaria
{
    public /*sealed?*/ class GeometryVisitor
    {
        /// <summary>
        /// Constructor (Named) -
        /// </summary>
        /// <param name="arc_visitor"></param>
        /// <param name="angular_position"></param>
        /// <param name="extrusion"></param>
        /// <param name="transformation"></param>
        /// <returns></returns>
        public static GeometryVisitor geometry_visitor(ArcVisitor arc_visitor, float angular_position, float extrusion, Transform transformation)
        {
            GeometryVisitor result = new GeometryVisitor(arc_visitor, extrusion, transformation);
            return result.set_position(angular_position, extrusion);
        }

        /// <summary>
        /// Constructor -
        /// </summary>
        /// <param name="arc_visitor"></param>
        /// <param name="extrusion"></param>
        /// <param name="transformation"></param>
        private GeometryVisitor(ArcVisitor arc_visitor, float extrusion, Transform transformation)
        {
            this.arc_visitor = arc_visitor;
            block_transform = transformation;
            arc_angle = arc_visitor.arc.angle();
            left_angle_boundary = -arc_angle/2;
            right_angle_boundary = +arc_angle/2;
            offset = extrusion;
        }

        public static bool concave(Arc arc, float extrusion)
        {
            bool concave_surfaced = arc.type == GeometryType.ConcaveCorner && extrusion > 0;
            bool concave_burrowed = arc.type == GeometryType.ConvexCorner && extrusion < 0;
            bool concave = concave_surfaced || concave_burrowed;
            return concave;
        }

        public GeometryVisitor move_position(float delta_length, optional<float> extrusion = new optional<float>())
        {
            if (extrusion.exists)
            {
                offset = extrusion.data;
            }
            float delta_angle = delta_length * (arc_angle/arc_length);
            return set_position(angular_position + delta_angle, offset);
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

        public bool contains(Vector3 position) // FIXME: bug with GeometryType.ConcaveCorner (negative extrusion)
        {
            position.Normalize(); // FIXME ? : this is an approximation

            if (block_transform.rotation != Quaternion.identity)
            {
                position = Quaternion.Inverse(block_transform.rotation)*position;
            }

            bool left_contains = arc_visitor[-1].contains(position, offset);
            bool center_contains = arc_visitor[0].contains(position, offset);
            bool right_contains = arc_visitor[+1].contains(position, offset);

            return left_contains || center_contains || right_contains;
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
                extrude(value);
                calculate_location();
                last_extrusion = value;
            }
        }

        private static GeometryVisitor right_visitor(ArcVisitor arc_visitor, float rightward_length_from_boundary, float extrusion, Transform transformation)
        {
            GeometryVisitor visitor = new GeometryVisitor(arc_visitor.right(), extrusion, transformation);
            return visitor.set_position(visitor.left_angle_boundary + rightward_length_from_boundary*(visitor.arc_angle/visitor.arc_length), extrusion);
        }

        private static GeometryVisitor left_visitor(ArcVisitor arc_visitor, float leftward_length_from_boundary, float extrusion, Transform transformation)
        {
            GeometryVisitor visitor = new GeometryVisitor(arc_visitor.left(), extrusion, transformation);
            return visitor.set_position(visitor.right_angle_boundary - leftward_length_from_boundary*(visitor.arc_angle/visitor.arc_length), extrusion);
        }

        private GeometryVisitor set_position(float angular_position, float extrusion)
        {
            GeometryVisitor result = this;
            if (angular_position < left_angle_boundary)
            {
                float extra_length = Mathf.Abs((left_angle_boundary - angular_position) * (arc_length/arc_angle));
                result = left_visitor(arc_visitor, extra_length, extrusion, block_transform);
            }
            else if (angular_position > right_angle_boundary)
            {
                float extra_length = Mathf.Abs((angular_position - right_angle_boundary) * (arc_length/arc_angle));
                result = right_visitor(arc_visitor, extra_length, extrusion, block_transform);
            }
            this.angular_position = angular_position;
            calculate_location();
            return result;
        }

        private void extrude(float center_of_mass_extrusion)
        {
            float floor_length = arc_visitor.arc.length(); // edge case
            float ceiling_length = arc_visitor.arc.length(2*center_of_mass_extrusion); // corner case
            arc_length = Mathf.Max(floor_length, ceiling_length); // use longer distance to make movement feel consistent

            if (concave(arc_visitor[+1], center_of_mass_extrusion)) // set right boundary
            {
                optional<Vector3> intersection = PlanetariaIntersection.arc_arc_intersection(arc_visitor[0], arc_visitor[+2], center_of_mass_extrusion);
                right_angle_boundary = arc_visitor.arc.position_to_angle(intersection.data);
            }
            else if (concave(arc_visitor[-1], center_of_mass_extrusion)) // set left boundary // no need to redefine boundaries if player isn't moving (delta_length == 0)
            {
                optional<Vector3> intersection = PlanetariaIntersection.arc_arc_intersection(arc_visitor[0], arc_visitor[-2], center_of_mass_extrusion);
                left_angle_boundary = arc_visitor.arc.position_to_angle(intersection.data);
            }
        }

        private void calculate_location()
        {
            if (arc_visitor.arc.type != GeometryType.ConcaveCorner)
            {
                cached_position = arc_visitor.arc.position(angular_position, offset);
                cached_normal = arc_visitor.arc.normal(angular_position, offset);
            }
            else
            {
                cached_position = arc_visitor.arc.position(angular_position, Mathf.Abs(offset));
                cached_normal = arc_visitor.arc.normal(angular_position, Mathf.Abs(offset));
            }
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