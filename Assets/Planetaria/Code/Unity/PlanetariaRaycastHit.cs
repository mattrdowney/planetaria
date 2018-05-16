using UnityEngine;

namespace Planetaria
{
    public class PlanetariaRaycastHit
    {
        /// <summary>
        /// Named Constructor - creates the Planetaria equivalent of UnityEngine.RaycastHit
        /// </summary>
        /// <returns>The result of a point sweep / raycast in spherical 2D space.</returns>
        public static PlanetariaRaycastHit hit(Arc raycast_arc, SphereCollider sphere_collider, Vector3 intersection_point, float raycast_distance)
        {
            return new PlanetariaRaycastHit(raycast_arc, sphere_collider, intersection_point, raycast_distance);
        }

        
        //barycentricCoordinate	The barycentric coordinate of the triangle we hit. // RESEARCH: are there any equivalents to triangles I should be creating?
        //lightmapCoord	The uv lightmap coordinate at the impact point.
        //textureCoord	The uv texture coordinate at the collision location.
        //textureCoord2	The secondary uv texture coordinate at the impact point.
        //triangleIndex	The index of the triangle that was hit.

        /// <summary>
        /// Constructor - creates the Planetaria equivalent of UnityEngine.RaycastHit
        /// </summary>
        /// <param name="raycast_arc">The path along which the raycast (inverse path for negative distances).</param>
        /// <param name="geometry_arc">The arc which intersects the raycast (i.e. the arc hit).</param>
        /// <param name="intersection_point">The intersection point of two circular arcs in 3D space.</param>
        /// <returns>The result of a point sweep / raycast in spherical 2D space.</returns>
        private PlanetariaRaycastHit(Arc raycast_arc, SphereCollider sphere_collider, Vector3 intersection_point, float raycast_distance)
        {
            arc = PlanetariaCache.arc_cache.get(sphere_collider).data;
            block = PlanetariaCache.block_cache.get(sphere_collider).data; // FIXME:
            collider = PlanetariaCache.collider_cache.get(sphere_collider).data;

            distance = raycast_arc.position_to_angle(intersection_point) * (raycast_arc.length()/raycast_arc.angle()); // TODO: verify
            if (raycast_distance < 0)
            {
                distance = 2*Mathf.PI - distance;
            }
            positive_face_collision = true; // FIXME: HACK: LAZY: 
            normal = raycast_arc.normal(raycast_arc.position_to_angle(intersection_point));
            point = intersection_point;
            rigidbody = block.GetComponent<PlanetariaRigidbody>();
            transform = block.GetOrAddComponent<PlanetariaTransform>();
        }

        /// <summary>The Arc with which the Raycast collided.</summary>
        public readonly Arc arc;
        /// <summary>The Block with which the Raycast collided.</summary>
        public readonly Block block;
        /// <summary>The PlanetariaCollider instance - a group of N sphere collisions.</summary>
        public readonly PlanetariaCollider collider;
        /// <summary>The distance along a circular path to the collision point.</summary>
        public readonly float distance;
        /// <summary>Is the raycast colliding with the arc from the outside (beyond the floor)?</summary>
        public readonly bool positive_face_collision;
        /// <summary>The normal of the arc's collision surface at the given point.</summary>
        public readonly Vector3 normal;
        /// <summary>The point of collision along the arc's surface.</summary>
        public readonly Vector3 point;
        /// <summary>The PlanetariaRigidbody instance if it exists - similar to UnityEngine.Rigidbody</summary>
        public readonly optional<PlanetariaRigidbody> rigidbody;
        /// <summary>The PlanetariaTransform instance - similar to UnityEngine.Transform</summary>
        public readonly PlanetariaTransform transform;
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