using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct Shape : ISerializationCallbackReceiver // TODO: clean-up this file~
    {
        /// <summary>
        /// Constructor - Creates a shape based on a list of curves (generates cached list of Arc)
        /// </summary>
        /// <param name="curves">The list of curves that uniquely defines a shape.</param>
        /// <param name="closed_shape">Is the shape closed? (i.e. does the shape draw the final arc from the last point to the first point?)</param>
        /// <param name="generate_corners">Does the shape have corners between line segments?</param>
        public Shape(List<GeospatialCurve> curves, bool closed_shape, bool generate_corners) // CONSIDER: TODO: add convex option
        {
            closed = closed_shape;
            has_corners = generate_corners;
            arc_list = new Arc[0];
            curve_list = curves.ToArray();
            generate_arcs();
        }

        /// <summary>
        /// Constructor - Creates an empty shape.
        /// </summary>
        /// <param name="closed_shape">Is the shape closed? (i.e. does the shape draw the final arc from the last point to the first point?)</param>
        /// <param name="generate_corners">Does the shape have corners between line segments?</param>
        public Shape(bool closed_shape, bool generate_corners)
        {
            closed = closed_shape;
            has_corners = generate_corners;
            arc_list = new Arc[0];
            curve_list = new GeospatialCurve[0];
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

        /// <summary>The list of curves that define a shape.</summary>
        public IEnumerable<GeospatialCurve> curves
        {
            get
            {
                return curve_list;
            }
        }

        /// <summary>
        /// Inspector/Constructor - Appends a new curve to a current shape.
        /// </summary>
        /// <param name="curve">The curve you are appending to the end of the shape.</param>
        /// <returns>A shape with a new curve appended.</returns>
        public Shape append(GeospatialCurve curve)
        {
            Shape result = new Shape();
            result.closed = this.closed;
            result.has_corners = this.has_corners;
            result.arc_list = new Arc[0];

            result.curve_list = new GeospatialCurve[this.curve_list.Length + 1]; // appending array so length += 1
            Array.Copy(this.curve_list, result.curve_list, this.curve_list.Length); // copy the original array
            result.curve_list[result.curve_list.Length - 1] = curve; // set the new element
            result.generate_arcs();
            return result;
        }

        /// <summary>
        /// Inspector/Constructor - Creates a copy of the shape then sets closed=true
        /// </summary>
        /// <returns>A closed shape mirroring all properties of original but with closed=true.</returns>
        public Shape close()
        {
            Shape shape = new Shape();
            shape.closed = true;
            shape.has_corners = this.has_corners;
            shape.arc_list = new Arc[0];
            shape.curve_list = this.curve_list;
            shape.generate_arcs();
            return shape;
        }

        public List<GeospatialCurve> to_curves()
        {
            return new List<GeospatialCurve>(curve_list);
        }

        public bool self_intersecting()
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

        public bool convex_hull()
        {
            if (arc_list.Length > 1)
            {
                Shape closed_shape = this.close();
                Arc last_arc = closed_shape.generate_edges().Last();
                foreach (Arc arc in closed_shape.generate_edges())
                {
                    if (Arc.corner_type(last_arc, arc) == GeometryType.ConcaveCorner)
                    {
                        return false;
                    }
                    last_arc = arc;
                }
            }
            return true;
        }

        public static bool operator ==(Shape left, Shape right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Shape left, Shape right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(System.Object other)
        {
            bool same_type = other is Shape;
            if (!same_type)
            {
                return false;
            }
            Shape other_shape = (Shape)other;
            bool same_data = this.curve_list == other_shape.curve_list; // compare by reference is intentional
            return same_data;
        }

        public override int GetHashCode()
        {
            return this.curve_list.GetHashCode();
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
        }

        /// <summary>
        /// Inspector - Generates a shape (without corners) from a list of curves. Closing edge not generated if "closed" is not set.
        /// </summary>
        /// <returns>A list of edges that define a shape.</returns>
        private List<Arc> generate_edges()
        {
            List<Arc> result = new List<Arc>();
            int edges = closed ? curve_list.Length : curve_list.Length - 1;
            for (int edge = 0; edge < edges; ++edge)
            {
                GeospatialCurve left_curve = curve_list[(edge + 0) % curve_list.Length];
                GeospatialCurve right_curve = curve_list[(edge + 1) % curve_list.Length];
                Arc arc = Arc.curve(left_curve.point, left_curve.slope, right_curve.point);
                result.Add(arc);
            }
            return result;
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
                    result.Add(Arc.corner(left_edge, right_edge));
                }
            }
            return result;
        }

        /// <summary>Does the shape have corners between line segments?</summary>
        [SerializeField] private bool has_corners;
        /// <summary>Is the shape closed? (i.e. does the shape draw the final arc from the last point to the first point?)</summary>
        [SerializeField] private bool closed;
        /// <summary>List of point-slope pairs in spherical space that define a shape.</summary>
        [SerializeField] private GeospatialCurve[] curve_list;
        /// <summary>List of arcs on a unit sphere that define a shape.</summary>
        [NonSerialized] private Arc[] arc_list;
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