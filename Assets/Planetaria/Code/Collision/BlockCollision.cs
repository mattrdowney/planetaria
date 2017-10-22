using UnityEngine;

public class BlockCollision
{
    public static optional<BlockCollision> block_collision(Arc arc, Vector3 last_position, Vector3 current_position, float radius)
    {
        BlockCollision result = new BlockCollision();

        NormalizedCartesianCoordinates begin = new NormalizedCartesianCoordinates(last_position);
        NormalizedCartesianCoordinates end = new NormalizedCartesianCoordinates(current_position);

        optional<Block> block = PlanetariaCache.block_cache.get(arc);
        if (!block.exists)
        {
            return new optional<BlockCollision>();
        }

        result.target = block.data;

        optional<int> collision_arc_index = result.target.arc_index(arc);
        if (!collision_arc_index.exists)
        {
            return new optional<BlockCollision>();
        }

        int closest_index = 0;
        float closest_similarity = -1;

        for (int adjacent_arc_index = collision_arc_index.data - 2; adjacent_arc_index <= collision_arc_index.data + 2; ++adjacent_arc_index)
        {
            int current_index = adjacent_arc_index;
            optional<Arc> current_arc = result.target.at(ref current_index);
            optional<Vector3> current_intersection_point = PlanetariaIntersection.arc_path_intersection(current_arc.data, begin, end); //TODO: check .data
            if (current_intersection_point.exists)
            {
                float similarity = Vector3.Dot(current_intersection_point.data, last_position);
                if (similarity > closest_similarity)
                {
                    closest_index = current_index;
                    closest_similarity = similarity;
                }
            }
        }

        optional<Arc> closest_arc = result.target.at(ref closest_index);
        
        optional<Vector3> closest_intersection_point = PlanetariaIntersection.arc_path_intersection(closest_arc.data, begin, end); //TODO: check
        if (!closest_intersection_point.exists)
        {
            return new optional<BlockCollision>();
        }

        result.arc_index = closest_index;
        result.interpolator_angle = closest_arc.data.position_to_angle(closest_intersection_point.data);

        return result;
    }

    private Block target;
    private int arc_index; // FIXME: iterator
    private float interpolator_angle;
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