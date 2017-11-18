﻿using System.Collections.Generic;
using UnityEngine;

public class BlockCollision
{
    public static optional<BlockCollision> block_collision(Arc arc, Block block, Vector3 last_position, Vector3 current_position, float extrusion)
    {
        optional<ArcVisitor> arc_visitor = block.arc_visitor(arc);
        if (!arc_visitor.exists)
        {
            return new optional<BlockCollision>();
        }
        NormalizedCartesianCoordinates begin = new NormalizedCartesianCoordinates(last_position);
        NormalizedCartesianCoordinates end = new NormalizedCartesianCoordinates(current_position);
        optional<Vector3> intersection_point = PlanetariaIntersection.arc_path_intersection(arc, begin, end, extrusion); //TODO: check .data
        if (!intersection_point.exists)
        {
            return new optional<BlockCollision>();
        }
        BlockCollision result = new BlockCollision();
        float angle = arc.position_to_angle(intersection_point.data);
        result.geometry_visitor_variable = GeometryVisitor.geometry_visitor(arc_visitor.data, angle, extrusion);
        result.distance_variable = (intersection_point.data - current_position).sqrMagnitude;
        result.block_variable = block;
        result.active = true;
        return result;
    }

    public void move(float delta_length, float extrusion)
    {
        geometry_visitor_variable = geometry_visitor.move_position(delta_length, extrusion);
    }

    public NormalizedCartesianCoordinates position()
    {
        return new NormalizedCartesianCoordinates(geometry_visitor.position());
    }

    public NormalizedCartesianCoordinates normal()
    {
        return new NormalizedCartesianCoordinates(geometry_visitor.normal());
    }
    
    public bool active
    {
        get
        {
            return block_variable.active && active_variable;
        }
        set
        {
            active_variable = value;
        }
    }

    public Block block
    {
        get
        {
            return block_variable;
        }
    }

    public float distance
    {
        get
        {
            return distance_variable;
        }
    }

    public GeometryVisitor geometry_visitor
    {
        get
        {
            return geometry_visitor_variable;
        }
    }

    private bool active_variable;
    private Block block_variable;
    private float distance_variable;
    private GeometryVisitor geometry_visitor_variable;
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