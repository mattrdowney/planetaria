using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="https://www.nature.com/articles/srep26756/figures/4"/> // binning light function for light from sun (and light polution) based on angle of incidence from sun.
    /// <seealso cref=""/> // Trying to find the research article that goes over 1) direct lighting, 2) indirect lighting (due to atmospheric refraction), and 3) ? reflective lighting due to surface properties
    public class SunlightPlanetarium : WorldPlanetarium
    {
        public SunlightPlanetarium(Vector3 point)
        {
            this.point = point;

            pixel_centroids = new NormalizedCartesianCoordinates[0];
        }
        
        public override void set_pixels(Color32[] positions) { }

        public override Color32[] get_pixels(NormalizedCartesianCoordinates[] positions)
        {
            Color32[] colors = new Color32[positions.Length];
            for (int index = 0; index < positions.Length; ++index)
            {
                Vector3 position = positions[index].data;
                float angle_in_degrees = Vector3.Angle(position, point);
                int color_index = Mathf.Clamp((sunlight_lookup_table.Length - 1) - Mathf.FloorToInt(angle_in_degrees/2), 0, (sunlight_lookup_table.Length - 1)); // avoid negatives
                colors[index] = sunlight_lookup_table[color_index];
            }
            return colors;
        }

#if UNITY_EDITOR
        public override void save(string file_name) { }
#endif

        private Vector3 point;

        // cache
        private float dot_product_threshold;
        // hardcode array of sunlight intensities from https://www.nature.com/articles/srep26756/figures/4 Figure #4 (texture read would work, but there are disadvantages)
        private readonly Color32[] sunlight_lookup_table = new Color32[90]
        {
            // amusingly, only alpha matters (forgot that DX)
            new Color32(124, 100, 82, 102), // -90 degrees below horizon (ratios preserved compared to -28 degrees)
            new Color32(126, 101, 83, 103),
            new Color32(127, 102, 84, 104),
            new Color32(128, 103, 85, 105),
            new Color32(129, 104, 83, 106),
            new Color32(130, 105, 84, 107), // -80 degrees ...
            new Color32(131, 106, 84, 108),
            new Color32(133, 107, 85, 109),
            new Color32(134, 108, 86, 110),
            new Color32(135, 109, 87, 111),
            new Color32(136, 110, 88, 112), // -70 degrees ...
            new Color32(137, 111, 89, 113),
            new Color32(138, 112, 90, 113),
            new Color32(140, 113, 90, 114),
            new Color32(141, 114, 91, 115),
            new Color32(142, 115, 92, 116), // -60 degrees ...
            new Color32(143, 115, 93, 117),
            new Color32(144, 116, 94, 118),
            new Color32(145, 117, 95, 119),
            new Color32(147, 118, 95, 120),
            new Color32(148, 119, 96, 121), // -50 degrees ...
            new Color32(149, 120, 97, 122),
            new Color32(150, 121, 98, 123),
            new Color32(151, 122, 99, 124),
            new Color32(152, 123, 100, 125),
            new Color32(154, 124, 100, 126), // -40 degrees ...
            new Color32(155, 125, 101, 127),
            new Color32(156, 126, 102, 128),
            new Color32(157, 127, 103, 129),
            new Color32(158, 128, 104, 130),
            new Color32(159, 129, 105, 131), // -30 degrees ...
            // PREVIOUS VALUES EXTRAPOLATED, LATER VALUES EMPIRICALLY DETERMINED
            new Color32(160, 130, 106, 132), //-28 degrees below the horizon
            new Color32(161, 135, 102, 133),
            new Color32(162, 138, 100, 133),
            new Color32(159, 137, 98, /*131*/ 134),
            new Color32(154, 131, 113, /*133*/ 136),
            new Color32(146, 130, 137, /*138*/ 138), // -18 degrees ...
            new Color32(111, 128, 174, /*138*/ 140),
            new Color32(86, 130, 193, /*136*/ 143),
            new Color32(92, 142, 203, 146), // -12 degrees ...
            new Color32(110, 155, 210, 158),
            new Color32(133, 170, 219, 174),
            new Color32(161, 189, 228, 193), // -6 degrees ...
            new Color32(179, 202, 233, 205),
            new Color32(192, 212, 236, 213),
            new Color32(201, 218, 238, 219), // 0 degrees - at horizon
            new Color32(206, 223, 241, 223),
            new Color32(215, 227, 241, 228),
            new Color32(222, 227, 237, 229),
            new Color32(234, 230, 227, 230),
            new Color32(238, 231, 225, 231),
            new Color32(239, 235, 226, 233),
            new Color32(249, 230, 224, 234),
            new Color32(251, 235, 221, 236),
            new Color32(246, 238, 225, 236),
            new Color32(243, 237, 232, 237),
            new Color32(244, 237, 227, /*236*/ 237),
            new Color32(247, 241, 229, /*239*/ 237),
            new Color32(247, 242, 222, 237), // +26 degrees above the horizon
            // PREVIOUS VALUES EMPIRICALLY DETERMINED, LATER VALUES EXTRAPOLATED
            new Color32(248, 244, 223, 238), // +28 degrees ...
            new Color32(249, 245, 224, 239),
            new Color32(250, 247, 225, 241),
            new Color32(251, 248, 226, 242),
            new Color32(252, 250, 227, 243),
            new Color32(253, 251, 228, 244),
            new Color32(254, 253, 229, 245),
            new Color32(255, 254, 230, 246), // +42 degrees above horizon
            new Color32(255, 255, 231, 247), // +44 degrees above horizon
            new Color32(255, 255, 232, 247),
            new Color32(255, 255, 233, 248),
            new Color32(255, 255, 235, 248),
            new Color32(255, 255, 236, 249),
            new Color32(255, 255, 237, 249),
            new Color32(255, 255, 238, 249),
            new Color32(255, 255, 239, 250),
            new Color32(255, 255, 240, 250), // +60 degrees ...
            new Color32(255, 255, 241, 250),
            new Color32(255, 255, 242, 251),
            new Color32(255, 255, 243, 251),
            new Color32(255, 255, 244, 251),
            new Color32(255, 255, 245, 252),
            new Color32(255, 255, 247, 252),
            new Color32(255, 255, 248, 253),
            new Color32(255, 255, 249, 253),
            new Color32(255, 255, 250, 253),
            new Color32(255, 255, 251, 254), // +80 degrees above horizon
            new Color32(255, 255, 252, 254),
            new Color32(255, 255, 253, 254),
            new Color32(255, 255, 254, 255),
            new Color32(255, 255, 255, 255), // +88 (< +90) degrees above horizon
        };
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