﻿using UnityEngine;

namespace Planetaria
{
    public class BlockCollision
    {
        public static optional<BlockCollision> block_collision(CollisionObserver observer, Arc arc, PlanetariaCollider collider, PlanetariaTransform transformation, PlanetariaRigidbody rigidbody)
        {
            optional<ArcVisitor> arc_visitor = collider.shape.arc_visitor(arc);
            if (!arc_visitor.exists)
            {
                Debug.LogError("This should never happen");
                return new optional<BlockCollision>();
            }
            
            Quaternion block_to_world = collider.gameObject.internal_game_object.transform.rotation;
            Quaternion world_to_block = Quaternion.Inverse(block_to_world);

            Vector3 last_position = world_to_block * rigidbody.get_previous_position();
            Vector3 current_position = world_to_block * rigidbody.get_position();

            float extrusion = transformation.scale/2;
            optional<Vector3> intersection_point = PlanetariaIntersection.arc_path_intersection(arc, last_position, current_position, extrusion);
            if (!intersection_point.exists) // theoretically only happens with moving objects for discrete collision checks
            {
                // these functions are general inverses of one another, but also serve to constrain/normalize the position to the arc path.
                float intersection_angle = arc.position_to_angle(current_position);
                if (Mathf.Abs(intersection_angle) <= arc.angle()/2) // if the intersection is valid
                {
                    intersection_point = arc.position(intersection_angle); // set the collision to the extruded collision point
                }
            }
            if (!intersection_point.exists)
            {
                Debug.LogError("Research why this happened.");
                return new optional<BlockCollision>();
            }
            BlockCollision result = new BlockCollision();
            float angle = arc.position_to_angle(intersection_point.data);
            result.geometry_visitor = ShapeVisitor.geometry_visitor(arc_visitor.data, angle, extrusion, collider.gameObject.internal_game_object.transform);
            intersection_point.data = block_to_world * intersection_point.data;
            result.distance = Vector3.Angle(intersection_point.data, rigidbody.get_previous_position())*Mathf.Deg2Rad;
            result.overshoot = Vector3.Angle(intersection_point.data, rigidbody.get_position())*Mathf.Deg2Rad;
            result.observer = observer;
            result.self = observer.collider();
            result.other = collider;

            PlanetariaPhysicMaterial self = result.self.material;
            PlanetariaPhysicMaterial other = result.other.material;

            result.elasticity = PlanetariaPhysics.blend(
                    self.elasticity, self.elasticity_combine,
                    other.elasticity, other.elasticity_combine);

            result.friction = PlanetariaPhysics.blend(
                    self.friction, self.friction_combine,
                    other.friction, other.friction_combine);

            result.magnetism =
                    -(self.magnetism - other.magnetism * self.induced_magnetism_multiplier) *
                    (other.magnetism - self.magnetism * other.induced_magnetism_multiplier);

            return result;
        }

        public CollisionObserver observer { get; private set; }
        public PlanetariaCollider self { get; private set; }
        public PlanetariaCollider other { get; private set; }
        public float distance { get; private set; }
        public float overshoot { get; private set; }
        public ShapeVisitor geometry_visitor { get; private set; }
        public float elasticity { get; private set; }
        public float friction { get; private set; }
        public float threshold_angle { get; private set; }
        public float magnetism { get; private set; }

        public bool grounded(Vector3 velocity)
        {
            return geometry_visitor.contains(PlanetariaMath.move(geometry_visitor.position(), velocity*Time.deltaTime));
        }

        private static bool platform_collision(Arc arc, PlanetariaCollider collider, PlanetariaTransform transformation, PlanetariaRigidbody rigidbody, optional<Vector3> intersection_point)
        {
            Vector3 velocity = Bearing.attractor(rigidbody.get_previous_position(), rigidbody.get_position());

            if (intersection_point.exists)
            {
                float arc_angle = arc.position_to_angle(intersection_point.data);
                Vector3 normal = arc.normal(arc_angle);
                bool upward_facing_normal = Vector3.Dot(normal, rigidbody.get_acceleration()) <= 0;
                bool moving_toward = Vector3.Dot(normal, velocity) <= 0;

                if (upward_facing_normal && moving_toward)
                {
                    return true;
                }
            }
            return false;
        }

        private bool active_variable;
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