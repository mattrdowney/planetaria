using UnityEngine;

namespace Planetaria
{
    /// <summary>
	///
    /// </summary>
	public class ArcPlanetarium : WorldPlanetarium // TODO: rename
	{
        public ArcPlanetarium(Arc arc, Color color, float radius)
        {
            this.arc = arc;
            this.color = color;
            this.radius = radius;
            // TODO: VERIFY: great caution should be taken with these optimizations, since mistakes are very likely
            // For early returns (avoiding unnecessary calculations)
            center = arc.position(0);
            Vector3 vertex = arc.position(arc.angle()/2);
            Vector3 furthest_point = Vector3.RotateTowards(vertex, -center, radius, 0.0f);
            dot_product_threshold = Vector3.Dot(center, furthest_point);
            SphericalCap lower = arc.floor(-radius);
            SphericalCap upper = arc.floor(+radius);
            center_axis = lower.normal;
            center_offset = lower.offset + upper.offset;
            center_range = Mathf.Abs(upper.offset - lower.offset)/2;
            similarity_threshold = Vector3.Dot(Vector3.forward, Vector3.RotateTowards(Vector3.forward, Vector3.back, radius, 0f));
            pixel_centroids = new NormalizedCartesianCoordinates[0];
        }
        
        public override void set_pixels(Color[] positions) { }

        public override Color[] get_pixels(NormalizedCartesianCoordinates[] positions)
        {
            Color[] colors = new Color[positions.Length];
            for (int index = 0; index < positions.Length; ++index)
            {
                Vector3 position = positions[index].data;
                if (Vector3.Dot(center, position) < dot_product_threshold) // circle check (is test point within circle circumscribing extruded arc?)
                {
                    colors[index] = Color.clear;
                }
                else if (Mathf.Abs(Vector3.Dot(center_axis, position) - center_offset) > center_range) // elevation check (is test point in narrow band of the arc?)
                {
                    colors[index] = Color.clear;
                }
                else // common early returns handled, do expensive work
                {
                    Vector3 closest_point = ArcUtility.snap_to_edge(arc, position);
                    if (Vector3.Dot(position, closest_point) < similarity_threshold) // distance check (is test point closer than max radial distance?)
                    {
                        colors[index] = Color.clear;
                    }
                    else
                    {
                        float angle = Vector3.Angle(position, closest_point) * Mathf.Deg2Rad;
                        float falloff = Mathf.Clamp01(1f - angle/radius);
                        colors[index] = color * falloff;
                    }
                }
            }
            return colors;
        }

#if UNITY_EDITOR
        public override void save(string file_name) { }
#endif

        private Arc arc;
        private Color color;
        private float intensity;
        private float radius;

        // cached early return data
        private Vector3 center;
        private float dot_product_threshold;

        private Vector3 center_axis;
        private float center_offset;
        private float center_range;
        
        private float similarity_threshold;
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