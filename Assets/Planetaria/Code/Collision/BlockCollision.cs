using UnityEngine;

namespace Planetaria
{
    public class BlockCollision
    {
        public static optional<BlockCollision> block_collision(Arc arc, Block block, PlanetariaCollider collider, PlanetariaTransform transformation)
        {
            optional<ArcVisitor> arc_visitor = block.arc_visitor(arc);
            if (!arc_visitor.exists)
            {
                return new optional<BlockCollision>();
            }
            NormalizedCartesianCoordinates begin = transformation.previous_position;
            NormalizedCartesianCoordinates end = transformation.position;
            float extrusion = transformation.scale;
            optional<Vector3> intersection_point = PlanetariaIntersection.arc_path_intersection(arc, begin, end, extrusion); //TODO: check .data
            if (!intersection_point.exists)
            {
                return new optional<BlockCollision>();
            }
            BlockCollision result = new BlockCollision();
            float angle = arc.position_to_angle(intersection_point.data);
            result.geometry_visitor = GeometryVisitor.geometry_visitor(arc_visitor.data, angle, extrusion);
            result.distance = (intersection_point.data - transformation.position.data).sqrMagnitude;
            result.block = block;
            result.collider = collider;
            result.active = true;
            return result;
        }

        public void move(float delta_length, float extrusion)
        {
            geometry_visitor = geometry_visitor.move_position(delta_length, extrusion);
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
                return block.active && active_variable;
            }
            private set
            {
                active_variable = value;
            }
        }

        public Block block { get; private set; }
        public PlanetariaCollider collider { get; private set; }
        public float distance { get; private set; }
        public GeometryVisitor geometry_visitor { get; private set; }

        private bool active_variable;
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