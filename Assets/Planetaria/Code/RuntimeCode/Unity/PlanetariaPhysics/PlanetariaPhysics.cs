using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Planetaria
{
    public static class PlanetariaPhysics
    {
        public static float blend(float left, PlanetariaPhysicMaterialCombine left_type, // TODO: put this somewhere else?
                float right, PlanetariaPhysicMaterialCombine right_type)
        {
            PlanetariaPhysicMaterialCombine type = (left_type >= right_type ? left_type : right_type);
            switch(type) // meta-commentary: I wonder if many languages set up function addresses deterministically such that the minimum function pointer can be used e.g. func = min(fp1, fp2); (*func)(parameters);
            {
                // Note: all functions map from (0,0)->0 and (1,1)->1 [and (c,c)->c for c >= 0]
                // values (positive and negative) outside this range can still be used
                case PlanetariaPhysicMaterialCombine.Minimum: return Mathf.Min(left, right);
                case PlanetariaPhysicMaterialCombine.Harmonic: return left+right != 0 ? 2*left*right/(left + right) : 0; // avoid division by zero
                case PlanetariaPhysicMaterialCombine.Geometric: return Mathf.Sign(left*right) == +1 ? Mathf.Sqrt(left*right) : 0; // sqrt(negative) is undefined
                case PlanetariaPhysicMaterialCombine.Average: return (left + right)/2;
                case PlanetariaPhysicMaterialCombine.Quadratic: return Mathf.Sqrt((left*left + right*right)/2);
            }
            /*case PlanetariaPhysicMaterialCombine.Maximum:*/ return Mathf.Max(left, right);
        }

        /*
        private static PlanetariaCollider[] overlap_circle(Vector3 position, float radius, int layer_mask = Physics.DefaultRaycastLayers)
        */

        /// <summary>
        /// Inspector - Finds the collision points between an arc extrapolated to be distance long (ordered by distance)
        /// </summary>
        /// <param name="arc">A fragment that defines the arc in space (might not be fully used or return collisions after the end of the arc).</param>
        /// <param name="distance">The distance to raycast (may be greater than or less than the length of the arc - or negative).</param>
        /// <param name="layer_mask">The collision mask that defines which objects will be ignored.</param>
        /// <returns>All of the collision points of the Raycast (listed exactly once).</returns>
        public static optional<PlanetariaRaycastHit> raycast(Arc arc, float distance, int layer_mask = Physics.DefaultRaycastLayers, QueryTriggerInteraction query_field_interaction = QueryTriggerInteraction.UseGlobal)
        {
            PlanetariaRaycastHit[] raycast_information = raycast_all(arc, distance, layer_mask, query_field_interaction); // HACK: TODO: optimize
            return raycast_information.Length > 0 ? raycast_information[0] : new optional<PlanetariaRaycastHit>();
        }

        /// <summary>
        /// Inspector - Finds the collision points between an arc (ordered by distance)
        /// </summary>
        /// <param name="arc">The arc that will detect collisions.</param>
        /// <returns>All of the collision points of the Raycast along the arc (listed exactly once).</returns>
        public static PlanetariaRaycastHit[] raycast_all(Arc arc)
        {
            return raycast_all(arc, arc.length(), Physics.DefaultRaycastLayers);
        }

        /// <summary>
        /// Inspector - Finds the collision points between an arc extrapolated to be distance long (ordered by distance)
        /// </summary>
        /// <param name="arc">A fragment that defines the arc in space (might not be fully used or return collisions after the end of the arc).</param>
        /// <param name="distance">The distance to raycast (may be greater than or less than the length of the arc - or negative).</param>
        /// <param name="layer_mask">The collision mask that defines which objects will be ignored.</param>
        /// <returns>All of the collision points of the Raycast (listed exactly once).</returns>
        public static PlanetariaRaycastHit[] raycast_all(Arc arc, float distance, int layer_mask = Physics.DefaultRaycastLayers, QueryTriggerInteraction query_field_interaction = QueryTriggerInteraction.UseGlobal)
        {
            bool collide_with_fields = Physics.queriesHitTriggers; // QueryTriggerInteraction.UseGlobal
            if (query_field_interaction == QueryTriggerInteraction.Collide)
            {
                collide_with_fields = true;
            }
            else if (query_field_interaction == QueryTriggerInteraction.Ignore)
            {
                collide_with_fields = false;
            }

            List<PlanetariaRaycastHit> result = unordered_raycast_all(arc, distance, layer_mask, collide_with_fields).ToList();

            Debug.Log("Unordered raycast count: " + result.Count);

            RaycastSorter sorter = RaycastSorter.sorter(arc, distance);

            result.Sort(sorter);

            return result.ToArray();
        }

        /// <summary>
        /// Inspector - Finds the collision points between an arc extrapolated to be distance long (the PlanetariaRaycastHit structs have no particular order)
        /// </summary>
        /// <param name="arc">A fragment that defines the arc in space (might not be fully used or return collisions after the end of the arc).</param>
        /// <param name="distance">The distance to raycast (may be greater than or less than the length of the arc - or negative).</param>
        /// <param name="layer_mask">The collision mask that defines which objects will be ignored.</param>
        /// <returns>All of the collision points of the Raycast (listed exactly once).</returns>
        private static PlanetariaRaycastHit[] unordered_raycast_all(Arc arc, float distance, int layer_mask, bool collide_with_fields)
        {
            //float angle = arc.angle(); // this would determine the intersections for the un-modified arc (ignoring distance)
            
            // desired_angle = desired_length * (partial_angle/partial_length) i.e. length * length_to_angle ratio
            float desired_angle = distance * (arc.angle() / arc.length()); // TODO: verify negative distances go backwards
            desired_angle = Mathf.Clamp(desired_angle, -2*Mathf.PI, 2*Mathf.PI);

            // primative arc points
            Vector3 arc_left = arc.position(-arc.angle()/2);
            Vector3 arc_center = arc.position(-arc.angle()/2 + desired_angle/2);
            Vector3 arc_right = arc.position(-arc.angle()/2 + desired_angle);

            SerializedArc ray_arc = ArcFactory.curve(arc_left, arc_center, arc_right);

            PlanetariaShape ray_shape = PlanetariaShape.Create(new List<SerializedArc> { ray_arc }, false);

            // composites
            Vector3 arc_boundary_midpoint = (arc_left + arc_right) / 2; // if the arc is like a wooden bow, this is the midpoint of the string
            Vector3 arc_forward = (arc_center - arc_boundary_midpoint).normalized; // the direction a hypothetical arrow would travel
            Vector3 arc_up = arc.floor().normal; // orthogonal/perpendicular to the imaginary "bow"

            // UnityEngine.Physics.OverlapBox() requirements
            // FIXME: OPTIMIZE: half_extents currently provides unnecessary false positives because the "width" of plane (the depth into the distance and zero height are fine)
            Vector3 half_extents = new Vector3(1, 0, 1); // The largest collision "box" for a unit sphere is a radius of 1 in the x-z plane; height along y is 0.
            Vector3 center = arc_boundary_midpoint + arc_forward*1; // The center of the "box" must be offset 1 (the radius) along the forward axis from the two arc boundaries.
            Quaternion rotation = Quaternion.LookRotation(arc_forward, arc_up);

            // SphereColliders (only) that represent potential collisions (not guaranteed).
            Collider[] colliders = Physics.OverlapBox(center, half_extents, rotation, layer_mask, QueryTriggerInteraction.Collide); // TODO: verify this casts properly
            List<PlanetariaRaycastHit> raycast_hits = new List<PlanetariaRaycastHit>();
            Debug.Log(colliders.Length);
            foreach (SphereCollider sphere_collider in colliders)
            {
                PlanetariaCollider planetaria_collider = PlanetariaCache.self.collider_fetch(sphere_collider);
                if (planetaria_collider.is_field && !collide_with_fields)
                {
                    Debug.LogError("Why?");
                    continue;
                }
                Quaternion geometry_rotation = planetaria_collider.gameObject.internal_game_object.transform.rotation;
                Debug.Log("Found a collider with " + planetaria_collider.shape.Length + " arcs.");
                foreach (Arc geometry_arc in ray_shape.block_collision(planetaria_collider.shape, geometry_rotation))
                {
                    Vector3[] intersections = PlanetariaIntersection.raycast_intersection(arc, geometry_arc, distance, geometry_rotation); // TODO: verify distance is indeed the angle in this scenario
                    Debug.Log("Found an arc with " + intersections.Length + " intersections.");
                    foreach (Vector3 intersection in intersections)
                    {
                        PlanetariaRaycastHit single_collision = PlanetariaRaycastHit.hit(arc, planetaria_collider, geometry_arc, intersection, distance);
                        raycast_hits.Add(single_collision);
                    }
                }
            }
            return raycast_hits.ToArray();
        }
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