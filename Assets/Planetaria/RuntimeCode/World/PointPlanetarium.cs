using UnityEngine;

namespace Planetaria
{
    /// <summary>
	///
    /// </summary>
	public class PointPlanetarium : WorldPlanetarium // TODO: rename // FIXME: use SpotLight (since it's better in almost all ways)
	{
        public PointPlanetarium(Vector3 point, float radius)
        {
            this.point = point;
            this.radius = radius;

            // cache for early return
            Vector3 furthest_point = Vector3.RotateTowards(point, -point, radius, 0.0f);
            dot_product_threshold = Vector3.Dot(point, furthest_point);

            pixel_centroids = new NormalizedCartesianCoordinates[0];
        }
        
        public override void set_pixels(Color32[] positions) { }

        public override Color32[] get_pixels(NormalizedCartesianCoordinates[] positions)
        {
            Color32[] colors = new Color32[positions.Length];
            for (int index = 0; index < positions.Length; ++index)
            {
                Vector3 position = positions[index].data;
                if (Vector3.Dot(point, position) < dot_product_threshold) // circle check (is test point within circle?)
                {
                    colors[index] = new Color32(0,0,0,0); // Color.clear
                }
                else // common early return handled, do expensive work
                {
                    float angle = Vector3.Angle(position, point) * Mathf.Deg2Rad;
                    float falloff = Mathf.Clamp01(1f - angle/radius); // avoid negatives
                    colors[index] = new Color(1, 1, 1, falloff);
                }
            }
            return colors;
        }

#if UNITY_EDITOR
        public override void save(string file_name) { }
#endif

        private Vector3 point;
        private float radius;

        // cache
        private float dot_product_threshold;
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