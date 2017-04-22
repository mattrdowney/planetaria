using UnityEngine;

public class BlockInteractor
{
    public BlockInteractor(Arc arc, Vector3 last_position, Vector3 current_position, float half_height_)
    {
        optional<Block> block = PlanetariaCache.block_cache.Get(arc);

        if (!block.exists)
        {
            Debug.Log("Critical Err0r.");
            target = null;
            arc_index = 0;
            interpolator_angle = 0;
            half_height = 0;
            return;
        }

        target = block.data;
        half_height = half_height_;

        // Special initialization notes: remember that the arc used for initialization might not be the ultimate arc_index! (it could be a closer adjacent node!)
        arc_index = target.arc_index(arc.data);
        interpolator_angle = target.position_to_angle(closest_intersection_point);
    }

    Block target;
    int arc_index;
    float interpolator_angle;
    float half_height;
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