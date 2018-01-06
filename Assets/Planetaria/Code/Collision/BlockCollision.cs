using UnityEngine;

namespace Planetaria
{
    public class BlockCollision
    {
        public static optional<BlockCollision> block_collision(CollisionObserver observer, Arc arc, Block block, PlanetariaCollider collider, PlanetariaTransform transformation, PlanetariaRigidbody rigidbody)
        {
            optional<ArcVisitor> arc_visitor = block.arc_visitor(arc);
            if (!arc_visitor.exists)
            {
                Debug.LogError("This should never happen");
                return new optional<BlockCollision>();
            }
            
            Quaternion block_to_world = block.transform.rotation;
            Quaternion world_to_block = Quaternion.Inverse(block_to_world);

            Vector3 last_position = world_to_block * transformation.previous_position.data;
            Vector3 current_position = world_to_block * transformation.position.data;

            float extrusion = transformation.scale;
            optional<Vector3> intersection_point = PlanetariaIntersection.arc_path_intersection(arc, last_position, current_position, extrusion);
            if (!intersection_point.exists) // theoretically only happens with moving objects for discrete collision checks
            {
                // these functions are general inverses of one another, but also serve to constrain/normalize the position to the arc path.
                float intersection_angle = arc.position_to_angle(current_position);
                if (intersection_angle < arc.angle()) // if the intersection is valid
                {
                    intersection_point = arc.position(intersection_angle); // set the collision to the extruded collision point
                }
            }
            if (!intersection_point.exists)
            {
                Debug.LogError("Research why this happened.");
                return new optional<BlockCollision>();
            }
            if (!platform_collision(arc, block, collider, transformation, rigidbody, intersection_point))
            {
                intersection_point = new optional<Vector3>();
            }
            BlockCollision result = new BlockCollision();
            float angle = arc.position_to_angle(intersection_point.data);
            result.geometry_visitor = GeometryVisitor.geometry_visitor(arc_visitor.data, angle, extrusion, block.transform);
            result.distance = (intersection_point.data - transformation.position.data).magnitude;
            result.block = block;
            result.collider = collider;
            result.active = true;
            result.observer = observer;
            result.self = observer.collider();
            result.other = collider;

            PlanetariaPhysicMaterial self = result.self.material;
            PlanetariaPhysicMaterial other = result.other.material;
            result.elasticity = PlanetariaPhysic.blend(
                    self.elasticity, self.elasticity_combine,
                    other.elasticity, other.elasticity_combine);

            result.dynamic_friction = PlanetariaPhysic.blend(
                    self.dynamic_friction, self.friction_combine,
                    other.dynamic_friction, other.friction_combine);

            result.static_friction = PlanetariaPhysic.blend(
                    self.static_friction, self.friction_combine,
                    other.static_friction, other.friction_combine);

            result.magnetism =
                    -(self.magnetism - other.magnetism * self.induced_magnetism_multiplier) *
                    (other.magnetism - self.magnetism * other.induced_magnetism_multiplier);

            Debug.LogError(self.magnetism - other.magnetism * self.induced_magnetism_multiplier);
            Debug.LogError(other.magnetism - self.magnetism * other.induced_magnetism_multiplier);
            Debug.LogError(result.magnetism);

            return result;
        }

        public void move(float delta_length, float extrusion)
        {
            geometry_visitor = geometry_visitor.move_position(delta_length, extrusion);
        }

        public NormalizedCartesianCoordinates position()
        {
            return new NormalizedCartesianCoordinates(geometry_visitor.position());
        }

        public NormalizedCartesianCoordinates normal()
        {
            return new NormalizedCartesianCoordinates(geometry_visitor.normal());
        }
    
        public bool active
        {
            get
            {
                return block.active && active_variable;
            }
            private set
            {
                active_variable = value;
            }
        }

        public CollisionObserver observer { get; private set; }
        public PlanetariaCollider self { get; private set; }
        public PlanetariaCollider other { get; private set; }
        public Block block { get; private set; }
        public PlanetariaCollider collider { get; private set; }
        public float distance { get; private set; }
        public GeometryVisitor geometry_visitor { get; private set; }
        public float elasticity { get; private set; }
        public float dynamic_friction { get; private set; }
        public float static_friction { get; private set; }
        public float magnetism { get; private set; }

        private static bool platform_collision(Arc arc, Block block, PlanetariaCollider collider, PlanetariaTransform transformation, PlanetariaRigidbody rigidbody, optional<Vector3> intersection_point)
        {
            Vector3 velocity = Bearing.attractor(transformation.previous_position.data, transformation.position.data);

            if (block.is_platform && intersection_point.exists)
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