﻿using UnityEngine;

public class BlockInteractor
{
    public BlockInteractor(Arc arc, Vector3 last_position, Vector3 current_position, float half_height_)
    {
        NormalizedCartesianCoordinates begin = new NormalizedCartesianCoordinates(last_position);
        NormalizedCartesianCoordinates end = new NormalizedCartesianCoordinates(current_position);

        optional<Block> block = PlanetariaCache.block_cache.Get(arc);
        if (!block.exists)
        {
            nullify(this);
            return;
        }

        target = block.data;
        half_height = half_height_;

        optional<int> collision_arc_index = target.arc_index(arc);
        if (!collision_arc_index.exists)
        {
            nullify(this);
            return;
        }

        int closest_index = 0;
        float closest_similarity = -1;

        for (int adjacent_arc_index = collision_arc_index.data - 2; adjacent_arc_index <= collision_arc_index.data + 2; ++adjacent_arc_index)
        {
            int current_index = adjacent_arc_index;
            Arc current_arc = target.at(ref current_index);
            optional<Vector3> current_intersection_point = PlanetariaIntersection.arc_path_intersection(current_arc, begin, end);
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

        optional<Arc> closest_arc = target.at(ref closest_index);
        if (!closest_arc.exists)
        {
            nullify(this);
            return;
        }

        optional<Vector3> closest_intersection_point = PlanetariaIntersection.arc_path_intersection(closest_arc.data, begin, end);
        if (!closest_intersection_point.exists)
        {
            nullify(this);
            return;
        }

        arc_index = closest_index;
        interpolator_angle = closest_arc.data.position_to_angle(closest_intersection_point.data);
    }

    private static void nullify(BlockInteractor block_interactor)
    {
        Debug.Log("Critical Err0r.");
        block_interactor.target = null;
        block_interactor.arc_index = 0;
        block_interactor.interpolator_angle = 0f;
        block_interactor.half_height = 0f;
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