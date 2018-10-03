using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Planetaria
{
    [Serializable]
    public class PlanetariaShape : ScriptableObject // TODO: clean-up this file~
    {
        public enum AppendMode { PermanentAppend, EphemeralAppend };

        [MenuItem("Assets/Create/PlanetariaShape")]
        public static PlanetariaShape Create()
        {
            PlanetariaShape asset = ScriptableObject.CreateInstance<PlanetariaShape>();

            asset.closed = false;
            asset.has_corners = true;
            asset.serialized_arc_list = new SerializedArc[0];
            asset.arc_list = new Arc[0];
            asset.block_list = new PlanetariaArcCollider[0];
            asset.field_list = new PlanetariaSphereCollider[0];

            AssetDatabase.CreateAsset(asset, "Assets/" + EditorSceneManager.GetActiveScene().name + "_PlanetariaShape.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
        
        /// <summary>
        /// Constructor - Creates a shape based on a list of curves (generates cached list of Arc)
        /// </summary>
        /// <param name="curves">The list of curves that uniquely defines a shape.</param>
        /// <param name="closed_shape">Is the shape closed? (i.e. does the shape draw the final arc from the last point to the first point?)</param>
        /// <param name="generate_corners">Does the shape have corners between line segments?</param>
        public static PlanetariaShape Create(List<SerializedArc> serialized_arcs, bool closed_shape, bool generate_corners) // CONSIDER: TODO: add convex option
        {
            PlanetariaShape asset = Create();

            asset.closed = closed_shape;
            asset.has_corners = generate_corners;
            asset.serialized_arc_list = serialized_arcs.ToArray();
            asset.generate_arcs();

            return asset;
        }

        public List<Arc> block_collision(PlanetariaShape other, Quaternion shift_from_self_to_other) // TODO: AABB-equivalent would be nice here
        {
            Debug.Log("Inside Block Collision Detection");
            List<Arc> union = new List<Arc>();
            for (int other_index = 0; other_index < other.block_list.Length; ++other_index)
            {
                Debug.Log("Other ArcCollider detected");
                PlanetariaArcCollider other_collider = other.block_list[other_index];
                foreach (PlanetariaArcCollider this_collider in this.block_list)
                {
                    Debug.Log("Comparing with one of our ArcColliders");
                    if (other_collider.collides_with(this_collider, shift_from_self_to_other))
                    {
                        Debug.Log("!!! Something should have happened !!!");
                        union.Add(other.arc_list[other_index]);
                        break; // try to see if the next arc collides (because we know this one does)
                    }
                }
            }
            return union;
        }

        public bool field_collision(PlanetariaShape other, Quaternion shift_from_self_to_other) // TODO: AABB-equivalent would be nice here
        {
            foreach (PlanetariaArcCollider arc in this.block_list)
            {
                bool colliding = true;
                foreach (PlanetariaSphereCollider sphere in other.field_list)
                {
                    if (!arc.collides_with(sphere, shift_from_self_to_other))
                    {
                        colliding = false;
                        break; // try to see if the next arc collides (because we know this one does)
                    }
                }
                if (colliding)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Get the nth arc in the arc_list.</summary>
        public Arc this[int index]
        {
            get
            {
                return arc_list[index];
            }
        }

        public int Length
        {
            get
            {
                return arc_list.Length;
            }
        }

        /// <summary>The list of arcs that define a shape.</summary>
        public IEnumerable<Arc> arcs
        {
            get
            {
                return arc_list;
            }
        }

        /// <summary>
        /// Returns the index of any existing arc within the shape that matches the external reference.
        /// </summary>
        /// <param name="arc">The reference to the external arc that will be compared to the shape's arc list.</param>
        /// <returns>The index of the match if the arc exists in the container; a nonexistent index otherwise.</returns>
        public optional<ArcVisitor> arc_visitor(Arc arc)
        {
            int arc_list_index = Array.IndexOf(arc_list, arc);
            if (arc_list_index == -1)
            {
                return new optional<ArcVisitor>();
            }
            return ArcVisitor.arc_visitor(this, arc_list_index);
        }

        /// <summary>
        /// Inspector/Constructor - Appends a new arc to a current shape.
        /// </summary>
        /// <param name="arc">The arc you are appending to the end of the shape.</param>
        /// <returns>A shape with a new arc appended.</returns>
        public void append(SerializedArc arc, AppendMode permanence = AppendMode.PermanentAppend)
        {
            if (!overwrite_last_entry) // avoid overwriting data by extending the array 
            {
                Array.Resize(ref serialized_arc_list, serialized_arc_list.Length + 1);
            }
            
            serialized_arc_list[serialized_arc_list.Length - 1] = arc; // set the new element
            generate_arcs();

            bool overwrite_next = (permanence == AppendMode.EphemeralAppend);
            overwrite_last_entry = overwrite_next;
        }

        /// <summary>
        /// Inspector/Constructor - Creates a copy of the shape then sets closed=true
        /// </summary>
        /// <returns>A closed shape mirroring all properties of original but with closed=true.</returns>
        public PlanetariaShape close()
        {
            PlanetariaShape asset = Create();
            asset.closed = true;
            asset.has_corners = this.has_corners;
            asset.arc_list = new Arc[0];
            asset.serialized_arc_list = this.serialized_arc_list;
            // FIXME: add last edge! (if not already closed and Dot of last/first point is != 1)
            asset.generate_arcs();
            // TODO: if field, make this a convex_hull() // TODO: add convex property
            return asset;
        }

        public PlanetariaSphereCollider bounding_sphere
        {
            get
            {
                Vector3 center = center_of_mass();
                Vector3 furthest_point = center;
                float furthest_distance_squared = 0;
                foreach (Arc arc in arc_list)
                {
                    Vector3 current_point = ArcUtility.furthest_point(arc, center);
                    float current_distance_squared = (current_point - center).sqrMagnitude;
                    if (current_distance_squared > furthest_distance_squared)
                    {
                        furthest_point = current_point;
                        furthest_distance_squared = current_distance_squared;
                    }
                }
                return PlanetariaArcCollider.boundary(center, furthest_point);
            }
        }

        public bool is_self_intersecting()
        {
            List<Arc> edges = generate_edges();
            for (int left = 0; left < edges.Count; ++left)
            {
                for (int right = left + 1; right < edges.Count; ++right)
                {
                    optional<Vector3> intersection = PlanetariaIntersection.arc_arc_intersection(edges[left], edges[right], 0);
                    if (intersection.exists)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Vector3 center_of_mass() // TODO: verify this is a proper volume integration (for convex hulls)
        {
            Vector3 result = Vector3.zero; // CONSIDER: should the center of mass be the 2D center (current implementation) or 3D center?
            foreach (Arc arc in arcs)
            {
                float weight = arc.length(); // multiply be the weight of the arc (length is an integration of sorts)
                Vector3 arc_center = arc.position(0); // get the center of mass of each arc // zero intentional
                result += arc_center * weight;
            }
            result.Normalize();
            return result; // FIXME: Vector3.zero can be returned
        }

        public Vector3[] convex_hull()
        {
            Debug.LogError("Implement this"); // FIXME:
            return new Vector3[0];
        }

        public bool is_convex_hull()
        {
            if (arc_list.Length > 1)
            {
                PlanetariaShape closed_shape = this.close();
                Arc last_arc = closed_shape.generate_edges().Last();
                foreach (Arc arc in closed_shape.generate_edges())
                {
                    if (ArcFactory.corner_type(last_arc, arc) == GeometryType.ConcaveCorner)
                    {
                        return false;
                    }
                    last_arc = arc;
                }
            }
            return true;
        }

        public static bool operator ==(PlanetariaShape left, PlanetariaShape right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlanetariaShape left, PlanetariaShape right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(System.Object other)
        {
            bool same_type = other is PlanetariaShape;
            if (!same_type)
            {
                return false;
            }
            PlanetariaShape other_shape = (PlanetariaShape)other;
            bool same_data = this.serialized_arc_list == other_shape.serialized_arc_list; // compare by reference is intentional
            return same_data;
        }

        public override int GetHashCode()
        {
            return this.serialized_arc_list.GetHashCode();
        }

        /// <summary>
        /// Inspector (pseudo-mutator) - initialize arc_list (when the user changes the list of curves)
        /// </summary>
        public void OnBeforeSerialize()
        {
            generate_arcs();
        }

        /// <summary>
        /// Inspector (pseudo-mutator) - initialize arc_list (which is NonSerialized (i.e. not saved))
        /// </summary>
        public void OnAfterDeserialize() // TODO: check if this can be private
        {
            generate_arcs();
        }

        /// <summary>
        /// Inspector (pseudo-mutator) - Caches all arcs based on curve_list at load-time
        /// </summary>
        private void generate_arcs()
        {
            List<Arc> result = generate_edges();
            result = add_corners_between_edges(result);
            arc_list = result.ToArray();
            generate_colliders();
            generate_colliders();
        }

        /// <summary>
        /// Inspector - Generates a shape (without corners) from a list of curves. Closing edge not generated if "closed" is not set.
        /// </summary>
        /// <returns>A list of edges that define a shape.</returns>
        private List<Arc> generate_edges()
        {
            List<Arc> result = new List<Arc>();
            int edges = closed ? serialized_arc_list.Length : serialized_arc_list.Length - 1;
            for (int edge = 0; edge < edges; ++edge)
            {
                Arc arc = serialized_arc_list[edge];
                result.Add(arc);
            }
            return result;
        }

        /// <summary>
        /// Inspector (pseudo-mutator) - Caches each PlanetariaArcCollider associated with each Arc.
        /// </summary>
        private void generate_colliders()
        {
            block_list = new PlanetariaArcCollider[arc_list.Length];
            for (int collider = 0; collider < arc_list.Length; ++collider)
            {
                block_list[collider] = PlanetariaArcCollider.block(arc_list[collider]);
            }
            field_list = new PlanetariaSphereCollider[has_corners ? arc_list.Length/2 : arc_list.Length];
            for (int collider = 0; collider < arc_list.Length; collider += (has_corners ? 2 : 1))
            {
                field_list[has_corners ? collider/2 : collider] = PlanetariaArcCollider.field(arc_list[collider]);
            }
        }

        /// <summary>
        /// Inspector - Generates a shape (with corners if has_corners is set) from a list of edges.
        /// </summary>
        /// <param name="edges">The arc edges that define the shape.</param>
        /// <returns>Returns a list of edges interspliced with corners.</returns>
        private List<Arc> add_corners_between_edges(List<Arc> edges)
        {
            List<Arc> result = new List<Arc>();
            for (int edge = 0; edge < edges.Count; ++edge)
            {
                Arc left_edge = edges[(edge + 0) % edges.Count];
                Arc right_edge = edges[(edge + 1) % edges.Count];
                result.Add(left_edge);
                bool ignore = !closed && edge == edges.Count-1; //ignore the last corner for unclosed shapes
                if (has_corners && !ignore)
                {
                    result.Add(ArcFactory.corner(left_edge, right_edge));
                }
            }
            return result;
        }

        /// <summary>Does the shape have corners between line segments?</summary>
        [SerializeField] private bool has_corners;
        /// <summary>Is the shape closed? (i.e. does the shape draw the final arc from the last point to the first point?)</summary>
        [SerializeField] private bool closed;
        /// <summary>List of arcs that define a shape (excluding corner connections).</summary>
        [SerializeField] private SerializedArc[] serialized_arc_list;
        /// <summary>List of arcs on a unit sphere that define a shape (including corner connections).</summary>
        [NonSerialized] private Arc[] arc_list; // TODO: would readonly prevent overwriting data entries? - I think so since this isn't a List<>
        /// <summary>List of arc colliders that will be used for intersection.</summary>
        [NonSerialized] public PlanetariaArcCollider[] block_list;
        /// <summary>List of arc colliders that will be used for intersection.</summary>
        [NonSerialized] public PlanetariaSphereCollider[] field_list;
        /// <summary>An internal flag for whether or not the last entry was ephemeral (non-permanent) e.g. for Arc creation visualization in the editor.</summary>
        [NonSerialized] private bool overwrite_last_entry = false;
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