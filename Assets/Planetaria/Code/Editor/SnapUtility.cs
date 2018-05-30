using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    public static class SnapUtility
    {
        public static void draw_grid()
        {
            UnityEditor.Handles.color = Color.white;

            for (float row = 1; row <= Global.self.rows; ++row) // equator lines
            {
                float angle = Mathf.PI * row / (Global.self.rows + 1);
                float radius = Mathf.Sin(angle);
                Vector3 center = Vector3.down * Mathf.Cos(angle);
                UnityEditor.Handles.DrawWireDisc(center, Vector3.up, radius);
            }

            for (float column = 0; column < Global.self.columns; ++column) // time zone lines
            {
                float angle = Mathf.PI * column / Global.self.columns;
                Vector3 normal = PlanetariaMath.slerp(Vector3.forward, Vector3.right, angle);
                UnityEditor.Handles.DrawWireDisc(Vector3.zero, normal, 1);
            }
        }

        public static Vector3 snap(Vector3 position, bool v_pressed)
        {
            if (Event.current.control && Event.current.shift) // Edge snap on Control + Shift + click
            {
                optional<Vector3> edge_snap = closest_heuristic(ArcUtility.snap_to_edge, position);
                if (edge_snap.exists)
                {
                    return edge_snap.data;
                }
            }
            else if (Global.self.v_pressed) // Vertex snap on V + click
            {
                optional<Vector3> vertex_snap = closest_heuristic(ArcUtility.snap_to_vertex, position);
                if (vertex_snap.exists)
                {
                    return vertex_snap.data;
                }
            }
            else if (Event.current.shift) // Grid snap on Shift + click
            {
                return grid_snap(position);
            }
            return position; // return the input position (if all else fails).
        }

        private static Vector3 grid_snap(Vector3 position) // FIXME: optimize
        {
            if (!Event.current.shift) // only snap when shift key is held
            {
                return position;
            }

            NormalizedCartesianCoordinates cartesian_position = new NormalizedCartesianCoordinates(position);

            NormalizedSphericalCoordinates clamped_coordinates = new NormalizedSphericalCoordinates(0, 0);
            NormalizedSphericalCoordinates desired_coordinates = cartesian_position;

            for (float row = 0; row <= Global.self.rows + 1; ++row) //going over with off by one errors won't ruin the program...
            {
                float angle = Mathf.PI * row / (Global.self.rows + 1);
                float x = Mathf.Sin(angle);
                float y = Mathf.Cos(angle);
                NormalizedSphericalCoordinates test_coordinates = new NormalizedCartesianCoordinates(new Vector3(x, y, 0));

                float error = Mathf.Abs(Mathf.DeltaAngle(desired_coordinates.data.x * Mathf.Rad2Deg, test_coordinates.data.x * Mathf.Rad2Deg));
                float old_error = Mathf.Abs(Mathf.DeltaAngle(desired_coordinates.data.x * Mathf.Rad2Deg, clamped_coordinates.data.x * Mathf.Rad2Deg));

                if (error < old_error)
                {
                    clamped_coordinates = new NormalizedSphericalCoordinates(test_coordinates.data.x, clamped_coordinates.data.y);
                }
            }

            for (float column = 0; column < Global.self.columns * 2; ++column) //... but going under is bad
            {
                float angle = column / Global.self.columns * Mathf.PI;
                float x = Mathf.Sin(angle);
                float z = Mathf.Cos(angle);
                NormalizedSphericalCoordinates test_coordinates = new NormalizedCartesianCoordinates(new Vector3(x, 0, z));

                float error = Mathf.Abs(Mathf.DeltaAngle(desired_coordinates.data.y * Mathf.Rad2Deg, test_coordinates.data.y * Mathf.Rad2Deg));
                float old_error = Mathf.Abs(Mathf.DeltaAngle(desired_coordinates.data.y * Mathf.Rad2Deg, clamped_coordinates.data.y * Mathf.Rad2Deg));

                if (error < old_error)
                {
                    clamped_coordinates = new NormalizedSphericalCoordinates(clamped_coordinates.data.x, test_coordinates.data.y);
                }
            }

            cartesian_position = clamped_coordinates;

            return cartesian_position.data;
        }

        private delegate Vector3 HeuristicFunction(Arc arc, Vector3 target);

        private static optional<Vector3> closest_heuristic(HeuristicFunction distance_heuristic, Vector3 target)
        {
            List<Arc> arc_list = ArcUtility.get_all_arcs();
            if (arc_list.Count > 0)
            {
                Vector3 closest_point = arc_list[0].begin();
                float closest_distance_squared = Mathf.Infinity;
                foreach (Arc arc in arc_list)
                {
                    Vector3 other_point = distance_heuristic(arc, target);
                    float distance_squared = (other_point - target).sqrMagnitude;
                    if (distance_squared < closest_distance_squared)
                    {
                        closest_distance_squared = distance_squared;
                        closest_point = other_point;
                    }
                }
                return closest_point;
            }
            return new optional<Vector3>();
        }
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
