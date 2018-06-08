using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    [ExecuteInEditMode]
    public static class BlockRendererIterator
    {
        private static void add_intersections(Arc arc, Vector3[] positions)
        {
            for (int intersection_index = 0; intersection_index < positions.Length; ++intersection_index)
            {
                add_intersection(arc, new NormalizedCartesianCoordinates(positions[intersection_index]));
            }
        }

        private static void add_intersection(Arc arc, NormalizedCartesianCoordinates position)
        {
            if (position.data.y <= 0) // FIXME:
            {
                if (!discontinuities.ContainsKey(arc))
                {
                    discontinuities.Add(arc, new List<Discontinuity>());
                }

                discontinuities[arc].Add(new Discontinuity(arc, position));
            }
        }

        /// <summary>
        /// Mutator - find all intersections along x=0 or z=0 arcs in southern hemisphere.
        /// </summary>
        /// <param name="block">The block (set of arcs) to be inspected.</param>
        private static void find_discontinuities(Block block)
        {
            foreach (optional<Arc> arc in block.shape.arcs)
            {
                for (int dimension = 0; dimension < 2; ++dimension) // Intersect already gets quadrants 3-4 by proxy
                {
                    if (arc.exists)
                    {
                        float angle = (Mathf.PI/2)*dimension;
                        Vector3 begin = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                        Vector3 end = Vector3.down;
                        Vector3[] intersections = PlanetariaIntersection.arc_path_intersections(arc.data, begin, end, 0);
                        add_intersections(arc.data, intersections);
                    }
                }
            }
        }

        public static void prepare(Block block)
        {
            discontinuities = new Dictionary<Arc, List<Discontinuity>>();
            find_discontinuities(block);
            sort_discontinuities();
            block_variable = block;
        }

        /// <summary>
        /// Mutator - Sort the Discontinuity lists in non-decreasing order (with respect to angle)
        /// </summary>
        private static void sort_discontinuities()
        {
            foreach (KeyValuePair<Arc, List<Discontinuity>> discontinuity in discontinuities)
            {
                discontinuity.Value.Sort((left_hand_side, right_hand_side) => left_hand_side.angle.CompareTo(right_hand_side.angle));
            }
        }

        public static IEnumerable<ArcIterator> arc_iterator()
        {
            foreach (optional<Arc> arc in block_variable.shape.arcs)
            { 
                if (!arc.exists)
                {
                    continue;
                }
            
                float begin_angle = 0;

                if (discontinuities.ContainsKey(arc.data))
                {
                    for (int list_index = 0; list_index < discontinuities[arc.data].Count; ++list_index)
                    {
                        float end_angle = discontinuities[arc.data][list_index].angle;
                        yield return new ArcIterator(arc.data, begin_angle, end_angle);
                        begin_angle = end_angle;
                    }
                }
            
                yield return new ArcIterator(arc.data, begin_angle, arc.data.angle());
            }
        }

        private static Block block_variable;
        private static Dictionary<Arc, List<Discontinuity>> discontinuities;
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